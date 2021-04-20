using System.Collections.Generic;

namespace ReportLibrary.Models
{
    public class DdDreportVersion
    {
        public string versionNumber = "";
        public string versionInfo = "";
        public bool reCreateCubeForThisVersion = false;
        public bool reCreateAllDataForThisVersion = false;
        public List<string> changeSteps = new List<string>();
    }
}
