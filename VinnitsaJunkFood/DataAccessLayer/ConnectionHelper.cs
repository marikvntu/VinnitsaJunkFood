using System.Configuration;
using System.Web.Configuration;

namespace VinnitsaJunkFood.DataAccessLayer
{
    public static class ConnectionHelper
    {
        public static string GetConnectionString()
        {
            string connectionString = "";
            ConnectionStringSettingsCollection configConnectionStrings = WebConfigurationManager.ConnectionStrings;
            if (configConnectionStrings.Count > 0)
            {
                connectionString = configConnectionStrings.Count == 2
                    ? configConnectionStrings[1].ToString()
                    : configConnectionStrings[0].ToString();
            }
            return connectionString;
        }
    }
}