using LocalS.BLL.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TongGuanPaySdk;

namespace LocalS.BLL
{
    public class TgPaySdkProvider
    {
        public AllQrcodePayRequestResult AllQrcodePay(TgPayInfoConfg config, string merchId, string storeId, string orderSn, decimal orderAmount, string goods_tag, string ip, string body, OrderAttachModel attach, DateTime time_expire)
        {
            TgPayUtil tongGuanUtil = new TgPayUtil(config);
            var ret = tongGuanUtil.AllQrcodePay(orderSn, orderAmount.ToString("#0.00"), body, storeId);
            return ret;
        }

        public string OrderQuery(TgPayInfoConfg config, string orderSn)
        {
            TgPayUtil tongGuanUtil = new TgPayUtil(config);
            string ret = tongGuanUtil.OrderQuery(orderSn);
            return ret;
        }
    }
}
