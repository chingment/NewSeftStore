using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XrtPaySdk
{
    [XmlRoot("xml")]
    public class WxJsPayRequestResult: BaseRequestResult
    {
        public string pay_info { get; set; }
    }
}
