using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace ReportLibrary
{
    public class User
    {
        public static bool IsChainUser(int id)
        {
            bool isChainUser = false;

            using (SqlConnection connection = new SqlConnection(ReportLibrary.ConnectionHandler.SqlConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = String.Format("SELECT isChain FROM ddd.dbo.Users2 where id={0}", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            isChainUser = reader.GetInt32(0) == 1;

                        Helpers.Debug("The user : " + id + " is a chain user : " + isChainUser);
                    }
                }
            }

            return isChainUser;
        }

        public static string Currency(int id)
        {
            string currency = "";

            using (SqlConnection connection = new SqlConnection(DdDReportState.ChainConnectionString(DdDReportState.currentChain)))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = String.Format("SELECT currency FROM ddd.dbo.Users2 where id={0}", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            currency = reader.GetString(0);
                    }
                }
            }

            return currency;
        }

        static public bool UsesCurrency(int id)
        {
            string cubeName = CubeName(id);

            if (String.IsNullOrEmpty(cubeName))
                return false;
            else
                return Helpers.ChainUsesCurrency(cubeName);
        }


        static public string CubeName(int id)
        {
            string cubename = "";
            Helpers.Debug("Trying to get cubename by chain: " + DdDReportState.currentChain);
            var chainConnString = DdDReportState.ChainConnectionString(DdDReportState.currentChain);
            Helpers.Debug("Got the chain connectionString: " + chainConnString);
            using (SqlConnection connection = new SqlConnection(DdDReportState.ChainConnectionString(DdDReportState.currentChain)))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = String.Format("SELECT cubeName FROM ddd.dbo.Users2 WHERE id = {0}", id);
                    Helpers.Debug("SQL to fire: " + cmd.CommandText);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            cubename = reader.GetString(0);
                    }
                }
            }

            return Helpers.RemoveSpecialChars(cubename);
        }

      
        static public string Email(int id)
        {
            string email = "";

            using (SqlConnection connection = new SqlConnection(DdDReportState.ChainConnectionString(DdDReportState.currentChain)))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = String.Format("SELECT email FROM ddd.dbo.Users2 WHERE id = {0}", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            email = reader.GetString(0);
                    }
                }
            }

            return email;
        }

        static public bool FirstLogin(int id)
        {
            bool isFirstLogin = true;

            using (SqlConnection connection = new SqlConnection(DdDReportState.ChainConnectionString(DdDReportState.currentChain)))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = String.Format("SELECT * from ddd.dbo.DdDreportActiveUsers WHERE id = {0}", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            isFirstLogin = false;
                    }

                    if (isFirstLogin)
                    {
                        var cube = "";
                        var username = "";
                        cmd.CommandText = String.Format("SELECT username,cubeName FROM ddd.dbo.Users2 WHERE id = {0}", id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                username = reader.GetString(0);
                                cube = reader.GetString(1);
                            }
                        }

                        cmd.CommandText = String.Format("INSERT INTO ddd.dbo.DdDreportActiveUsers values ({0},'{1}','{2}',CURRENT_TIMESTAMP)", id,username,cube);
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            return isFirstLogin;
        }
        // eu => de-DE is a bad hack to fix displaying euro instead of local currency
        static Dictionary<string, string> cultureDict = new Dictionary<string, string>() { 
                { "  ", "da-DK" }, { "dk", "da-DK" }, { "eu", "de-DE" }, { "us", "en-US" }, { "uk", "en-GB" }, { "se", "sv-SE" }, { "no", "nn-NO" }, { "is", "is-IS" }, { "ch", "de-CH" },
                { "ee", "et-EE" }, { "lv", "lv-LV" }, { "lt", "lt-LT" }, { "pl", "pl-PL" }, { "cz", "cs-CZ" }, { "hu", "hu-HU" }, { "ru", "ru-RU" }, { "fr", "fr-FR "} };

        public static string Culture(int id)
        {
            if (ReportLibrary.User.IsChainUser(id))
                return cultureDict[ReportLibrary.User.Currency(id)];
            else
                return "da-DK";
        }
    }
}
