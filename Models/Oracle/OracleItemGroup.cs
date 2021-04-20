namespace ReportLibrary.Models.Oracle
{
    public class OracleItemGroup
    {
        public int varegruppeId { get; set; }
        public string varegruppeNavn { get; set; }
        public double moms { get; set; }
        public int koncernId { get; set; }
        public string koncernNavn { get; set; }
    }
}