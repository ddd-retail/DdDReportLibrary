using System;

namespace ReportLibrary.Models
{
    public class OracleRow
    {
        public int brugernummer;
        public double salgspris;
        public double rabat;
        public double moms;
        public double kostpris;
        public int antal;
        public DateTime transaktionsdato;
        public int hour;
        public int edbNummer;
        public int klientNummer;
        public int koncern;
        public int virtueltNummerGL;
        public int datatype;
        public int transaktionstype;
        public int histLagerAntal;
        public double histLagerBeløb;
        public int histSalgAntal;
        public double histSalgBeløb;
        public double histRabatBeløb;
        public double histMomsBeløb;
        public double histVareforbrug;
        public double histKøbInternBeløb;
        public double histKøbExternBeløb;
        public int histKøbInternAntal;
        public int histKøbExternAntal;
        public double histKorrektionStatusBeløb;
        public double histKorrektionManualBeløb;
        public int histKorrektionStatusAntal;
        public int histKorrektionManualAntal;
        public int maskinNr;
        public int bonNr;
        public int varegruppe;
        public long cardNumber;
        public string shipmentnumber;
        public string koncernName;

        public string uniqueBonNr; // concatination of transaktionsdato + maskineNr + bonNr

        public int discountReason;
        public int returnReason;
        public string transactionNumber;
        public bool isPrimoETL = false;
    }
}
