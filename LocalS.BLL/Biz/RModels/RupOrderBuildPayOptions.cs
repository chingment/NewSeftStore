using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class RupOrderBuildPayOptions
    {
        public E_AppCaller AppCaller { get; set; }
        public string AppId { get; set; }
        public string MerchId { get; set; }
        public string ClientUserId { get; set; }
    }
}
