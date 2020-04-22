using MyAlipaySdk;
using LocalS.BLL.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumos;
using LocalS.Entity;

namespace LocalS.BLL
{
    public class ZfbSdkProvider : BaseDbContext, IPaySdkProvider<ZfbAppInfoConfig>
    {
        public PayBuildQrCodeResult PayBuildQrCode(ZfbAppInfoConfig config, E_OrderPayCaller payCaller, string merch_id, string store_id, string machine_id, string order_sn, decimal order_amount, string goods_tag, string create_ip, string body, DateTime? time_expire = null)
        {

            var result = new PayBuildQrCodeResult();

            ZfbUtil zfbUtil = new ZfbUtil(config);

            UnifiedOrder unifiedOrder = new UnifiedOrder();
            unifiedOrder.store_id = store_id;
            unifiedOrder.out_trade_no = order_sn;//商户订单号
            unifiedOrder.total_amount = order_amount.ToF2Price();
            unifiedOrder.subject = body;//商品描述  
            unifiedOrder.timeout_express = "2m";
            //unifiedOrder.extend_params = attach.ToJsonString();

            var ret = zfbUtil.UnifiedOrder(unifiedOrder);
            if (ret != null)
            {
                result.CodeUrl = ret.CodeUrl;
            }



            return result;

        }

        public string PayQuery(ZfbAppInfoConfig config, string orderId)
        {
            ZfbUtil zfbUtil = new ZfbUtil(config);


            return zfbUtil.OrderQuery(orderId);
        }


        public PayResult Convert2PayResultByPayQuery(ZfbAppInfoConfig config, string content)
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
                            result.OrderId = payResult.out_trade_no;
                        }

                        if (!string.IsNullOrEmpty(payResult.trade_no))
                        {
                            result.PayPartnerOrderId = payResult.trade_no;
                        }

                        if (!string.IsNullOrEmpty(payResult.buyer_logon_id))
                        {
                            result.ClientUserName = payResult.buyer_logon_id;
                        }

                        LogUtil.Info("解释支付宝支付协议，订单号：" + result.OrderId);

                        if (payResult.trade_status == "TRADE_SUCCESS")
                        {
                            result.IsPaySuccess = true;
                            result.OrderPayWay = Entity.E_OrderPayWay.Zfb;
                        }
                    }
                }
            }

            return result;
        }

        public PayResult Convert2PayResultByNotifyUrl(ZfbAppInfoConfig config, string content)
        {
            var result = new PayResult();

            var dic = MyAlipaySdk.CommonUtil.FormStringToDictionary(content);

            if (dic.ContainsKey("out_trade_no"))
            {
                result.OrderId = dic["out_trade_no"].ToString();
            }

            if (dic.ContainsKey("trade_no"))
            {
                result.PayPartnerOrderId = dic["trade_no"].ToString();
            }

            LogUtil.Info("解释支付宝支付协议，订单号：" + result.OrderId);


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
                    result.OrderPayWay = Entity.E_OrderPayWay.Zfb;
                }
            }

            return result;
        }

    }
}
