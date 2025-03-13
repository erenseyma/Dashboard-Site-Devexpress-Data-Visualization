using System.ComponentModel.DataAnnotations;

namespace AppManager.Models
{
    public class DashboardCreateViewModel
    {
        public int Id { get; set; }
        public List<DatabaseModel> Databases { get; set; } = new List<DatabaseModel>();

        [Required]
        public int SelectedDatabaseId { get; set; }

        [Required]
        public string SqlQuery { get; set; }

        [Required]
        public List<string> SelectedChartTypes { get; set; } = new List<string>();

        [Required]
        public string DashboardName { get; set; }

        public List<ChartModel> Charts { get; set; } = new List<ChartModel>();
    }
}
