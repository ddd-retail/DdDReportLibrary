using ReportLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportLibrary.Helpers
{
    public class VersionHolder
    {
        public static List<DdDreportVersion> Versions = new List<DdDreportVersion>();

        public static DdDreportVersion GetCurrentVersion()
        {
            if (Versions.Count > 0)
                return Versions.Last();
            else
            {
                DdDreportVersion vh = new DdDreportVersion();
                vh.versionNumber = "2013.01.01";
                vh.reCreateAllDataForThisVersion = true;
                vh.reCreateCubeForThisVersion = true;
                vh.versionInfo =
                    "<table><tr><th>Version: 2013.01.01 gives access to:</th></tr></table>";
                Versions.Add(vh);

                vh = new DdDreportVersion();
                vh.versionNumber = "2013.06.01";
                vh.reCreateAllDataForThisVersion = true;
                vh.reCreateCubeForThisVersion = true;
                vh.versionInfo =
                    "<table><tr><th>Version: 2013.06.01 gives access to:</th></tr><tr><td>- Fixed reports </td></tr><tr><td>- Fast ultimo stock </td></tr><tr><td>- Enhanced user experience </td></tr></table>";
                Versions.Add(vh);

                vh = new DdDreportVersion();
                vh.versionNumber = "2013.07.01";
                vh.reCreateAllDataForThisVersion = false;
                vh.reCreateCubeForThisVersion = true;
                vh.versionInfo =
                   "<table><tr><th>Version: 2013.07.01 gives access to:</th></tr><tr><td>- Machine information in the ETL </td></tr><tr><td>- More intutive menu structure </td></tr></table>";

                vh.changeSteps.Add("IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'MachineCUBEXXX') CREATE TABLE MachineCUBEXXX (PK_machine_Id [int] not null, [Client] [int] NOT NULL, [MachineNumber] [int] NOT NULL, PRIMARY KEY CLUSTERED ([PK_machine_Id] ASC) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY];");

                Versions.Add(vh);

                vh = new DdDreportVersion();
                vh.versionNumber = "2013.08.01";
                vh.reCreateAllDataForThisVersion = false;
                vh.reCreateCubeForThisVersion = true;
                vh.versionInfo =
                   "<table><tr><th>Version: 2013.08.01 gives access to:</th></tr><tr><td>- Machine number as dimension </td></tr></table>";

                vh.changeSteps.Add("alter table [CUBEXXX].[dbo].CUBEXXXFACT add FK_MachineId [INT] NULL");
                vh.changeSteps.Add("Insert into MachineCUBEXXX values (0,0,0)");

                Versions.Add(vh);

                vh = new DdDreportVersion();
                vh.versionNumber = "2013.09.01";
                vh.reCreateAllDataForThisVersion = false;
                vh.reCreateCubeForThisVersion = true;
                vh.versionInfo =
                   "<table><tr><th>Version: 2013.09.01 Improves:</th></tr><tr><td>Stabillity of nightly cube builds </td></tr></table>";

                Versions.Add(vh);

                vh = new DdDreportVersion();
                vh.versionNumber = "2014.04.01";
                vh.reCreateAllDataForThisVersion = false;
                vh.reCreateCubeForThisVersion = true;
                vh.versionInfo =
                   "<table><tr><th>Version: 2014.04.01 Improves:</th></tr><tr><td>Complete rewrite of engine and introducing quick reports </td></tr></table>";

                Versions.Add(vh);

                vh = new DdDreportVersion();
                vh.versionNumber = "2014.07.01";
                vh.reCreateAllDataForThisVersion = false;
                vh.reCreateCubeForThisVersion = true;
                vh.versionInfo =
                   "<table><tr><th>Version: 2014.06.16 Improves:</th></tr><tr><td>Major bugfixing, and fixed reports as autoreports. Gender in customer club </td></tr></table>";

                Versions.Add(vh);

                vh = new DdDreportVersion();
                vh.versionNumber = "2014.09.01";
                vh.reCreateAllDataForThisVersion = false;
                vh.reCreateCubeForThisVersion = true;
                vh.versionInfo =
                   "<table><tr><th>Version: 2014.09.01 Improves:</th></tr><tr><td>Loyalty integration in DdDreport</td></tr></table>";

                Versions.Add(vh);

                vh = new DdDreportVersion();
                vh.versionNumber = "2014.10.01";
                vh.reCreateAllDataForThisVersion = false;
                vh.reCreateCubeForThisVersion = true;
                vh.versionInfo =
                   "<table><tr><th>Version: 2014.09.01 Improves:</th></tr><tr><td>Loyalty integration in DdDreport and naming of shops</td></tr></table>";

                Versions.Add(vh);

                vh = new DdDreportVersion();
                vh.versionNumber = "2014.10.14";
                vh.reCreateAllDataForThisVersion = false;
                vh.reCreateCubeForThisVersion = true;
                vh.versionInfo =
                   "<table><tr><th>Version: 2014.10.14 Improves:</th></tr><tr><td>Roll back naming of shops</td></tr></table>";

                Versions.Add(vh);

                vh = new DdDreportVersion();
                vh.versionNumber = "2015.01.29";
                vh.reCreateAllDataForThisVersion = false;
                vh.reCreateCubeForThisVersion = true;
                vh.versionInfo =
                   "<table><tr><th>Version: 2015.01.29 Improves:</th></tr><tr><td>ETL Management and Deliverynotes as a dimension</td></tr></table>";

                Versions.Add(vh);

                // Disabled this version if primo stock isn't enabled. Should be removed when customer move is done.
                if (SettingsContext.EnablePrimoStock)
                {
                    vh = new DdDreportVersion();
                    vh.versionNumber = "2019.03.15";
                    vh.reCreateAllDataForThisVersion = true;
                    vh.reCreateCubeForThisVersion = false;
                    vh.versionInfo =
                        "<table><tr><th>Version: 2019.03.15 Improves:</th></tr><tr><td>Adds primo stock column to ETLWClient table to track when primo stock was last run.</td></tr></table>";

                    vh.changeSteps.Add("IF COL_LENGTH('dbo.ETLWClients', 'PrimoStockDate') IS NULL BEGIN ALTER TABLE ETLWClients ADD PrimoStockDate [DateTime] NULL END");
                    vh.changeSteps.Add("TRUNCATE TABLE [CUBEXXX].[dbo].CUBEXXXFACT");
                    vh.changeSteps.Add("TRUNCATE TABLE [CUBEXXX].[dbo].ProductCUBEXXX");
                    vh.changeSteps.Add("TRUNCATE TABLE [CUBEXXX].[dbo].GroupsBridgeCUBEXXX");
                    vh.changeSteps.Add("TRUNCATE TABLE [CUBEXXX].[dbo].StoreCUBEXXX");

                    Versions.Add(vh);
                }

                return Versions.Last();
            }
        }

        public static string GetChangesSinceVersion(string version)
        {
            string msg = "";
            int versionAsInt = Convert.ToInt32(version.Replace(".", ""));
            foreach (var v in Versions)
            {
                if (Convert.ToInt32(v.versionNumber.Replace(".", "")) > versionAsInt)
                    msg += v.versionInfo + "<br>";
            }
            LoggingHelper.Debug("Changes since version: " + msg);
            return msg;
        }

        public static List<DdDreportVersion> GetChangesSinceVersionAsObjects(string version)
        {
            var resp = new List<DdDreportVersion>();
            int versionAsInt = Convert.ToInt32(version.Replace(".", ""));
            foreach (var v in Versions)
            {
                if (Convert.ToInt32(v.versionNumber.Replace(".", "")) > versionAsInt)
                    resp.Add(v);
            }
            return resp;
        }
    }
}
