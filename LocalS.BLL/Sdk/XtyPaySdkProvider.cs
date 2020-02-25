﻿using LocalS.BLL.Biz;
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

        public PayBuildWxJsPayInfoResult PayBuildWxJsPayInfo(XrtPayInfoConfg config, string merch_id, string store_id, string app_id, string open_id, string machine_id, string order_sn, decimal order_amount, string goods_tag, string create_ip, string body, DateTime? time_expire = null)
        {
            var result = new PayBuildWxJsPayInfoResult();

            XrtPayUtil xrtPayUtil = new XrtPayUtil(config);

            string totelFee = Convert.ToInt32(order_amount * 100).ToString();

            var xrtWxPayBuildByJsResult = xrtPayUtil.WxPayBuildByJs(app_id, open_id, order_sn, totelFee, body, "", create_ip, "", time_expire.Value.ToString("yyyyMMddHHmmss"));

            if (xrtWxPayBuildByJsResult.status == "0" && xrtWxPayBuildByJsResult.result_code == "0")
            {
                if (!string.IsNullOrEmpty(xrtWxPayBuildByJsResult.pay_info))
                {

                    Dictionary<string, string> dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(xrtWxPayBuildByJsResult.pay_info);

                    result.AppId = dic["appId"];
                    result.NonceStr = dic["nonceStr"];
                    result.Timestamp = dic["timeStamp"];
                    result.Package = dic["package"];
                    result.SignType = dic["signType"];
                    result.PaySign = dic["paySign"];
                }


            }

            return result;
        }

        public PayBuildQrCodeResult PayBuildQrCode(XrtPayInfoConfg config, E_OrderPayCaller payCaller, string merch_id, string store_id, string machine_id, string order_sn, decimal order_amount, string goods_tag, string create_ip, string body, DateTime? time_expire = null)
        {
            var result = new PayBuildQrCodeResult();

            XrtPayUtil xrtPayUtil = new XrtPayUtil(config);

            string totelFee = Convert.ToInt32(order_amount * 100).ToString();

            if (payCaller == E_OrderPayCaller.WxByNt)
            {
                var wxPayBuildByNt = xrtPayUtil.WxPayBuildByNt(order_sn, totelFee, body, "", create_ip, "", time_expire.Value.ToString("yyyyMMddHHmmss"));

                if (wxPayBuildByNt.status == "0" && wxPayBuildByNt.result_code == "0")
                    result.CodeUrl = wxPayBuildByNt.code_url;
            }
            else if (payCaller == E_OrderPayCaller.AliByNt)
            {
                var aliPayBuildByNt = xrtPayUtil.AliPayBuildByNt(order_sn, totelFee, body, "", create_ip, "", time_expire.Value.ToString("yyyyMMddHHmmss"));

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
