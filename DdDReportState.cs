using System.Data.SqlClient;
namespace ReportLibrary
{
    public class DdDReportState
    {
        public static string currentUser = "";
        public static string currentChain = "";

        public static string ChainConnectionString(string chain)
        {
            using (var conn = new SqlConnection(ConnectionHandler.SqlConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = $"select ConnString from ConnectionStrings where chain = '{chain}'";
                    var res = cmd.ExecuteScalar();
                    if (res == null)
                    {
                        return ConnectionHandler.SqlConnectionString;
                    }
                    conn.Close();
                    return res.ToString();
                }
            }
        }
    }
}
