using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XrtPaySdk
{
    [XmlRoot("xml")]
    public class WxNativePayRequestResult : BaseRequestResult
    {
        public string code_url { get; set; }
        public string code_img_url { get; set; }
    }
}
