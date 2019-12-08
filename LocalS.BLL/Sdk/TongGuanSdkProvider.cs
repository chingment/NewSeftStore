using LocalS.BLL.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TongGuanPaySdk;

namespace LocalS.BLL
{
    public class TongGuanSdkProvider
    {
        public AllQrcodePayRequestResult AllQrcodePay(TongGuanPayInfoConfg config, string merchId, string storeId, string orderSn, decimal orderAmount, string goods_tag, string ip, string body, OrderAttachModel attach, DateTime time_expire)
        {
            TongGuanUtil tongGuanUtil = new TongGuanUtil(config);
            var ret = tongGuanUtil.AllQrcodePay(orderSn, orderAmount.ToString("#0.00"), "", storeId);
            return ret;
        }

        public string OrderQuery(TongGuanPayInfoConfg config, string orderSn)
        {
            TongGuanUtil tongGuanUtil = new TongGuanUtil(config);
            string ret = tongGuanUtil.OrderQuery(orderSn);
            return ret;
        }
    }
}
