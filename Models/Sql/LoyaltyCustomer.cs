namespace ReportLibrary.Models.Sql
{
    public class LoyaltyCustomer
    {
        public string postal { get; set; }
        public int year { get; set; }
        public string yearGroup { get; set; }
        public int clubnr { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string city { get; set; }
        public string mobile { get; set; }
        public string mail { get; set; }
        public string gender { get; set; }
        public string creationStore { get; set; }
        public bool foundInOldSystem { get; set; }
        public int discountLevel { get; set; }
        public string discountLevelName { get; set; }
        public string country { get; set; }

        public LoyaltyCustomer()
        {
            foundInOldSystem = false;
            discountLevelName = "None";
        }
    }
}