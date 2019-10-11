using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWeiXinSdk
{
    public class WxApiMediaUploadNewsResult : WxApiBaseResult
    {
        public string type { get; set; }
        public string media_id { get; set; }
        public long created_at { get; set; }
    }
}
