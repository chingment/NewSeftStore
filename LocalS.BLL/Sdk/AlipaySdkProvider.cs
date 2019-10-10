using AlipaySdk;
using LocalS.BLL.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class AlipaySdkProvider : BaseDbContext
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
    }
}
