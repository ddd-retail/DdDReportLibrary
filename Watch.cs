using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace ReportLibrary
{
    public class Watch : XPObject
    {
        public Watch(Session session)
            : base(session)
        {
        }

        string keyfigure;

        [Size(SizeAttribute.Unlimited)]
        public string Keyfigure
        {
            get { return keyfigure; }
            set { SetPropertyValue("Keyfigure", ref keyfigure, value); }
        }

        string keyfigureName;

        [Size(SizeAttribute.Unlimited)]
        public string KeyfigureName
        {
            get { return keyfigureName; }
            set { SetPropertyValue("KeyfigureName", ref keyfigureName, value); }
        }

        KeyFigureType keyfigureType;

        public KeyFigureType KeyfigureType
        {
            get { return keyfigureType; }
            set { SetPropertyValue("KeyfigureType", ref keyfigureType, value); }
        }

        string dimension;

        [Size(SizeAttribute.Unlimited)]
        public string Dimension
        {
            get { return dimension; }
            set { SetPropertyValue("Dimension", ref dimension, value); }
        }

        string dimensionName;

        [Size(SizeAttribute.Unlimited)]
        public string DimensionName
        {
            get { return dimensionName; }
            set { SetPropertyValue("DimensionName", ref dimensionName, value); }
        }

        string restrictions;

        [Size(SizeAttribute.Unlimited)]
        public string Restrictions
        {
            get { return restrictions; }
            set { SetPropertyValue("Restrictions", ref restrictions, value); }
        }

        public enum valuetype { bigger, smaller };
        valuetype vt;

        public valuetype ValueType
        {
            get { return vt; }
            set { SetPropertyValue("ValueType", ref vt, value); }
        }

        double val;

        public double Value
        {
            get { return val; }
            set { SetPropertyValue("Value", ref val, value); }
        }

        int userid;

        public int UserID
        {
            get { return userid; }
            set { SetPropertyValue("UserID", ref userid, value); }
        }

        public enum valueStatus { bigger, smaller };

        valueStatus lastValueStatus;

        public valueStatus LastValueStatus
        {
            get { return lastValueStatus; }
            set { SetPropertyValue("LastValueStatus", ref lastValueStatus, value); }
        }
    }
}
