using System.Configuration;

namespace ReportLibrary
{
    public static class ConnectionHandler
    {
        public static bool debug = false;

        public static string adomdCubeName(int id)
        {
            return User.CubeName(id);
        }

        public static string OlapConnectionString(int id)
        {
            return ConfigurationManager.ConnectionStrings["OlapConnectionString"].ConnectionString.Replace("{cubeName}", adomdCubeName(id));
        }

        public static string OlapConnectionString(string cubeName)
        {
            return ConfigurationManager.ConnectionStrings["OlapConnectionString"].ConnectionString.Replace("{cubeName}", cubeName);
        }

        public static string AdomdConnectionString(int id)
        {
            return ConfigurationManager.ConnectionStrings["AdomdConnectionString"].ConnectionString.Replace("{cubeName}", adomdCubeName(id));
        }

        public static string AdomdConnectionString(string cubename)
        {
            return ConfigurationManager.ConnectionStrings["AdomdConnectionString"].ConnectionString.Replace("{cubeName}", cubename);
        }

        public static string AdomdConnectionStringToAnalytics()
        {
            return ConfigurationManager.ConnectionStrings["AdomdConnectionStringToAnalytics"].ConnectionString;
        }
        
        public static string MsSqlConnectionIp => ConfigurationManager.AppSettings["MsSqlConnectionIp"];
        public static string SqlUserName => ConfigurationManager.AppSettings["SqlUserName"];
        public static string SqlPassword => ConfigurationManager.AppSettings["SqlPassword"];

        public static readonly string SqlConnectionString = $"Data Source={MsSqlConnectionIp};Database=ddd;UID={SqlUserName};Password={SqlPassword};Connect Timeout=0;";

        public static string OracleConnectionString => ConfigurationManager.ConnectionStrings["OracleConnectionString"].ConnectionString;
        public static string OracleConnectionString2 => ConfigurationManager.ConnectionStrings["OracleConnectionString2"].ConnectionString;
        public static string MdfFilePath => ConfigurationManager.AppSettings["MdfFilePath"];
        public static string LogFilePath => ConfigurationManager.AppSettings["LogFilePath"];
        public static string LogFilePath2 => ConfigurationManager.AppSettings["LogFilePath2"];

        public static readonly string DddAdminSqlConnectionString = $"Data Source={MsSqlConnectionIp};Database=DdDAdminMaster;User ID={SqlUserName};Password={SqlPassword};Max Pool Size=300";
    }
}
