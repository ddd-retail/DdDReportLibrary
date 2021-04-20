using System;

namespace ReportLibrary.Models.Sql
{
    public class EtlWClient
    {
        public string  Chain { get; set; }
        public int ClientId { get; set; }
        public int RowNum { get; set; }
        public DateTime? PrimoStockDate { get; set; }
    }
}