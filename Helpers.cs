using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using DdDRetail.Common.Logger.NLog;

namespace ReportLibrary
{
    public static class Helpers
    {
        static string[] timeDimensions = { "[Time].[Year].[Year]", "[Time].[Month].[Month]", "[Time].[Week].[Week]", "[Time].[PK_Date].[PK_Date]", "[Time].[YTD].[YTD]", "[Time].[MTD].[MTD]" };
        static string[] partKeyFigures = { "[Measures].[NettoomsaetningPart]", "[Measures].[BruttoavancePart]", "[Measures].[UltimolagerPart]", "[Measures].[PrimolagerPart]" };
        static string[] GTpartKeyFigures = { "[Measures].[NettoomsaetningGTPart]", "[Measures].[BruttoavanceGTPart]", "[Measures].[UltimolagerGTPart]", "[Measures].[PrimolagerGTPart]" };

        private static NLogger logger = new NLogger("debug");

        public static bool forceDebug = false;
        public static bool IsTimeDimension(string dbName)
        {
            return Array.Exists(timeDimensions, x => x == dbName);
        }

        public static bool IsPartKeyFigure(string keyfigure)
        {
            return Array.Exists(partKeyFigures, x => x == keyfigure);
        }

        public static bool IsGTPartKeyFigure(string keyfigure)
        {
            return Array.Exists(GTpartKeyFigures, x => x == keyfigure);
        }

        public static string LastName(string dbName)
        {
            int start = dbName.LastIndexOf('[') + 1, end = dbName.LastIndexOf(']');
            return dbName.Substring(start, end - start);
        }

        public static string DateTimeFormatString(string dbName)
        {
            string lastName = LastName(dbName);
            if (lastName == "Year")
                return "yyyy";
            else if (lastName == "Month")
                return "MMMM";
            else if (lastName == "Date")
                return "dd";

            return "d";
        }

        static public void sendEmail(string to, string header, string body, Attachment attachment)
        {
            Helpers.Debug("Sending email to: {0}", to);
            MailMessage m;
            try
            {
                // m = new MailMessage("lolcat@lolcat.dk", to);
                m = new MailMessage("DdDreport@dddretail.com", to);

            }
            catch (Exception e)
            {
                // most likely invalid email
                Helpers.Debug("mail message failed with: " + e.Message);
                return;
            }
            m.Subject = header;
            m.Body = body;
            if (attachment != null)
                m.Attachments.Add(attachment);
            SmtpClient client = new SmtpClient("pasmtp.tele.dk");
            try
            {
                client.Send(m);
            }
            catch (Exception exception)
            {
                Helpers.error("Could not send email, error {0}", exception.Message);
                Helpers.Debug("Could not send email, error {0}\n{1}", exception.Message, exception.InnerException);
            }
        }

        static public bool ChainUsesCurrency(string chainName)
        {

            //MGA: 30.08.2010 : Does not work on chains with more than one currency
            // return true;
            int currencies = 0;

            using (SqlConnection connection = new SqlConnection(ReportLibrary.DdDReportState.ChainConnectionString(chainName)))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = String.Format("SELECT distinct currency FROM [{0}].[dbo].[ChainConcernRelation] where chain = (Select id from Chain where name = '{0}')", chainName);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                            ++currencies;
                    }

                    Helpers.Debug("Currency count for chain " + chainName + " was " + currencies);
                }
            }

            return currencies > 1;
        }

        static public string toDbString(double p)
        {
            return p.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        static public double fromDbDouble(object o)
        {
            return System.Convert.ToDouble(o);
        }

#if false
        static private double fromDbDouble(object o)
        {
            return System.Convert.ToDouble(String.Format("{0}", o).Replace(".", ","));
        }
#endif

        public static string RemoveSpecialChars(string input)
        {
            string output = "";
            foreach (char c in input.ToCharArray())
                if (c != ' ' && c != '.' && c != ',' && c != '-' && c != '/')
                    output += c;
            return output;
        }

        #region debug

        // FIXME: maps arrays directly, this is not intuitive
        public static string niceDebugOutput(string format, object[] paramlist)
        {
            if (paramlist.Length == 0)
                return format;

            for (int i = 0; i < paramlist.Length; ++i)
            {
                try
                {
                    Type t = paramlist[i].GetType();
                    if (!(t.IsValueType || t == typeof(string) || t.IsEnum))
                        paramlist[i] = LitJson.JsonMapper.ToJson(paramlist[i]);
                }
                catch { }
            }

            return string.Format(format, paramlist);
        }

        static bool outputToConsole = false;


        public static void Debug(string format, params object[] paramlist)
        {

            bool writeDebug = false;
            bool WriteLocalLogFile = true; //MGA fix, should be false.
            if (writeDebug)
            {
                if (!outputToConsole)
                {
                    logger.Debug(niceDebugOutput(format, paramlist));
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(niceDebugOutput(format, paramlist));
                }
            }
            if (WriteLocalLogFile)
            {
                logger.Debug(niceDebugOutput(format, paramlist));
            }
        }

        public static void error(string format, params object[] paramlist)
        {
            //MGA: 09092010
            //if (!outputToConsole)
            //{
            //    if (file == null)
            //        file = new System.IO.StreamWriter(@"etl.log", true);
            //    file.WriteLine(String.Format("{0}: ERROR - ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")) + niceDebugOutput(format, paramlist));
            //}
            //else
            //{
            //    System.Diagnostics.Debug.WriteLine("ERROR: " + niceDebugOutput(format, paramlist));
            //}

            // errors are fatal
            //System.Environment.Exit(0);
        }

        #endregion

        #region currency

        static private void InsertConversionRate(string name, string shortName, double rate, OleDbConnection SQLServer)
        {
            System.DateTime time = System.DateTime.Now;

            using (OleDbCommand cmd = SQLServer.CreateCommand())
            {
                cmd.CommandText = String.Format("INSERT INTO [ddd].[dbo].[Valuta] VALUES ('{0}', '{1}', {2}, {3}, {4})",
                                  name, shortName, time.Year, time.Month, rate.ToString(System.Globalization.CultureInfo.InvariantCulture));
                cmd.ExecuteNonQuery();
            }
        }

        static private void InsertNewConversionRates(OleDbConnection SQLServer)
        {
            string url = "http://nationalbanken.statistikbank.dk/statbank5a/selectvarval/saveselections.asp?MainTable=DNVALD&PLanguage=0&TableStyle=&Buttons=5&PXSId=105719&IQY=&TC=&ST=ST&rvar0=&rvar1=&rvar2=&rvar3=&rvar4=&rvar5=&rvar6=&rvar7=&rvar8=&rvar9=&rvar10=&rvar11=&rvar12=&rvar13=&rvar14=";

            Dictionary<string, string> countryCodes = new Dictionary<string, string>();
            countryCodes.Add("Amerikanske dollar", "us");
            countryCodes.Add("Britiske pund", "uk");
            countryCodes.Add("Svenske kroner", "se");
            countryCodes.Add("Norske kroner", "no");
            countryCodes.Add("Islandske kroner", "is");
            countryCodes.Add("Schweiziske franc", "ch");
            countryCodes.Add("Estiske kroon", "ee");
            countryCodes.Add("Lettiske lats", "lv");
            countryCodes.Add("Litauiske litas", "lt");
            countryCodes.Add("Polske zloty", "pl");
            countryCodes.Add("Tjekkiske koruna", "cz");
            countryCodes.Add("Ungarske forint", "hu");
            countryCodes.Add("Russiske rubel", "ru");

            WebRequest myWebRequest = WebRequest.Create(url);
            WebResponse myWebResponse = myWebRequest.GetResponse();
            Stream ReceiveStream = myWebResponse.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("iso-8859-1");

            using (StreamReader readStream = new StreamReader(ReceiveStream, encode))
            {
                string line;
                while ((line = readStream.ReadLine()) != null)
                {
                    int index = line.IndexOf("Euro");
                    if (index != -1)
                    {
                        int trIndex = line.IndexOf("<tr>", index);
                        string tmp = line.Substring(0, trIndex);
                        int tdStartIndex = tmp.LastIndexOf("<td class=No>") + "<td class=No>".Length;
                        int tdLastIndex = tmp.LastIndexOf("</td>");

                        double euroRate = System.Convert.ToDouble(line.Substring(tdStartIndex, tdLastIndex - tdStartIndex));

                        InsertConversionRate("Dansk", "dk", euroRate / 100, SQLServer);

                        line = line.Substring(tdLastIndex);

                        foreach (KeyValuePair<string, string> kvp in countryCodes)
                        {
                            index = line.IndexOf(kvp.Key);

                            trIndex = line.IndexOf("<tr>", index);
                            tmp = line.Substring(0, trIndex);
                            tdStartIndex = tmp.LastIndexOf("<td class=No>") + "<td class=No>".Length;
                            tdLastIndex = tmp.LastIndexOf("</td>");

                            string strRate = line.Substring(tdStartIndex, tdLastIndex - tdStartIndex);
                            strRate = strRate.Replace("1 ", "1");

                            double rate = System.Convert.ToDouble(strRate);

                            line = line.Substring(tdLastIndex);

                            double r;

                            if (rate != 0)
                                r = euroRate / rate;
                            else
                                r = 0;

                            InsertConversionRate(kvp.Key, kvp.Value, r, SQLServer);
                        }

                        // done
                        return;
                    }
                }
            }

            myWebResponse.Close();
        }

        public static string CurrencyFromConcernId(int id, SqlConnection SQLServer)
        {
            string currency = "";

            using (var command = SQLServer.CreateCommand())
            {
                command.CommandText = String.Format("SELECT currency from ChainConcernRelation where concern = {0}", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        currency = reader.GetString(0);
                }
            }

            return currency;
        }

        public static string CurrencyFromConcernName(string concernName, SqlConnection SQLServer)
        {
            string currency = "";

            using (var command = SQLServer.CreateCommand())
            {
                command.CommandText = String.Format("SELECT currency from ChainConcernRelation where concern = (SELECT id from Koncern where name = '{0}')", concernName);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        currency = reader.GetString(0);
                }
            }

            return currency;
        }

        private static readonly IDictionary<string, double> ConversionRateCache = new Dictionary<string, double>();
        public static double ConversionRate(string currency, SqlConnection SQLServer, SqlTransaction transaction = null)
        {
            // If conversion rate has already been found take it from cache
            if (ConversionRateCache.ContainsKey(currency))
            {
                return ConversionRateCache[currency];
            }

            if (string.IsNullOrEmpty(currency.Trim()))
            {
                // Helpers.debug("Error: Currency name is : " + currency);
                return 1;
            }

            double conversionRate = 1;
            using (var cmd = SQLServer.CreateCommand())
            {
                if (transaction != null) cmd.Transaction = transaction;
                cmd.CommandText = $"SELECT currency FROM dbo.Valuta2 where shortName = '{currency}'";
                using (var reader = cmd.ExecuteReader())
                    if (reader.Read())
                        conversionRate = reader.GetDouble(0);
            }
            
            // Add found conversion rate to cache
            ConversionRateCache.Add(currency, conversionRate);

            return conversionRate;
        }

        public static Dictionary<string, double> ConversionRates(SqlConnection SQLServer)
        {
            var rates = new Dictionary<string, double>();

            using (var cmd = SQLServer.CreateCommand())
            {
                cmd.CommandText = String.Format("SELECT shortName, currency FROM dbo.Valuta2");
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        if (!rates.ContainsKey(reader.GetString(0)))
                            rates.Add(reader.GetString(0), reader.GetDouble(1));
            }

            return rates;
        }
        #endregion
    }
}