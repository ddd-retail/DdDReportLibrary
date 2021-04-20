using System;
using System.Collections.Generic;

namespace ReportLibrary.Models.Oracle
{
    public class OracleProduct
    {
        public long variantEdbNr { get; set; }
        public long variantEanNr { get; set; }
        public int katalogvareId { get; set; }
        public int varegruppeId { get; set; }
        public string varegruppeNavn { get; set; }
        public int koncernId { get; set; }
        public string koncernNavn { get; set; }
        public string leverandør { get; set; }
        public int leverandørId { get; set; }
        public double salgsværdi { get; set; }
        public double moms { get; set; }
        public IList<string> parameters { get; set; }
        public DateTime FirstPurchase { get; set; }

        public OracleProduct()
        {
            parameters = new List<string>();
            for (var i = 0; i < 10; ++i)
                parameters.Add("");
        }

        public OracleProduct(OracleItemGroup itemGroup)
        {
            varegruppeId = itemGroup.varegruppeId;
            varegruppeNavn = itemGroup.varegruppeNavn;
            moms = itemGroup.moms;
            koncernId = itemGroup.koncernId;
            koncernNavn = itemGroup.koncernNavn;
            parameters = new List<string>();

            for (var i = 0; i < 10; ++i)
                parameters.Add("");
        }

        public string ToUniqueIdentifier()
        {
            return $"{koncernNavn}-{varegruppeId}-{varegruppeNavn}-{katalogvareId}-{variantEanNr}-{leverandør}-{leverandørId}-{string.Join("-", parameters)}";
        }
    }
}