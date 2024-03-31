namespace Openbook.Data
{
    public class DatabaseConnection
    {
        private readonly IConfiguration _configuration;

        public string DbConn;
        public DatabaseConnection(IConfiguration configuration)
        {
            _configuration = configuration;
            DbConn = _configuration.GetConnectionString("DefaultConnection");
        }
    }
}
