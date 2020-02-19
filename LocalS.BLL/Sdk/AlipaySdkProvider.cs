using MyAlipaySdk;
using LocalS.BLL.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumos;

namespace LocalS.BLL
{
    public class AliPaySdkProvider : BaseDbContext
    {
        public UnifiedOrderResult UnifiedOrderByNative(AlipayAppInfoConfig config, string merchId,string storeId, string orderSn, decimal orderAmount, string goods_tag, string ip, string body, OrderAttachModel attach, DateTime time_expire)
        {

            var ret = new UnifiedOrderResult();

            AlipayUtil alipayUtil = new AlipayUtil(config);

            UnifiedOrder unifiedOrder = new UnifiedOrder();
            unifiedOrder.store_id = storeId;
            unifiedOrder.out_trade_no = orderSn;//商户订单号
            unifiedOrder.total_amount = orderAmount.ToF2Price();
            unifiedOrder.subject = body;//商品描述  
            unifiedOrder.timeout_express = "2m";
            //unifiedOrder.extend_params = attach.ToJsonString();

            ret = alipayUtil.UnifiedOrder(unifiedOrder);




            return ret;

        }

        public string OrderQuery(AlipayAppInfoConfig config, string orderSn)
        {
            AlipayUtil alipayUtil = new AlipayUtil(config);


            return alipayUtil.OrderQuery(orderSn);
        }


        public PayResult Convert2PayResultByPayQuery(AlipayAppInfoConfig config, string content)
        {
            var result = new PayResult();

            var obj_content = Newtonsoft.Json.JsonConvert.DeserializeObject<TradeQueryResult>(content);
            if (obj_content != null)
            {

                var payResult = obj_content.alipay_trade_query_response;
                if (payResult != null)
                {
                    if (payResult.code == "10000")
                    {
                        if (!string.IsNullOrEmpty(payResult.out_trade_no))
                        {
                            result.OrderSn = payResult.out_trade_no;
                        }

                        if (!string.IsNullOrEmpty(payResult.trade_no))
                        {
                            result.PayPartnerOrderSn = payResult.trade_no;
                        }

                        if (!string.IsNullOrEmpty(payResult.buyer_logon_id))
                        {
                            result.ClientUserName = payResult.buyer_logon_id;
                        }

                        LogUtil.Info("解释支付宝支付协议，订单号：" + result.OrderSn);

                        if (payResult.trade_status == "TRADE_SUCCESS")
                        {
                            result.IsPaySuccess = true;
                            result.OrderPayWay = Entity.E_OrderPayWay.AliPay;
                        }
                    }
                }
            }

            return result;
        }

        public PayResult Convert2PayResultByNotifyUrl(AlipayAppInfoConfig config, string content)
        {
            var result = new PayResult();

            var dic = MyAlipaySdk.CommonUtil.FormStringToDictionary(content);

            if (dic.ContainsKey("out_trade_no"))
            {
                result.OrderSn = dic["out_trade_no"].ToString();
            }

            if (dic.ContainsKey("trade_no"))
            {
                result.PayPartnerOrderSn = dic["trade_no"].ToString();
            }

            LogUtil.Info("解释支付宝支付协议，订单号：" + result.OrderSn);


            if (dic.ContainsKey("buyer_logon_id"))
            {
                result.ClientUserName = dic["buyer_logon_id"];
            }


            if (dic.ContainsKey("trade_status"))
            {
                string trade_status = dic["trade_status"].ToString();
                LogUtil.Info("解释支付宝支付协议，（trade_status）订单状态：" + trade_status);
                if (trade_status == "TRADE_SUCCESS")
                {
                    result.IsPaySuccess = true;
                    result.OrderPayWay = Entity.E_OrderPayWay.AliPay;
                }
            }

            return result;
        }

    }
}
