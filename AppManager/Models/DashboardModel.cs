namespace AppManager.Models
{
    public class DashboardModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ChartModel> Charts { get; set; } = new List<ChartModel>();
    }
}
