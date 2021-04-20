using System;

namespace ReportLibrary.Models.Oracle
{
    public class MainGroupRow : IEquatable<MainGroupRow>
    {
        public MainGroupRow()
        {
            
        }

        public int TypeId;
        public string TypeName;
        public int HovedgruppeId;
        public string HovedgruppeName;
        public int VaregruppeId;
        public int ConcernId;

        public static MainGroupRow DeepCopy(MainGroupRow row)
        {
            return new MainGroupRow
            {
                TypeId = row.TypeId,
                TypeName = row.TypeName,
                HovedgruppeId = row.HovedgruppeId,
                HovedgruppeName = row.HovedgruppeName,
                ConcernId = row.ConcernId
            };
        }

        public bool Equals(MainGroupRow other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TypeId == other.TypeId && HovedgruppeId == other.HovedgruppeId && ConcernId == other.ConcernId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MainGroupRow) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = TypeId;
                hashCode = (hashCode * 397) ^ HovedgruppeId;
                hashCode = (hashCode * 397) ^ ConcernId;
                return hashCode;
            }
        }
    }
}