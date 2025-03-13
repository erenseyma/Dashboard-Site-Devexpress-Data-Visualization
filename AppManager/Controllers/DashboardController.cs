
using Microsoft.AspNetCore.Mvc;
using AppManager.Data;
using AppManager.Models;
using System.Linq;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Data.SqlClient;

namespace AppManager.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private string _connectionStringTemplate = "Server={0};Database={1};User Id={2};Password={3};MultipleActiveResultSets=True;TrustServerCertificate=True;";


        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var dashboards = _context.Dashboards
                .Include(d => d.Charts)
                .ToList();

            return View(dashboards);
        }

        public IActionResult Create()
        {
            var connectedDatabases = _context.DatabaseConnections
                .Select(db => new DatabaseModel
                {
                    Id = db.Id,
                    Name = db.DatabaseName
                })
                .ToList();

            if (!connectedDatabases.Any())
            {
                ViewBag.Message = "No connected databases found. Please ensure the databases are loaded and connected.";
            }

            var viewModel = new DashboardCreateViewModel
            {
                Databases = connectedDatabases
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddDatabase(DatabaseModel model)
        {
            if (ModelState.IsValid)
            {
                var newDatabase = new DatabaseModel
                {
                    Name = model.Name
                };

                _context.Databases.Add(newDatabase);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }


        [HttpPost]
        public IActionResult Create(DashboardCreateViewModel model, string ChartsData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var selectedDatabase = _context.DatabaseConnections.FirstOrDefault(db => db.Id == model.SelectedDatabaseId);

                    if (selectedDatabase == null)
                    {
                        ModelState.AddModelError("", "Veritabanı bulunamadı.");
                        return View(model);
                    }

                    // Dashboard'u oluşturuyoruz
                    var dashboard = new DashboardModel
                    {
                        Name = model.DashboardName
                    };

                    _context.Dashboards.Add(dashboard);
                    _context.SaveChanges();

                    if (!string.IsNullOrEmpty(ChartsData))
                    {
                        var charts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ChartModel>>(ChartsData);

                        foreach (var chart in charts)
                        {
                            // Grafiğin ismini de kaydediyoruz
                            var newChart = new ChartModel
                            {
                                Name = chart.Name,  // Grafik ismi burada tutuluyor
                                ChartType = chart.ChartType,
                                DataQuery = chart.DataQuery,
                                ChartImage = chart.ChartImage, // Base64 görselini kaydediyoruz
                                DashboardId = dashboard.Id,
                                DatabaseId = selectedDatabase.Id  // Veritabanı ile ilişkisini kaydediyoruz
                            };

                            _context.Charts.Add(newChart);
                        }

                        _context.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Dashboard oluşturulurken bir hata oluştu: " + ex.Message);
                }
            }

            return View(model);
        }




        [HttpPost]
        public async Task<IActionResult> GetChartData([FromBody] ChartDataRequestModel model)
        {
            var dbConnection = _context.DatabaseConnections.FirstOrDefault(db => db.Id == model.DatabaseId);
            if (dbConnection == null)
            {
                return BadRequest("Veritabanı bağlantısı bulunamadı.");
            }

            string connectionString = string.Format(
                "Server={0};Database={1};User Id={2};Password={3};MultipleActiveResultSets=True;",
                dbConnection.Server, dbConnection.DatabaseName, dbConnection.Username, dbConnection.Password
            );

            var data = new { labels = new List<string>(), values = new List<int>() };

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(model.Query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                // Grafik verilerini döndürüyoruz (örneğin, label ve value sütunları)
                                data.labels.Add(reader.GetString(0));  // X ekseni
                                data.values.Add(reader.GetInt32(1));   // Y ekseni
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Sorgu çalıştırılırken hata oluştu: {ex.Message}");
            }

            return Json(data);  // Grafik verilerini JSON formatında döndürüyoruz
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var dashboard = _context.Dashboards
                .Include(d => d.Charts)  // İlişkili chart'ları da alıyoruz
                .FirstOrDefault(d => d.Id == id);

            if (dashboard == null)
            {
                return NotFound();
            }

            // Öncelikle ilişkili chart'ları sil
            _context.Charts.RemoveRange(dashboard.Charts);

            // Sonra dashboard'u sil
            _context.Dashboards.Remove(dashboard);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var dashboard = _context.Dashboards
                .Include(d => d.Charts) 
                .FirstOrDefault(d => d.Id == id);

            if (dashboard == null)
            {
                return NotFound();
            }

            return View(dashboard); 

        }

        [HttpPost]
        public IActionResult SetAsHomeDashboard(int id)
        {
            var dashboard = _context.Dashboards.FirstOrDefault(d => d.Id == id);
            if (dashboard == null)
            {
                return NotFound();
            }


            var setting = _context.Settings.FirstOrDefault();
            if (setting == null)
            {
                setting = new Setting { HomeDashboardId = id };
                _context.Settings.Add(setting);
            }
            else
            {
                setting.HomeDashboardId = id;
            }

            _context.SaveChanges();

            return RedirectToAction("Home");
        }


        public IActionResult Home()
        {

            var setting = _context.Settings.FirstOrDefault();

            if (setting != null)
            {

                var homeDashboard = _context.Dashboards
                    .Include(d => d.Charts)
                    .FirstOrDefault(d => d.Id == setting.HomeDashboardId);

                return View(homeDashboard);
            }


            return View(null);
        }



    }
}