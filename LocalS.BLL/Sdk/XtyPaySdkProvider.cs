using LocalS.BLL.Biz;
using Lumos;
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

        public string PayQuery(XrtPayInfoConfg config, string order_sn)
        {
            var result = new PayResult();

            XrtPayUtil xrtPayUtil = new XrtPayUtil(config);


            var xrtPayQueryResult = xrtPayUtil.PayQuery(order_sn);

            return xrtPayQueryResult;
        }

        public PayResult Convert2PayResultByPayQuery(XrtPayInfoConfg config, string content)
        {
            var result = new PayResult();

            var obj_content = XmlUtil.DeserializeToObject<OrderPayQueryRequestResult>(content);
            if (obj_content.status == "0" && obj_content.result_code == "0")
            {
                if (obj_content.trade_state == "SUCCESS")
                {
                    result.IsPaySuccess = true;
                    result.OrderSn = obj_content.out_trade_no;
                    result.PayPartnerOrderSn = obj_content.transaction_id;
                }

            }


            return result;
        }

        public PayResult Convert2PayResultByNotifyUrl(XrtPayInfoConfg config, string content)
        {
            var result = new PayResult();

            return result;
        }
    }
}
