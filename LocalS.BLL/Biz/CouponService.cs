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
                    var d_Coupon = CurrentDb.Coupon.Where(m => m.Id == couponId).FirstOrDefault();

                    var d_ClientCoupon = new ClientCoupon();
                    d_ClientCoupon.Id = IdWorker.Build(IdType.NewGuid);
                    d_ClientCoupon.Sn = "";
                    d_ClientCoupon.MerchId = merchId;
                    d_ClientCoupon.ClientUserId = clientUserId;
                    d_ClientCoupon.CouponId = d_Coupon.Id;
                    if (d_Coupon.UseTimeType == E_Coupon_UseTimeType.ValidDay)
                    {
                        d_ClientCoupon.ValidStartTime = DateTime.Now;
                        d_ClientCoupon.ValidEndTime = DateTime.Now.AddDays(int.Parse(d_Coupon.UseTimeValue));
                    }
                    else if (d_Coupon.UseTimeType == E_Coupon_UseTimeType.TimeArea)
                    {
                        string[] arr_UseTimeValue = d_Coupon.UseTimeValue.ToJsonObject<string[]>();
                        if (arr_UseTimeValue.Length == 2)
                        {
                            d_ClientCoupon.ValidStartTime = DateTime.Parse(arr_UseTimeValue[0]);
                            d_ClientCoupon.ValidEndTime = DateTime.Parse(arr_UseTimeValue[1]);
                        }
                    }

                    d_ClientCoupon.Status = E_ClientCouponStatus.WaitUse;
                    d_ClientCoupon.SourceObjType = senderType;
                    d_ClientCoupon.SourceObjId = senderId;
                    d_ClientCoupon.SourcePoint = sendPoint;
                    d_ClientCoupon.SourceTime = DateTime.Now;
                    d_ClientCoupon.SourceDes = sendDes;
                    d_ClientCoupon.Creator = senderId;
                    d_ClientCoupon.CreateTime = DateTime.Now;
                    CurrentDb.ClientCoupon.Add(d_ClientCoupon);

                    d_Coupon.ReceivedQuantity += 1;
                    d_Coupon.Mender = senderId;
                    d_Coupon.MendTime = DateTime.Now;
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
                    var d_ClientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == clientCouponId).FirstOrDefault();
                    if (d_ClientCoupon != null)
                    {
                        d_ClientCoupon.Status = E_ClientCouponStatus.Frozen;
                        d_ClientCoupon.Mender = operater;
                        d_ClientCoupon.MendTime = DateTime.Now;

                        var d_Ccopon = CurrentDb.Coupon.Where(m => m.Id == d_ClientCoupon.CouponId).FirstOrDefault();
                        if (d_Ccopon != null)
                        {
                            d_Ccopon.FrozenQuantity += 1;
                            d_Ccopon.Mender = operater;
                            d_Ccopon.MendTime = DateTime.Now;
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
                    var d_ClientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == clientCouponId).FirstOrDefault();
                    if (d_ClientCoupon != null)
                    {
                        d_ClientCoupon.Status = E_ClientCouponStatus.WaitUse;
                        d_ClientCoupon.Mender = operater;
                        d_ClientCoupon.MendTime = DateTime.Now;

                        var d_Copon = CurrentDb.Coupon.Where(m => m.Id == d_ClientCoupon.CouponId).FirstOrDefault();
                        if (d_Copon != null)
                        {
                            d_Copon.FrozenQuantity -= 1;
                            d_Copon.Mender = operater;
                            d_Copon.MendTime = DateTime.Now;
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
                    var d_ClientCoupon = CurrentDb.ClientCoupon.Where(m => m.Id == clientCouponId).FirstOrDefault();
                    if (d_ClientCoupon != null)
                    {
                        d_ClientCoupon.Status = E_ClientCouponStatus.Used;
                        d_ClientCoupon.UseTime = DateTime.Now;
                        d_ClientCoupon.Mender = operater;
                        d_ClientCoupon.MendTime = DateTime.Now;

                        var d_Copon = CurrentDb.Coupon.Where(m => m.Id == d_ClientCoupon.CouponId).FirstOrDefault();
                        if (d_Copon != null)
                        {
                            d_Copon.UsedQuantity += 1;
                            d_Copon.Mender = operater;
                            d_Copon.MendTime = DateTime.Now;
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
