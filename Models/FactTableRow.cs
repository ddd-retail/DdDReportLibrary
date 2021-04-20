using System;
using System.Collections.Generic;

namespace ReportLibrary.Models
{
    public class FactTableRow
    {
        public bool ok;

        public int customerId;
        public int storeId;
        public int productId;
        public DateTime timeId;
        public int hourId;
        public int discountId;
        public int returnId;

        public string uniqueBonNr;
        public int machineId;


        public Dictionary<string, object> keyFigures = new Dictionary<string, object>();
        // currency
        public string currency;
        public int year;
        public int month;
        public string KoncernName;
        public int klientNr;
        public int KoncernNummer;
    }
}