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

        public PayUrlNotifyResult ConvertPayUrlNotifyResult(TgPayInfoConfg config, string content)
        {
            var result = new PayUrlNotifyResult();

            var obj_content = Newtonsoft.Json.JsonConvert.DeserializeObject<AllQrcodePayAsynNotifyResult>(content);
            if (obj_content != null)
            {
                if (obj_content.state == "0")
                {
                    result.IsPaySuccess = true;
                    result.OrderSn = obj_content.lowOrderId;
                    result.PayPartnerOrderSn = obj_content.upOrderId;
                    if (obj_content.channelID == "WX")
                    {
                        result.OrderPayWay = E_OrderPayWay.Wechat;
                    }
                    if (obj_content.channelID == "ZFB")
                    {
                        result.OrderPayWay = E_OrderPayWay.AliPay;
                    }

                }
            }

            return result;
        }

        public PayQueryResult ConvertPayQueryResult(TgPayInfoConfg config, string content)
        {
            var result = new PayQueryResult();

            var obt_content = Newtonsoft.Json.JsonConvert.DeserializeObject<OrderQueryRequestResult>(content);
            if (result != null)
            {
                if (obt_content.status == "100")
                {
                    if (obt_content.state == "0")
                    {
                        result.IsPaySuccess = true;
                        result.OrderSn = obt_content.lowOrderId;
                        result.PayPartnerOrderSn = obt_content.upOrderId;
                        if (obt_content.channelID == "WX")
                        {
                            result.OrderPayWay = E_OrderPayWay.Wechat;
                        }
                        if (obt_content.channelID == "ZFB")
                        {
                            result.OrderPayWay = E_OrderPayWay.AliPay;
                        }

                    }
                }
            }

            return result;
        }
    }
}
