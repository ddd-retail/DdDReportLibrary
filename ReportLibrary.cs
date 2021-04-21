using System;
using System.Collections.Generic;
using i18n;

namespace ReportLibrary
{
    // function templates
    public delegate void VoidFunction();
    public delegate void VoidFunction<A>(A a);
    public delegate void VoidFunction<A, B>(A a, B b);
    public delegate void VoidFunction<A, B, C>(A a, B b, C c);
    public delegate void VoidFunction<A, B, C, D>(A a, B b, C c, D d);

    public delegate R Function<R>();
    public delegate R Function<R, A>(A a);
    public delegate R Function<R, A, B>(A a, B b);
    public delegate R Function<R, A, B, C>(A a, B b, C c);
    public delegate R Function<R, A, B, C, D>(A a, B b, C c, D d);

    public class Timing : IDisposable
    {
        DateTime start;
        string name;

        public Timing(string name)
        {
            start = System.DateTime.Now;
            this.name = name;
        }

        public void Stop()
        {
            System.TimeSpan span = System.DateTime.Now - start;
            System.Diagnostics.Debug.WriteLine(String.Format("{0} took {1} ms", name, span.Seconds * 1000 + span.Milliseconds));
        }

        public void Dispose()
        {
            Stop();
        }
    }

    public class Field
    {
        public string id;
        public string dbName;
        public string group;

        public List<string> enabledBy = new List<string>();
        public List<string> disables = new List<string>();

        public I18nString nameToShow;

        virtual public string name
        {
            get
            {
                if (nameToShow != null)
                    return nameToShow.translate();
                else
                    return Helpers.ETLHelpers.LastName(dbName);
            }
        }
    }

    public class I18nString
    {
        private string str;

        public I18nString(string s)
        {
            str = s;
        }

        public string translate()
        {
            return I18n.GetString(str);
        }

        public static implicit operator string(I18nString i18nstr)
        {
            //MGA UGLY FIX
            try
            {
                return i18nstr.translate();
            }
            catch (Exception)
            {
                if (i18nstr != null)
                    return i18nstr.str;
                else
                    return "unknown string";
            }
        }
    }

    public class DimensionRestrictionResult
    {
        public List<string> values, labels;
    }

    public class Dimension : Field
    {
        // FIXME: whack this
        public enum Size { small, large };
        public Size size = Size.small;

        public List<Dimension> comesBefore = new List<Dimension>();

        public Func<List<KeyFigure>, Dictionary<Dimension, string[]>, bool, bool, bool, DimensionRestrictionResult> restrictionValues = null;
        public Func<object, string> valueFormat = null;
        public Func<object, string> labelFormat = null;
        public Func<object, int, string> koncernLabelFormat = null;
       
    }

    public enum KeyFigureType
    {
        Money, Count, Percentage, CountDecimals
    }

    public enum KeyFigureGroupId
    {
        Sales, GrossProfit, Index, Buy, Inventory, Budget
    };

    public class KeyFigure : Field
    {
        public I18nString description;
        public KeyFigureType type; // used to determine what we can plot together
        public KeyFigureGroupId kfGroup;
        #region index
        public string nonIndexKeyFigure;
        public string nonIndexOneYearKeyFigure;
        public bool visible;
        public bool positive; // positive means that the higher the better
        #endregion
    }
}
