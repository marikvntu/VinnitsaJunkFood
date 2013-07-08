namespace VinnitsaJunkFood.DataAccessLayer{
    public static class ConnectionHelper{
        public static string GetConnectionString() {
            string connectionString = "";
            var configConnectionStrings = System.Web.Configuration.WebConfigurationManager.ConnectionStrings;
            if (configConnectionStrings.Count > 0)            {
                connectionString = configConnectionStrings.Count == 2 ?
                    configConnectionStrings[1].ToString() :
                    configConnectionStrings[0].ToString();
            }            
            return connectionString;      
        }
    }
}