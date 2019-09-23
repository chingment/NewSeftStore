using LocalS.BLL.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RupOrderJsApiPaymentPms
    {
        public string MerchId { get; set; }
        public string AppId { get; set; }
        public string OrderId { get; set; }
        /// <summary>
        /// 1: Wechat,  2 AliPay
        /// </summary>
        public int PayWay { get; set; }
        public int Caller { get; set; }

    }
}
