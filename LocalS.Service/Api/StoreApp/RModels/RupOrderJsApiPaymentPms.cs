using LocalS.BLL.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public enum PayWay
    {
        Unknow = 0,
        Wechat = 1,
        AliPay = 2
    }

    public class RupOrderJsApiPaymentPms
    {
        public string MerchId { get; set; }
        public string AppId { get; set; }
        public string OrderId { get; set; }
        public PayWay PayWay { get; set; }
        public Caller Caller { get; set; }

    }
}
