using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using AppManager.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppManager.Data;

public class DatabaseController : Controller
{
    private string _connectionStringTemplate = "Server={0};Database={1};User Id={2};Password={3};MultipleActiveResultSets=True;TrustServerCertificate=True;";

    private static List<DatabaseConnectionModel> _connections = new List<DatabaseConnectionModel>();
    private readonly ApplicationDbContext _context;


    public DatabaseController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    [HttpGet]
    public IActionResult Connect()
    {

        _connections = _context.DatabaseConnections.Select(db => new DatabaseConnectionModel
        {
            Id = db.Id,
            Server = db.Server,
            DatabaseName = db.DatabaseName,
            Username = db.Username,
            Password = db.Password,
            IsConnected = false 
        }).ToList();

        return View(_connections); 
    }


    [HttpPost]
    public async Task<IActionResult> Connect(DatabaseConnectionModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                string connectionString = string.Format(_connectionStringTemplate, model.Server, model.DatabaseName, model.Username, model.Password);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    model.IsConnected = true;
                }


                _connections.Add(model);
                SaveDatabaseConnection(model);


                var newDatabase = new DatabaseModel
                {
                    Name = model.DatabaseName
                };
                _context.Databases.Add(newDatabase); 
                _context.SaveChanges();               

                ViewBag.Message = "Bağlantı başarılı!";
            }
            catch (Exception)
            {
                model.IsConnected = false;
                ViewBag.Message = "Bağlantı başarısız. Lütfen bilgilerinizi kontrol edin.";
            }
        }

        return View(_connections);
    }

    [HttpPost]
    public IActionResult DeleteConnection(int id)
    {
        var connection = _connections.FirstOrDefault(c => c.Id == id);
        if (connection != null)
        {
            _connections.Remove(connection);
            DeleteDatabaseConnection(connection);  
        }

        return RedirectToAction("Connect");
    }

    private void SaveDatabaseConnection(DatabaseConnectionModel model)
    {
        try
        {

            var newConnection = new DatabaseConnectionModel
            {
                Server = model.Server,
                DatabaseName = model.DatabaseName,
                Username = model.Username,
                Password = model.Password
            };

            _context.DatabaseConnections.Add(newConnection); 
            _context.SaveChanges(); 
        }
        catch (Exception ex)
        {

            ViewBag.ErrorMessage = $"Veritabanına kaydetme sırasında bir hata oluştu: {ex.Message}";
        }
    }



    private void DeleteDatabaseConnection(DatabaseConnectionModel model)
{
    var connectionToDelete = _context.DatabaseConnections
                                     .FirstOrDefault(db => db.Server == model.Server && 
                                                          db.DatabaseName == model.DatabaseName && 
                                                          db.Username == model.Username && 
                                                          db.Password == model.Password);
    
    if (connectionToDelete != null)
    {
        _context.DatabaseConnections.Remove(connectionToDelete);
        _context.SaveChanges(); 
    }
}

}
