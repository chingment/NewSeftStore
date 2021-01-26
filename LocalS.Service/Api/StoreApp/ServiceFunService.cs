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
    public class ServiceFunService : BaseService
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
            else if (rop.Code.IndexOf("couponwtcode@v1:") > -1 || rop.Code.IndexOf("couponwtcode@v2:") > -1)
            {
                string dec_code = "";
                if (rop.Code.IndexOf("couponwtcode@v1:") > -1)
                {
                    dec_code = rop.Code.Split(':')[1];
                }
                else if (rop.Code.IndexOf("couponwtcode@v2:") > -1)
                {
                    dec_code = MyDESCryptoUtil.DecodeQrcode2CouponWtCode(rop.Code);
                }

                var clientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == dec_code).FirstOrDefault();

                if (clientCoupon == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到优惠券，请重新输入");
                }

                return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { couponId = clientCoupon.Id, action = "WtCouponCode" });

            }

            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "数据查找失败");

        }

        public CustomJsonResult GetMyReffSkus(string operater, string clientUserId, RupServiceFunGetMyReffSkus rup)
        {
            var result = new CustomJsonResult();


            var pageEntiy = new PageEntity<object>();

            var query = (from o in CurrentDb.ClientReffSku
                         where (o.ClientUserId == clientUserId && o.Status == E_ClientReffSkuStatus.Valid)
                         select new
                         {
                             o.Id,
                             o.ClientUserId,
                             o.SkuMainImgUrl,
                             o.SkuName,
                             o.Quantity,
                             o.CreateTime,
                             o.ReffClientUserId
                         }
             );


            int pageSize = 10;

            pageEntiy.PageIndex = rup.PageIndex;
            pageEntiy.PageSize = pageSize;
            pageEntiy.Total = query.Count();
            pageEntiy.PageCount = (pageEntiy.Total + pageEntiy.PageSize - 1) / pageEntiy.PageSize;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (rup.PageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> models = new List<object>();

            foreach (var item in list)
            {
                var d_ReffClientUser = CurrentDb.SysClientUser.Where(m => m.Id == item.ClientUserId).FirstOrDefault();
                if (d_ReffClientUser != null)
                {
                    var model = new
                    {
                        MainImgUrl = item.SkuMainImgUrl,
                        Name = item.SkuName,
                        Quantity = item.Quantity,
                        ReffClientUserAvatar = d_ReffClientUser.Avatar,
                        ReffClientUserNickName = d_ReffClientUser.NickName,
                        StatusName = "有效"
                    };

                    pageEntiy.Items.Add(model);
                }
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", pageEntiy);

            return result;

        }
    }
}
