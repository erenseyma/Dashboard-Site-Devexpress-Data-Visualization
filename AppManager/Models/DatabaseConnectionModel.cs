namespace AppManager.Models
{
    public class DatabaseConnectionModel
    {
        public int Id { get; set; }
        public string Server { get; set; }
        public string DatabaseName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsConnected { get; set; }
    }
}
