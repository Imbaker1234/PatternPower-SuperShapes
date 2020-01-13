using System;
using System.Xml;

namespace Prototype
{
    [Serializable]
    public class Baz
    {
        public DateTime DateTime { get; set; }
        public int IntVal { get; set; }
        public double DoubleVal { get; set; }
        public string Name { get; set; }
        public Action DoIt { get; set; }

        public override string ToString()
        {
            return "Baz.ToString()";
        }

        public Baz(DateTime dateTime, XmlNode xml, int intVal, int doubleVal, Action doIt)
        {
            DateTime = dateTime;
            IntVal = intVal;
            DoubleVal = doubleVal;
            DoIt = doIt;
        }

        public Baz()
        {
            DoIt = () => IntVal += 5;
            DateTime = DateTime.Now.AddDays(-3);
            IntVal = 21;
            DoubleVal = 32.44478;
        }
    }
}