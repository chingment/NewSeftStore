using LocalS.BLL.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrtPaySdk;

namespace LocalS.BLL
{
    public class XtyPaySdkProvider : IPaySdkProvider<XrtPayInfoConfg>
    {

        public WxPayBuildByNtResult WxPayBuildByNt(XrtPayInfoConfg config, string merch_id, string store_id, string machine_id, string order_sn, decimal order_amount, string goods_tag, string create_ip, string body, DateTime time_expire)
        {
            var result = new WxPayBuildByNtResult();

            XrtPayUtil xrtPayUtil = new XrtPayUtil(config);

            string totelFee = Convert.ToInt32(order_amount * 100).ToString();

            var wxPayBuildByNt = xrtPayUtil.WxPayBuildByNt(order_sn, totelFee, body, "", create_ip, "", time_expire.ToString("yyyyMMddHHmmss"));

            if (wxPayBuildByNt.status == "0" && wxPayBuildByNt.result_code == "0")
                result.CodeUrl = wxPayBuildByNt.code_url;

            return result;

        }

        public AliPayBuildByNtResult AliPayBuildByNt(XrtPayInfoConfg config, string merch_id, string store_id, string machine_id, string order_sn, decimal order_amount, string goods_tag, string create_ip, string body, DateTime time_expire)
        {
            var result = new AliPayBuildByNtResult();

            XrtPayUtil xrtPayUtil = new XrtPayUtil(config);

            string totelFee = Convert.ToInt32(order_amount * 100).ToString();

            var wxPayBuildByNt = xrtPayUtil.AliPayBuildByNt(order_sn, totelFee, body, "", create_ip, "", time_expire.ToString("yyyyMMddHHmmss"));

            if (wxPayBuildByNt.status == "0" && wxPayBuildByNt.result_code == "0")
                result.CodeUrl = wxPayBuildByNt.code_url;

            return result;

        }

        public PayQueryResult PayQuery(XrtPayInfoConfg config, string order_sn)
        {
            var result = new PayResult();

            XrtPayUtil xrtPayUtil = new XrtPayUtil(config);


            var xrtPayQueryResult = xrtPayUtil.PayQuery(order_sn);

            if (xrtPayQueryResult.status == "0" && xrtPayQueryResult.result_code == "0")
            {
                if (xrtPayQueryResult.trade_state == "SUCCESS")
                {
                    result.IsPaySuccess = true;
                }

            }

            return result;
        }

        public PayUrlNotifyResult ConvertPayUrlNotifyResult(XrtPayInfoConfg config, string content)
        {
            var result = new PayUrlNotifyResult();

            return result;
        }
    }
}
