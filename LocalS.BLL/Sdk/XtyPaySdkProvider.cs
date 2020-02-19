using LocalS.BLL.Biz;
using LocalS.Entity;
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

        public void PayBuildMpPayParms(XrtPayInfoConfg config, string openId, string orderSn, decimal orderAmount, string goods_tag, string ip, string body, OrderAttachModel attach, DateTime? time_expire = null)
        {

        }

        public PayBuildQrCodeResult PayBuildQrCode(XrtPayInfoConfg config, E_OrderPayCaller payCaller, string merch_id, string store_id, string machine_id, string order_sn, decimal order_amount, string goods_tag, string create_ip, string body, DateTime time_expire)
        {
            var result = new PayBuildQrCodeResult();

            XrtPayUtil xrtPayUtil = new XrtPayUtil(config);

            string totelFee = Convert.ToInt32(order_amount * 100).ToString();

            if (payCaller == E_OrderPayCaller.WxByNt)
            {
                var wxPayBuildByNt = xrtPayUtil.WxPayBuildByNt(order_sn, totelFee, body, "", create_ip, "", time_expire.ToString("yyyyMMddHHmmss"));

                if (wxPayBuildByNt.status == "0" && wxPayBuildByNt.result_code == "0")
                    result.CodeUrl = wxPayBuildByNt.code_url;
            }
            else if(payCaller== E_OrderPayCaller.AliByNt)
            {
                var aliPayBuildByNt = xrtPayUtil.AliPayBuildByNt(order_sn, totelFee, body, "", create_ip, "", time_expire.ToString("yyyyMMddHHmmss"));

                if (aliPayBuildByNt.status == "0" && aliPayBuildByNt.result_code == "0")
                    result.CodeUrl = aliPayBuildByNt.code_url;
            }

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
