using LocalS.BLL.Biz;
using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgPaySdk;

namespace LocalS.BLL
{
    public class TgPaySdkProvider : IPaySdkProvider<TgPayInfoConfg>
    {

        public PayBuildQrCodeResult PayBuildQrCode(TgPayInfoConfg config, E_PayCaller payCaller, string merch_id, string store_id, string machine_id, string order_sn, decimal order_amount, string goods_tag, string create_ip, string body, DateTime? time_expire=null)
        {
            var result = new PayBuildQrCodeResult();

    
            TgPayUtil tgUtil = new TgPayUtil(config);

            if (payCaller == E_PayCaller.AggregatePayByNt)
            {
                var ret = tgUtil.AllQrcodePay(order_sn, order_amount.ToString("#0.00"), body, store_id);

                if (ret != null)
                {
                    result.CodeUrl = ret.codeUrl;
                }
            }


            return result;
        }

        public string PayQuery(TgPayInfoConfg config, string orderId)
        {
            TgPayUtil tgUtil = new TgPayUtil(config);
            string ret = tgUtil.OrderQuery(orderId);
            return ret;
        }

        public PayResult Convert2PayResultByPayQuery(TgPayInfoConfg config, string content)
        {
            var result = new PayResult();

            var obt_content = Newtonsoft.Json.JsonConvert.DeserializeObject<OrderQueryRequestResult>(content);
            if (result != null)
            {
                if (obt_content.status == "100")
                {
                    if (obt_content.state == "0")
                    {
                        result.IsPaySuccess = true;
                        result.PayTransId = obt_content.lowOrderId;
                        result.PayPartnerOrderId = obt_content.upOrderId;
                        if (obt_content.channelID == "WX")
                        {
                            result.PayWay = E_PayWay.Wx;
                        }
                        if (obt_content.channelID == "ZFB")
                        {
                            result.PayWay = E_PayWay.Zfb;
                        }

                    }
                }
            }

            return result;
        }

        public PayResult Convert2PayResultByNotifyUrl(TgPayInfoConfg config, string content)
        {
            var result = new PayResult();

            var obj_content = Newtonsoft.Json.JsonConvert.DeserializeObject<AllQrcodePayAsynNotifyResult>(content);
            if (obj_content != null)
            {
                if (obj_content.state == "0")
                {
                    result.IsPaySuccess = true;
                    result.PayTransId = obj_content.lowOrderId;
                    result.PayPartnerOrderId = obj_content.upOrderId;
                    if (obj_content.channelID == "WX")
                    {
                        result.PayWay = E_PayWay.Wx;
                    }
                    if (obj_content.channelID == "ZFB")
                    {
                        result.PayWay = E_PayWay.Zfb;
                    }

                }
            }

            return result;
        }
    }
}
