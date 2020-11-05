using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.AnalysisServices.AdomdClient;
using System.Diagnostics;
using System.Data.OleDb;
using i18n;
using System.Data.SqlClient;

namespace ReportLibrary
{
    public static class AdoMD
    {
        public static void FixPartValues(DataTable data, List<KeyFigure> keyFigures, Dimension dimension, Dictionary<Dimension, string[]> restrictions,
                                         string connectionString, int userID, SqlConnection SQLServer)
        {
            bool isAnyPartOrGTPart = false;
            foreach (var kf in keyFigures)
            {
                if (Helpers.IsPartKeyFigure(kf.dbName) ||
                    Helpers.IsGTPartKeyFigure(kf.dbName))
                {
                    isAnyPartOrGTPart = true;
                    break;
                }
            }

            if (isAnyPartOrGTPart)
            {
                Dictionary<Dimension, string[]> GTrestrictions = new Dictionary<Dimension, string[]>();
                foreach (var kvp in restrictions)
                {
                    if (kvp.Key == dimension)
                        GTrestrictions.Add(kvp.Key, new string[] { });
                    else
                        GTrestrictions.Add(kvp.Key, kvp.Value);
                }

                foreach (DataRow row in data.Rows)
                {
                    for (int index = 1; index < row.Table.Columns.Count; ++index)
                    {
                        KeyFigure kf = keyFigures[index - 1];
                        if (Helpers.IsPartKeyFigure(kf.dbName))
                        {
                            row[index] = FormatPartKeyFigureDouble(row[index], new List<Dimension> { dimension }, new List<KeyFigure> { kf }, restrictions,
                                                                   connectionString, userID, SQLServer);
                        }
                        else if (Helpers.IsGTPartKeyFigure(kf.dbName))
                        {
                            row[index] = FormatPartKeyFigureDouble(row[index], new List<Dimension> { dimension }, new List<KeyFigure> { kf }, GTrestrictions,
                                                                   connectionString, userID, SQLServer);
                        }
                    }
                }
            }
        }

        public static string FormatPartKeyFigure(object o, List<Dimension> dimensions, List<KeyFigure> keyFigures, Dictionary<Dimension, string[]> restrictions,
                                                 string connectionString, int userID, SqlConnection SQLServer)
        {
            return String.Format("{0:f2}%", 100 * FormatPartKeyFigureDouble(o, dimensions, keyFigures, restrictions, connectionString, userID, SQLServer));
        }

        public static double FormatPartKeyFigureDouble(object o, List<Dimension> dimensions, List<KeyFigure> keyFigures, Dictionary<Dimension, string[]> restrictions,
                                                       string connectionString, int userID, SqlConnection SQLServer)
        {
            if (o is DBNull || System.Convert.ToDouble(o) == 0)
            {
                return 0;
            }
            else
            {
                if (dimensions.Count > 0)
                {
                    // FIXME: caching here would be nice
                    string rows = "NonEmpty({" + dimensions[dimensions.Count - 1].dbName + "})";
                    var dataTable = GetDataHelper(dimensions[dimensions.Count - 1].dbName, "", rows, keyFigures, restrictions, true, false, false, false, true, connectionString, userID, SQLServer);
                    double total = 0;
                    foreach (DataRow dataTableRow in dataTable.Rows)
                        total += (double)dataTableRow[dataTableRow.ItemArray.Count() - 1];

                    if (total != 0)
                        return System.Convert.ToDouble(o) / total;
                    else
                        return 1;
                }
                else
                {
                    return 1;
                }
            }
        }

        // whereOk = only one dimension in rows
        public static DataTable GetDataHelper(string dim, string dim2, string rows, List<KeyFigure> keyFigures, Dictionary<Dimension, string[]> restrictions,
                                              bool zeroSkip, bool zeroskipstock, bool whereOk, bool skipRows, bool nonEmpty,
                                              string connectionString, int userID, SqlConnection SQLServer)
        {
            AdomdConnection con = new AdomdConnection();
            con.ConnectionString = connectionString;
            Helpers.Debug("Using ADOMD String : " + con.ConnectionString);
            con.Open();

            List<string> kfs = keyFigures.ConvertAll(x => x.dbName);

            string columns = "{" + string.Join(", ", kfs.ToArray()) + "}";

            string where = "";
            string withMember = "";
            List<string> whereClauses = new List<string>();
            List<string> idsAdded = new List<string>();

            if (whereOk)
            {
                Debug.Assert(dim2 == "");

                Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();

                bool restrictionOnDimension = false;

                foreach (KeyValuePair<Dimension, string[]> r in restrictions)
                {
                    if (r.Key != null)
                    {
                        if (dim == r.Key.dbName && r.Value.Length > 0 && !skipRows)
                        {
                            if (!restrictionOnDimension)
                            {
                                restrictionOnDimension = true;
                                rows = "{";
                            }
                        }

                        List<string> whereClauseValues = new List<string>();
                        foreach (string v in r.Value)
                        {
                            if (v != null && v != I18n.GetString("latest"))
                            {
                                if (dim == r.Key.dbName && !skipRows && nonEmpty)
                                    rows += string.Format("NonEmpty({0}.&[{1}]), ", r.Key.dbName, v);
                                else if (dim == r.Key.dbName)
                                    rows += string.Format("{0}.&[{1}], ", r.Key.dbName, v);
                                else
                                    whereClauseValues.Add(string.Format("{0}.&[{1}]", r.Key.dbName, v));
                            }
                        }
                        if (whereClauseValues.Count > 0)
                            whereClauses.Add("{" + String.Join(", ", whereClauseValues.ToArray()) + "}");
                    }
                }

                if (restrictionOnDimension)
                {
                    rows = rows.Substring(0, rows.Length - 2) + "}";
                }
            }
            else
            {
                // if there is no restriction on the rows, then we can just use WITH MEMBER...
                // if there is restrictions on the rows, then we need to construct all the values we want manually
                bool restrictionOnRows = false;
                List<string> dimRestrictions = new List<string>();
                List<string> dim2Restrictions = new List<string>();

                foreach (KeyValuePair<Dimension, string[]> r in restrictions)
                {
                    if (r.Key.dbName == dim || r.Key.dbName == dim2)
                    {
                        restrictionOnRows = true;
                    }
                }

                // with can be used to restrict on several dimensions, but only ones that are not in rows
                foreach (KeyValuePair<Dimension, string[]> r in restrictions)
                {
                    if (restrictionOnRows)
                    {
                        if (r.Key.dbName == dim)
                        {
                            foreach (string v in r.Value)
                                if (v != I18n.GetString("latest"))
                                    dimRestrictions.Add(v);
                        }
                        else if (r.Key.dbName == dim2)
                        {
                            foreach (string v in r.Value)
                                if (v != I18n.GetString("latest"))
                                    dim2Restrictions.Add(v);
                        }
                        else
                        {
                            if (r.Value.Length > 0)
                            {
                                if (withMember == "")
                                    withMember += "WITH ";
                                withMember += "MEMBER " + r.Key.dbName.Substring(0, r.Key.dbName.LastIndexOf("]", r.Key.dbName.LastIndexOf("]") - 1) + 1) + "." + r.Key.id + " AS Aggregate ({";
                                bool inforeach = false; //MGA 24092010; week missing in r.
                                foreach (string v in r.Value)
                                {
                                    if (v != I18n.GetString("latest"))
                                    {
                                        inforeach = true;
                                        withMember += r.Key.dbName + ".&[" + v + "], ";
                                    }
                                }
                                //  Helpers.debug("infoeach1 is : " + inforeach);

                                if (inforeach)
                                    withMember = withMember.Substring(0, withMember.Length - 2) + "}) ";
                                else
                                    withMember = withMember.Substring(0, withMember.Length - 2) + "}) ";
                                whereClauses.Add(r.Key.id);
                            }
                        }
                    }
                    else
                    {
                        if (r.Value.Length > 0)
                        {
                            if (withMember == "")
                                withMember += "WITH ";
                            withMember += "MEMBER " + r.Key.dbName.Substring(0, r.Key.dbName.LastIndexOf("]", r.Key.dbName.LastIndexOf("]") - 1) + 1) + "." + r.Key.id + " AS Aggregate ({";
                            bool inforeach = false; //MGA 24092010; week missing in r.
                            foreach (string v in r.Value)
                            {
                                //What happens is the only one is latest?
                                if (v != I18n.GetString("latest"))
                                {
                                    inforeach = true;
                                    withMember += r.Key.dbName + ".&[" + v + "], ";
                                }
                            }

                            //  Helpers.debug("infoeach2 is : " + inforeach);
                            if (inforeach)
                                withMember = withMember.Substring(0, withMember.Length - 2) + "}) ";
                            else
                                withMember = withMember + "}) ";

                            whereClauses.Add(r.Key.id);
                        }
                    }

                    Helpers.Debug("Rows1: " + rows);
                    if (dimRestrictions.Count > 0 && dim2Restrictions.Count > 0)
                    {
                        rows = "{";
                        foreach (string dimRestriction in dimRestrictions)
                        {
                            foreach (string dim2Restriction in dim2Restrictions)
                            {
                                if (nonEmpty)
                                    rows += string.Format("(NonEmpty({0}.&[{1}]), NonEmpty({2}.&[{3}])), ", dim, dimRestriction,
                                                          dim2, dim2Restriction);
                                else
                                    rows += string.Format("({0}.&[{1}], {2}.&[{3}]), ", dim, dimRestriction,
                                                          dim2, dim2Restriction);

                            }
                        }
                        rows = rows.Substring(0, rows.Length - 2) + "}";
                        Helpers.Debug("Rows2: " + rows);

                    }
                    else if (dimRestrictions.Count > 0)
                    {
                        rows = "{";
                        foreach (string dimRestriction in dimRestrictions)
                        {
                            if (nonEmpty)
                                rows += string.Format("(NonEmpty({0}.&[{1}])), ", dim, dimRestriction);
                            else
                                rows += string.Format("({0}.&[{1}]), ", dim, dimRestriction);
                        }
                        rows = rows.Substring(0, rows.Length - 2) + "}";
                        Helpers.Debug("Rows3: " + rows);

                    }
                    else if (dim2Restrictions.Count > 0)
                    {
                        rows = "{";
                        foreach (string dimRestriction in dim2Restrictions)
                        {
                            if (nonEmpty)
                                rows += string.Format("(NonEmpty({0}), NonEmpty({1}.&[{2}])), ", dim, dim2, dimRestriction);
                            else
                                rows += string.Format("({0}, {1}.&[{2}]), ", dim, dim2, dimRestriction);
                        }
                        rows = rows.Substring(0, rows.Length - 2) + "}";
                        Helpers.Debug("Rows4: " + rows);

                    }
                }
            }

            if (whereClauses.Count > 0)
                where = " WHERE (" + string.Join(", ", whereClauses.ToArray()) + ")";

            if (dim.ToLower().StartsWith("[product]") && String.IsNullOrEmpty(dim2))
            {
                //problem if we also want to see eg. sold sold qty and this is > 0 but ultimolager = 0 then this does not appear.

                //This does not work when filtrering on multiple dimensions (set on axis). Uncommented gives larges values.
                //rows = String.Format("Non Empty(FILTER({0}, [Measures].[UltimoLager_Stk] <> 0 or  [Measures].[Afsætning] <> 0))", dim);

            }

            string query;
            if (skipRows)
                query = withMember + "SELECT " + columns + " on columns FROM [" + ConnectionHandler.adomdCubeName(userID) + "]" + where;
            else
                query = withMember + "SELECT " + columns + " on columns, " + rows + " ON ROWS FROM [" + ConnectionHandler.adomdCubeName(userID) + "]" + where;

            Helpers.Debug("ADOMD query for execution: " + query);

            AdomdDataAdapter adapter = new AdomdDataAdapter(query, con);
            DataTable table = new DataTable();

            Type type = null;
            if (Helpers.IsTimeDimension(dim))
                type = typeof(System.DateTime);
            else
                type = typeof(string);
            table.Columns.Add(new DataColumn(dim + ".[MEMBER_CAPTION]", type));

            foreach (string x in kfs)
                table.Columns.Add(new DataColumn(x, typeof(Double)));

            if (!String.IsNullOrEmpty(dim2))
            {
                if (Helpers.IsTimeDimension(dim2))
                    type = typeof(System.DateTime);
                else
                    type = typeof(string);
                table.Columns.Add(new DataColumn(dim2 + ".[MEMBER_CAPTION]", type));
            }

            adapter.Fill(table);
            con.Close();
            // FIXME: exception handling? connection pooling?

            DateTime timeNow = DateTime.Now;

            // currency
            if (User.IsChainUser(userID) && User.UsesCurrency(userID))
            {
                var conversionRate = ReportLibrary.Helpers.ConversionRate(User.Currency(userID), SQLServer);

                foreach (DataRow row in table.Rows)
                {
                    int index = 1;
                    foreach (KeyFigure k in keyFigures)
                    {
                        if (k.type == KeyFigureType.Money)
                        {
                            try
                            {
                                double val = System.Convert.ToDouble(row[index]);
                                row[index] = val * conversionRate;
                            }
                            catch (InvalidCastException)
                            {
                                row[index] = 0;
                            }
                        }
                        ++index;
                    }
                }
            }

            //Helpers.debug("count {0}", table.Rows.Count);

            if (zeroSkip)
            {
                List<int> row_indexes = new List<int>();
                int row_index = 0;

                var dims = 1;

                var dimsAtEnd = 0;

                if (!String.IsNullOrEmpty(dim2))
                    dimsAtEnd = 1;

                Helpers.Debug("ADOMD restrictions count:" + dims);
                Helpers.Debug("Columns count:" + table.Columns.Count);
                Helpers.Debug("Itemarray[0] count:" + table.Rows[0].ItemArray.Length);

                Type stringType = "".GetType();
                foreach (DataRow row in table.Rows)
                {
                    bool skip = true;
                    for (int i = dims; i < row.ItemArray.Length - dimsAtEnd; ++i)
                    {

                        // this is much faster than catching exceptions
                        Helpers.Debug("Data type : " + table.Columns[i].DataType + ", value: " + row.ItemArray[i]);

                        if (row.ItemArray[i] == DBNull.Value || table.Columns[i].DataType == stringType)
                            continue;

                        Helpers.Debug("item array value: " + (Double)row.ItemArray[i]);


                        try
                        {
                            if ((Double)row.ItemArray[i] != 0)
                            {
                                skip = false;
                                Helpers.Debug(" Zeroskip, skipped = false: " + row.ItemArray[i]);
                                break;
                            }
                        }
                        catch (System.InvalidCastException)
                        {
                            Helpers.Debug("Invalid cast with zeroskip: " + row.ItemArray[i]);

                            skip = false; //MGA NEW 27052013

                        }
                    }

                    if (skip)
                        row_indexes.Add(row_index);

                    ++row_index;
                }

                row_indexes.Reverse(); // otherwise the indexes don't match ;-)

                Helpers.Debug("tablerows count before : " + table.Rows.Count);
                foreach (int index in row_indexes)
                {

                    Helpers.Debug("Zeroskip, should remove index : " + index);
                    table.Rows.RemoveAt(index);
                }
                Helpers.Debug("tablerows count after : " + table.Rows.Count);
            }

            // Helpers.debug("count {0}", table.Rows.Count);

            return table;
        }
    }
}
