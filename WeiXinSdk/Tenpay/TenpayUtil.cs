using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWeiXinSdk.Tenpay
{
    public class TenpayUtil
    {
        private TenpayRequest _request = new TenpayRequest();
        private WxAppInfoConfig _config = null;

        public WxAppInfoConfig Config
        {
            get
            {
                return _config;
            }
        }

        public TenpayUtil(WxAppInfoConfig config)
        {
            this._config = config;
        }

        public UnifiedOrderResult UnifiedOrder(UnifiedOrder order)
        {
            var rt = new UnifiedOrderResult();

            TenpayUnifiedOrderApi api = new TenpayUnifiedOrderApi(_config, order);

            var result = _request.DoPost(_config, api);
            string prepayId = null;
            string code_url = null;
            result.TryGetValue("prepay_id", out prepayId);
            result.TryGetValue("code_url", out code_url);
            rt.PrepayId = prepayId;
            rt.CodeUrl = code_url;
            return rt;
        }

        public string OrderQuery(string out_trade_no)
        {
            TenpayOrderQueryApi api = new TenpayOrderQueryApi(_config, out_trade_no);
            var result = _request.DoPost(_config, api);


            return _request.ReturnContent;
        }

        public OrderPayRefundResult OrderPayRefund(string out_trade_no, string out_refund_no, string total_fee, string refund_fee, string refund_desc)
        {
            var ret = new OrderPayRefundResult();

            TenpayOrderPayReFundApi api = new TenpayOrderPayReFundApi(_config, out_trade_no, out_refund_no, total_fee, refund_fee, refund_desc);
            var result = _request.DoPost(_config, api, true);
            if (result.ContainsKey("result_code"))
            {
                string result_code = result["result_code"].ToString();
                if (result_code == "SUCCESS")
                {
                    ret.Status = "APPLYING";
                }
            }
            return ret;
        }

        public string OrderRefundQuery(string out_refund_no)
        {
            TenpayOrderRefundQueryApi api = new TenpayOrderRefundQueryApi(_config, out_refund_no);
            var result = _request.DoPost(_config, api, true);

            return _request.ReturnContent;
        }

    }
}
