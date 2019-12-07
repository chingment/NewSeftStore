using LocalS.BLL.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TongGuanPaySdk;

namespace LocalS.BLL
{
    public class TongGuanSdkProvider
    {
        public AllQrcodePayRequestResult AllQrcodePay(TongGuanPayInfoConfg config, string merchId, string storeId, string orderSn, decimal orderAmount, string goods_tag, string ip, string body, OrderAttachModel attach, DateTime time_expire)
        {

            var ret = new AllQrcodePayRequestResult();

            TongGuanUtil tongGuanUtil = new TongGuanUtil(config);

            //UnifiedOrder unifiedOrder = new UnifiedOrder();
            //unifiedOrder.store_id = storeId;
            //unifiedOrder.out_trade_no = orderSn;//商户订单号
            //unifiedOrder.total_amount = orderAmount.ToF2Price();
            //unifiedOrder.subject = body;//商品描述  
            //unifiedOrder.timeout_express = "2m";
            ////unifiedOrder.extend_params = attach.ToJsonString();

            //ret = alipayUtil.UnifiedOrder(unifiedOrder);




            return ret;

        }

        public string OrderQuery(TongGuanPayInfoConfg config, string orderSn)
        {
            TongGuanUtil tongGuanUtil = new TongGuanUtil(config);
            string xml = tongGuanUtil.OrderQuery(orderSn);

            return xml;
        }
    }
}
