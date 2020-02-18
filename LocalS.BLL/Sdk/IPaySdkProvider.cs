using LocalS.BLL.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public interface IPaySdkProvider<T>
    {
        //构建微信支付二维码
        WxPayBuildByNtResult WxPayBuildByNt(T config, string merch_id, string store_id, string machine_id, string order_sn, decimal order_amount, string goods_tag, string create_ip, string body, DateTime time_expire);
        //构建支付宝支付二维码
        AliPayBuildByNtResult AliPayBuildByNt(T config, string merch_id, string store_id, string machine_id, string order_sn, decimal order_amount, string goods_tag, string create_ip, string body, DateTime time_expire);

        PayResult PayQuery(T config, string order_sn);

    }
}
