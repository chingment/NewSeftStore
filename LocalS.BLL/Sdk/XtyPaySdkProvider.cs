using LocalS.BLL.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrtPaySdk;

namespace LocalS.BLL
{
    public class XtyPaySdkProvider : IPaySdkProvider
    {

        public WxPayBuidResultByNt WxPayBuildByNt(object config, string merch_id, string store_id, string machine_id, string order_sn, decimal order_amount, string goods_tag, string create_ip, string body, OrderAttachModel attach, DateTime time_expire)
        {
            //XrtPayUtil xrtPayUtil = new XrtPayUtil(config);
            //var ret = xrtPayUtil.WxPayByNt(orderSn, orderAmount.ToString("#0.00"), body, "", ip, "", time_expire.ToString("yyyyMMddHHmmss"), storeId);
            //return ret;

            return null;
        }
    }
}
