using LocalS.BLL.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgPaySdk;

namespace LocalS.BLL
{
    public class TgPaySdkProvider
    {
        public AllQrcodePayRequestResult AllQrcodePay(TgPayInfoConfg config, string merchId, string storeId, string orderSn, decimal orderAmount, string goods_tag, string ip, string body, OrderAttachModel attach, DateTime time_expire)
        {
            TgPayUtil tgUtil = new TgPayUtil(config);
            var ret = tgUtil.AllQrcodePay(orderSn, orderAmount.ToString("#0.00"), body, storeId);
            return ret;
        }

        public string OrderQuery(TgPayInfoConfg config, string orderSn)
        {
            TgPayUtil tgUtil = new TgPayUtil(config);
            string ret = tgUtil.OrderQuery(orderSn);
            return ret;
        }
    }
}
