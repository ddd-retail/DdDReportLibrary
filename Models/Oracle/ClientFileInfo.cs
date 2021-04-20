namespace ReportLibrary.Models.Oracle
{
    public class ClientFileInfo
    {
        public int ClientId { get; set; }
        public int SquareMeters { get; set; }
        public decimal CostPerSquareMeter { get; set; }
        public int TypeOfStore { get; set; }
        public int Location { get; set; }
    }
}