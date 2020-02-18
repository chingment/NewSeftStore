using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XrtPaySdk
{
    [XmlRoot("xml")]
    public class BaseRequestResult
    {
        public string version { get; set; }
        public string charset { get; set; }
        public string sign_type { get; set; }
        public string status { get; set; }
        public string message { get; set; }
    }
}
