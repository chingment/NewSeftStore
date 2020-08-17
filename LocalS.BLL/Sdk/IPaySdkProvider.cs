using LocalS.BLL.Biz;
using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{

    public interface IPaySdkProvider<T>
    {
        PayBuildQrCodeResult PayBuildQrCode(T config, E_PayCaller payCaller, string merch_id, string store_id, string machine_id, string order_sn, decimal order_amount, string goods_tag, string create_ip, string body, DateTime? time_expire);
        string PayQuery(T config, string order_sn);
        PayResult Convert2PayResultByPayQuery(T config, string content);
        PayResult Convert2PayResultByNotifyUrl(T config, string content);
        PayRefundResult PayRefund(T config, string payTranId, string payRefundId, decimal total_fee, decimal refund_fee, string refund_desc);
        string PayRefundQuery(T config, string payRefundId);
    }
}
