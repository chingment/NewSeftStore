using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Entity;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LocalS.Service.Api.StoreApp
{
    public class ServiceFunService : BaseDbContext
    {
        public CustomJsonResult ScanCodeResult(string operater, string clientUserId, RopServiceFunScanCodeResult rop)
        {
            var result = new CustomJsonResult();

            if (string.IsNullOrEmpty(rop.Code))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "数据码为空");
            }

            LogUtil.Info("Code1=>>" + rop.Code);


            if (rop.Code.IndexOf("pickupcode@v1:") > -1 || rop.Code.IndexOf("pickupcode@v2:") > -1)
            {
                string dec_code = "";
                if (rop.Code.IndexOf("pickupcode@v1:") > -1)
                {
                    dec_code = rop.Code.Split(':')[1];
                }
                else if (rop.Code.IndexOf("pickupcode@v2:") > -1)
                {
                    dec_code = MyDESCryptoUtil.DecodeQrcode2PickupCode(rop.Code);
                }

                LogUtil.Info("Code2=>>" + dec_code);

                var order = CurrentDb.Order.Where(m => m.PickupCode == dec_code).FirstOrDefault();

                if (order == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该订单，请重新输入");
                }

                return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { orderId = order.Id, action = "CfSelfTakeOrder" });

            }

            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "数据查找失败");

        }
    }
}
