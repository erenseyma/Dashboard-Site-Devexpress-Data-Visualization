namespace AppManager.Models
{
    public class ChartModel
    {
        public int Id { get; set; }


        public string ChartType { get; set; }


        public string DataQuery { get; set; }


        public string Name { get; set; }


        public int DashboardId { get; set; }


        public DashboardModel Dashboard { get; set; }
        public string ChartImage { get; set; }
        public int DatabaseId { get; set; }
    }
}
