using System;

namespace ReportLibrary.Models.Sql
{
    public class Product
    {
        public int PK_Product_Id { get; set; }
        public string Katalogvare_Id { get; set; }
        public int Koncern_Id { get; set; }
        public string Koncern_Navn { get; set; }
        public int Leverandør_Id { get; set; }
        public string Leverandør_Navn { get; set; }
        public int Varegruppe_Id { get; set; }
        public string Varegruppe_Navn { get; set; }
        public string Variant_EanNr { get; set; }
        public long VariantEdbNr { get; set; }
        public DateTime First_Insertion { get; set; }
        public string ProductGrouping { get; set; }
        public string Parameter0 { get; set; }
        public string Parameter1 { get; set; }
        public string Parameter2 { get; set; }
        public string Parameter3 { get; set; }
        public string Parameter4 { get; set; }
        public string Parameter5 { get; set; }
        public string Parameter6 { get; set; }
        public string Parameter7 { get; set; }
        public string Parameter8 { get; set; }
        public string Parameter9 { get; set; }

        public string ToUniqueIdentifier()
        {
            var identifier = $"{Koncern_Navn}-{Varegruppe_Id}-{Varegruppe_Navn}-{Katalogvare_Id}-{Variant_EanNr}-{Leverandør_Navn}-{Leverandør_Id}";
            identifier += $"-{Parameter0}";
            identifier += $"-{Parameter1}";
            identifier += $"-{Parameter2}";
            identifier += $"-{Parameter3}";
            identifier += $"-{Parameter4}";
            identifier += $"-{Parameter5}";
            identifier += $"-{Parameter6}";
            identifier += $"-{Parameter7}";
            identifier += $"-{Parameter8}";
            identifier += $"-{Parameter9}";
            return identifier;
        }
    }
}