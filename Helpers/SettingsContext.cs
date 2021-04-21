using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportLibrary.Helpers
{
    public static class SettingsContext
    {
        public const int GuestProductId = 9999999;
        public const int GuestCustomerId = 9999999;


        #region AppSettings

        private static readonly NameValueCollection AppSettings = ConfigurationManager.AppSettings;

        public static int ProcessingRetries => int.Parse(AppSettings["ProcessingRetries"]);

        public static int MaxDegreeOfParallelism
        {
            get
            {
                var result = int.Parse(AppSettings["MaxDegreeOfParallelism"]);
                return result == 0 ? Environment.ProcessorCount : result;
            }
        }

        public static int EtlChunkSize => int.Parse(AppSettings["EtlChunkSize"]);
        public static bool SendEmails => bool.Parse(AppSettings["SendEmails"]);

        public static bool EnablePrimoStock => bool.Parse(AppSettings["EnablePrimoStock"]);

        #endregion

        public static string ChainName { get; set; }
        public static int CubeId { get; set; }
        public static bool SendAutomaticReports { get; set; }
        public static DateTime EtlrelatedDay { get; set; }
        public static bool ProcessCube { get; set; }
        public static bool RecreateCube { get; set; }
        public static bool OnlyAutomaticReports { get; set; }
        public static bool SkipAutomaticReports { get; set; }
        public static bool SkipExpensive { get; set; }
        public static bool ForceGuests { get; set; }
        public static DateTime? FixedStartDate { get; set; }
        public static bool ForcePrimoStock { get; set; }
        public static bool ForceProductReload { get; set; }
    }
}
