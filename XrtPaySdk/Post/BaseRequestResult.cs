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

        public string result_code { get; set; }
        public string mch_id { get; set; }
        public string device_info { get; set; }
        public string nonce_str { get; set; }
        public string err_code { get; set; }
        public string err_msg { get; set; }
        public string sign { get; set; }
    }
}
