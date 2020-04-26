using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System
{

    public class EventCodeRemarkAttribute : Attribute
    {
        private string _level;
        private string _name;
        public EventCodeRemarkAttribute(string level, string name)
        {
            this._level = level;
            this._name = name;
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Level
        {
            get { return _level; }
            set { _level = value; }
        }
    }

}
