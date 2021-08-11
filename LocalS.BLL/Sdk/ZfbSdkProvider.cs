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
    public class ZfbSdkProvider : BaseService, IPaySdkProvider<ZfbAppInfoConfig>
    {
        public PayBuildQrCodeResult PayBuildQrCode(ZfbAppInfoConfig config, E_PayCaller payCaller, string merch_id, string store_id, string device_id, string order_sn, decimal order_amount, string goods_tag, string create_ip, string body, DateTime? time_expire = null)
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

        public string PayTransQuery(ZfbAppInfoConfig config, string orderId)
        {
            ZfbUtil zfbUtil = new ZfbUtil(config);


            return zfbUtil.OrderQuery(orderId);
        }


        public PayTransResult Convert2PayTransResultByQuery(ZfbAppInfoConfig config, string content)
        {
            var result = new PayTransResult();

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
                            result.PayTransId = payResult.out_trade_no;
                        }

                        if (!string.IsNullOrEmpty(payResult.trade_no))
                        {
                            result.PayPartnerPayTransId = payResult.trade_no;
                        }

                        if (!string.IsNullOrEmpty(payResult.buyer_logon_id))
                        {
                            result.ClientUserName = payResult.buyer_logon_id;
                        }

                        LogUtil.Info("解释支付宝支付协议，订单号：" + result.PayTransId);

                        if (payResult.trade_status == "TRADE_SUCCESS")
                        {
                            result.IsPaySuccess = true;
                            result.PayWay = Entity.E_PayWay.Zfb;
                        }
                    }
                }
            }

            return result;
        }

        public PayTransResult Convert2PayTransResultByNotifyUrl(ZfbAppInfoConfig config, string content)
        {
            var result = new PayTransResult();

            var dic = MyAlipaySdk.CommonUtil.FormStringToDictionary(content);

            if (dic.ContainsKey("out_trade_no"))
            {
                result.PayTransId = dic["out_trade_no"].ToString();
            }

            if (dic.ContainsKey("trade_no"))
            {
                result.PayPartnerPayTransId = dic["trade_no"].ToString();
            }

            LogUtil.Info("解释支付宝支付协议，订单号：" + result.PayTransId);


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
                    result.PayWay = Entity.E_PayWay.Zfb;
                }
            }

            return result;
        }

        public PayRefundResult PayRefund(ZfbAppInfoConfig config, string payTranId, string payRefundId, decimal total_fee, decimal refund_fee, string refund_desc)
        {
            var result = new PayRefundResult();

            ZfbUtil zfbUtil = new ZfbUtil(config);

            var respone = zfbUtil.PayRefund(payTranId, refund_fee, refund_desc);

            if (respone.Code == "10000")
            {
                if (respone.FundChange == "Y")
                {
                    result.Status = "SUCCESS";
                    result.RefundTime = DateTime.Parse(respone.GmtRefundPay);
                    result.RefundFee = decimal.Parse(respone.RefundFee);
                }
            }
            else
            {
                result.Status = "FAIL";
            }


            return result;
        }

        public string PayRefundQuery(ZfbAppInfoConfig config, string payTranId, string payRefundId)
        {
            return null;
        }

    }
}
