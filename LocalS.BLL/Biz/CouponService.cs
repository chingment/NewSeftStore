using LocalS.Entity;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.BLL.Biz
{
    public class CouponService : BaseService
    {
        public CustomJsonResult Send(string senderType, string senderId, string sendPoint, string sendDes, string merchId, string clientUserId, string couponId, int quantity)
        {

            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                for (int i = 0; i < quantity; i++)
                {
                    var d_coupon = CurrentDb.Coupon.Where(m => m.Id == couponId).FirstOrDefault();

                    var d_clientCoupon = new ClientCoupon();
                    d_clientCoupon.Id = IdWorker.Build(IdType.NewGuid);
                    d_clientCoupon.Sn = "";
                    d_clientCoupon.MerchId = merchId;
                    d_clientCoupon.ClientUserId = clientUserId;
                    d_clientCoupon.CouponId = d_coupon.Id;
                    if (d_coupon.UseTimeType == E_Coupon_UseTimeType.ValidDay)
                    {
                        d_clientCoupon.ValidStartTime = DateTime.Now;
                        d_clientCoupon.ValidEndTime = DateTime.Now.AddDays(int.Parse(d_coupon.UseTimeValue));
                    }
                    else if (d_coupon.UseTimeType == E_Coupon_UseTimeType.TimeArea)
                    {
                        string[] arr_UseTimeValue = d_coupon.UseTimeValue.ToJsonObject<string[]>();
                        if (arr_UseTimeValue.Length == 2)
                        {
                            d_clientCoupon.ValidStartTime = DateTime.Parse(arr_UseTimeValue[0]);
                            d_clientCoupon.ValidEndTime = DateTime.Parse(arr_UseTimeValue[1]);
                        }
                    }

                    d_clientCoupon.Status = E_ClientCouponStatus.WaitUse;
                    d_clientCoupon.SourceObjType = senderType;
                    d_clientCoupon.SourceObjId = senderId;
                    d_clientCoupon.SourcePoint = sendPoint;
                    d_clientCoupon.SourceTime = DateTime.Now;
                    d_clientCoupon.SourceDes = sendDes;
                    d_clientCoupon.Creator = senderId;
                    d_clientCoupon.CreateTime = DateTime.Now;
                    CurrentDb.ClientCoupon.Add(d_clientCoupon);

                    d_coupon.ReceivedQuantity += 1;
                    d_coupon.Mender = senderId;
                    d_coupon.MendTime = DateTime.Now;
                }

                CurrentDb.SaveChanges();

                ts.Complete();

                return result;
            }
        }

        public CustomJsonResult SignFrozen(string operater, string[] clientCouponIds)
        {

            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                foreach (var clientCouponId in clientCouponIds)
                {
                    var d_clientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == clientCouponId).FirstOrDefault();
                    if (d_clientCoupon != null)
                    {
                        d_clientCoupon.Status = E_ClientCouponStatus.Frozen;
                        d_clientCoupon.Mender = operater;
                        d_clientCoupon.MendTime = DateTime.Now;

                        var d_copon = CurrentDb.Coupon.Where(m => m.Id == d_clientCoupon.CouponId).FirstOrDefault();
                        if (d_copon != null)
                        {
                            d_copon.FrozenQuantity += 1;
                            d_copon.Mender = operater;
                            d_copon.MendTime = DateTime.Now;
                        }
                    }
                }

                CurrentDb.SaveChanges();

                ts.Complete();

                return result;
            }
        }

        public CustomJsonResult SignUnFrozen(string operater, string[] clientCouponIds)
        {

            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {

                foreach (var clientCouponId in clientCouponIds)
                {
                    var d_clientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == clientCouponId).FirstOrDefault();
                    if (d_clientCoupon != null)
                    {
                        d_clientCoupon.Status = E_ClientCouponStatus.WaitUse;
                        d_clientCoupon.Mender = operater;
                        d_clientCoupon.MendTime = DateTime.Now;

                        var d_copon = CurrentDb.Coupon.Where(m => m.Id == d_clientCoupon.CouponId).FirstOrDefault();
                        if (d_copon != null)
                        {
                            d_copon.FrozenQuantity -= 1;
                            d_copon.Mender = operater;
                            d_copon.MendTime = DateTime.Now;
                        }
                    }
                }

                CurrentDb.SaveChanges();

                ts.Complete();

                return result;
            }
        }

        public CustomJsonResult SignUsed(string operater, string[] clientCouponIds)
        {

            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {

                foreach (var clientCouponId in clientCouponIds)
                {
                    var d_clientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == clientCouponId).FirstOrDefault();
                    if (d_clientCoupon != null)
                    {
                        d_clientCoupon.Status = E_ClientCouponStatus.Used;
                        d_clientCoupon.UseTime = DateTime.Now;
                        d_clientCoupon.Mender = operater;
                        d_clientCoupon.MendTime = DateTime.Now;

                        var d_copon = CurrentDb.Coupon.Where(m => m.Id == d_clientCoupon.CouponId).FirstOrDefault();
                        if (d_copon != null)
                        {
                            d_copon.UsedQuantity += 1;
                            d_copon.Mender = operater;
                            d_copon.MendTime = DateTime.Now;
                        }
                    }
                }

                CurrentDb.SaveChanges();

                ts.Complete();

                return result;
            }
        }
    }
}
