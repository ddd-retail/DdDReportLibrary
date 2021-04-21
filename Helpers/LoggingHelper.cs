using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportLibrary.Helpers
{
    public static class LoggingHelper
    {
        private static readonly ILogger Logger = LogManager.GetLogger("debug");
        public static bool ForceDebug = false;

        public static void Debug(string format, params object[] paramList)
        {
            if (!ForceDebug) return;

            var logEvent = new LogEventInfo(LogLevel.Debug, "", NiceDebugOutput(format, paramList));
            logEvent.Properties.Add("client", SettingsContext.ChainName);
            Logger.Log(logEvent);
        }

        public static string NiceDebugOutput(string format, object[] paramList)
        {
            if (paramList.Length == 0)
                return format;

            for (int i = 0; i < paramList.Length; ++i)
            {
                try
                {
                    Type t = paramList[i].GetType();
                    if (!(t.IsValueType || t == typeof(string) || t.IsEnum))
                        paramList[i] = Newtonsoft.Json.JsonConvert.SerializeObject(paramList[i]);
                }
                catch { }
            }

            return string.Format(format, paramList);
        }
    }
}
