using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasemobSdk
{
    public class ErrorResult
    {
        public string error { get; set; }
        public string exception { get; set; }
        public string timestamp { get; set; }
        public string duration { get; set; }
        public string error_description { get; set; }
    }
}
