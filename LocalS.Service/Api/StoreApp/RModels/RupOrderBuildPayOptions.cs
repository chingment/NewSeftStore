using LocalS.BLL.Biz;
using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RupOrderBuildPayOptions
    {
        public E_AppCaller AppCaller { get; set; }
        public string AppId { get; set; }
        //todo 删除MerchId
        public string MerchId { get; set; }
    }
}
