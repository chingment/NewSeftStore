using LocalS.BLL.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public interface IPaySdkProvider
    {
        WxPayBuildResultByNt WxPayBuildByNt(Object config, string merch_id, string store_id, string machine_id, string order_sn, decimal order_amount, string goods_tag, string create_ip, string body, OrderAttachModel attach, DateTime time_expire);
    }
}
