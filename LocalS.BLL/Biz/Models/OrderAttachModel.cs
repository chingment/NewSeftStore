using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public enum Caller
    {
        Unknow = 0,
        WxPa = 1,
        WxMp = 2
    }

    public class OrderAttachModel
    {
        public string MerchId { get; set; }

        public string StoreId { get; set; }

        public Caller Caller { get; set; }
    }
}
