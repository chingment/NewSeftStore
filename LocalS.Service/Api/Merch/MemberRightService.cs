using LocalS.BLL;
using LocalS.BLL.Biz;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class MemberRightService : BaseService
    {
        public CustomJsonResult GetLevelSts(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var d_Merch = CurrentDb.Merch.Where(m => m.Id == merchId).FirstOrDefault();

            var d_MemberLevelSts = CurrentDb.MemberLevelSt.Where(m => m.MerchId == merchId).OrderBy(m => m.Level).ToList();

            List<object> levelSts = new List<object>();

            foreach (var d_MemberLevelSt in d_MemberLevelSts)
            {
                levelSts.Add(new
                {
                    Id = d_MemberLevelSt.Id,
                    Name = d_MemberLevelSt.Name,
                    MainImgUrl = d_MemberLevelSt.MainImgUrl,
                    Tag = d_MemberLevelSt.Tag,
                    Level = d_MemberLevelSt.Level
                });
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { IsOpenMemberRight = d_Merch.IsOpenMemberRight, levelSts = levelSts });
            return result;
        }

        public CustomJsonResult InitManage(string operater, string merchId, string levelStId)
        {
            var ret = new { };

            List<object> levelSts = new List<object>();

            object curLevelSt = null;

            var d_MemberLevelSts = CurrentDb.MemberLevelSt.Where(m => m.MerchId == merchId).ToList();
            foreach (var d_MemberLevelSt in d_MemberLevelSts)
            {

                if (d_MemberLevelSt.Id == levelStId)
                {
                    curLevelSt = new { Id = d_MemberLevelSt.Id, Name = d_MemberLevelSt.Name };
                }

                levelSts.Add(new { Id = d_MemberLevelSt.Id, Name = d_MemberLevelSt.Name });
            }

            if (curLevelSt == null)
            {
                if (levelSts.Count > 0)
                {
                    curLevelSt = levelSts[0];
                }
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { levelSts, curLevelSt });
        }

        public CustomJsonResult GetLevelSt(string operater, string merchId, string levelId)
        {
            var result = new CustomJsonResult();

            var d_MemberLevelSt = CurrentDb.MemberLevelSt.Where(m => m.MerchId == merchId && m.Id == levelId).FirstOrDefault();

            var ret = new { Id = d_MemberLevelSt.Id, Name = d_MemberLevelSt.Name, Discount = d_MemberLevelSt.Discount };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult SetLevelSt(string operater, string merchId, RopMemberRightSetLevelSt rop)
        {
            var result = new CustomJsonResult();

            if (rop.Discount == 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "折扣不能等于0");
            }
            else if (rop.Discount < 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "折扣不能小于0");
            }
            else if (rop.Discount > 1)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "折扣不能大于1");
            }

            var d_MemberLevelSt = CurrentDb.MemberLevelSt.Where(m => m.MerchId == merchId && m.Id == rop.LevelStId).FirstOrDefault();

            if (d_MemberLevelSt != null)
            {
                d_MemberLevelSt.Discount = rop.Discount;
                CurrentDb.SaveChanges();
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            return result;
        }

        public CustomJsonResult GetFeeSts(string operater, string merchId, string levelStId)
        {
            var result = new CustomJsonResult();

            var d_MemberFeeSts = CurrentDb.MemberFeeSt.Where(m => m.MerchId == merchId && m.LevelStId == levelStId).OrderBy(m => m.FeeType).ToList();

            List<object> feeSts = new List<object>();

            foreach (var d_MemberFeeSt in d_MemberFeeSts)
            {
                var status = new StatusModel();

                if (d_MemberFeeSt.IsStop)
                {
                    status = new StatusModel(2, "停用");
                }
                else
                {
                    status = new StatusModel(1, "使用中");
                }

                feeSts.Add(new
                {
                    Id = d_MemberFeeSt.Id,
                    FeeTypeName = d_MemberFeeSt.FeeType,
                    FeeOriginalValue = d_MemberFeeSt.FeeOriginalValue,
                    FeeSaleValue = d_MemberFeeSt.FeeSaleValue,
                    Name = d_MemberFeeSt.Name,
                    MainImgUrl = d_MemberFeeSt.MainImgUrl,
                    Status = status,
                    IsStop = d_MemberFeeSt.IsStop,
                    Tag = d_MemberFeeSt.Tag
                });
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { feeSts = feeSts });
            return result;
        }

        public CustomJsonResult SetFeeSt(string operater, string merchId, RopMemberRightSetFeeSt rop)
        {
            var result = new CustomJsonResult();

            var d_MemberFeeSt = CurrentDb.MemberFeeSt.Where(m => m.MerchId == merchId && m.Id == rop.FeeStId).FirstOrDefault();

            if (d_MemberFeeSt == null)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败[01]");

            if (rop.OriginalValue <= rop.SaleValue)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败，原价不能小于实际价[02]");


            d_MemberFeeSt.FeeOriginalValue = rop.OriginalValue;
            d_MemberFeeSt.FeeSaleValue = rop.SaleValue;
            d_MemberFeeSt.IsStop = rop.IsStop;
            CurrentDb.SaveChanges();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            return result;
        }

        public CustomJsonResult GetCoupons(string operater, string merchId, RupMemberRightGetCoupons rup)
        {
            var result = new CustomJsonResult();

            var query = (from m in CurrentDb.MemberCouponSt
                         join s in CurrentDb.Coupon on m.CouponId equals s.Id into temp
                         from u in temp.DefaultIfEmpty()
                         where
                         m.MerchId == merchId
                         && m.LevelStId == rup.LevelStId
                         select new { CouponStId = m.Id, CouponId = u.Id, m.Quantity, u.Name, u.UseMode, u.Category, u.ShopMode, u.UseAreaType, u.UseAreaValue, u.AtLeastAmount, u.FaceType, u.FaceValue, u.StartTime, u.EndTime, u.IsDelete, u.CreateTime });

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = int.MaxValue;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                olist.Add(new
                {
                    CouponStId = item.CouponStId,
                    CouponId = item.CouponId,
                    Name = item.Name,
                    Quantity = item.Quantity,
                    Category = MerchServiceFactory.Coupon.GetCategoryName(item.Category),
                    UseAreaType = MerchServiceFactory.Coupon.GetUseAreaTypeName(item.UseAreaType),
                    AtLeastAmount = MerchServiceFactory.Coupon.GetAtLeastAmount(item.AtLeastAmount),
                    FaceType = MerchServiceFactory.Coupon.GetFaceTypeName(item.FaceType),
                    UseMode = MerchServiceFactory.Coupon.GetUseModeName(item.UseMode),
                    FaceValue = MerchServiceFactory.Coupon.GetFaceValue(item.FaceType, item.FaceValue),
                    ValidDate = MerchServiceFactory.Coupon.GetValidDate(item.StartTime, item.EndTime),
                    Status = MerchServiceFactory.Coupon.GetStatus(item.StartTime, item.EndTime)
                });

            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);


            return result;

        }

        public CustomJsonResult RemoveCoupon(string operater, string merchId, RopMemberRightRemoveCoupon rop)
        {
            var result = new CustomJsonResult();

            var d_MemberCouponSt = CurrentDb.MemberCouponSt.Where(m => m.Id == rop.CouponStId).FirstOrDefault();

            CurrentDb.MemberCouponSt.Remove(d_MemberCouponSt);
            CurrentDb.SaveChanges();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "移除成功");

            return result;

        }

        public CustomJsonResult AddCoupon(string operater, string merchId, RopMemberRightAddCoupon rop)
        {
            var result = new CustomJsonResult();

            var d_MemberCouponSt = CurrentDb.MemberCouponSt.Where(m => m.MerchId == merchId && m.CouponId == rop.CouponId && m.LevelStId == rop.LevelStId).FirstOrDefault();
            if (d_MemberCouponSt != null)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "已经存在");

            d_MemberCouponSt = new Entity.MemberCouponSt();
            d_MemberCouponSt.Id = IdWorker.Build(IdType.NewGuid);
            d_MemberCouponSt.MerchId = merchId;
            d_MemberCouponSt.LevelStId = rop.LevelStId;
            d_MemberCouponSt.CouponId = rop.CouponId;
            d_MemberCouponSt.Quantity = rop.Quantity;
            d_MemberCouponSt.Creator = operater;
            d_MemberCouponSt.CreateTime = DateTime.Now;
            CurrentDb.MemberCouponSt.Add(d_MemberCouponSt);
            CurrentDb.SaveChanges();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "添加成功");

            return result;

        }

        public CustomJsonResult GetSkus(string operater, string merchId, RupMemberRightGetSkus rup)
        {
            var result = new CustomJsonResult();

            var query = (from m in CurrentDb.MemberSkuSt
                         where
                         m.MerchId == merchId
                         && m.LevelStId == rup.LevelStId
                         select new { m.Id, m.SkuId, m.MemberPrice, m.CreateTime, m.StoreId, m.StatTime, m.EndTime, m.IsDisabled });

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = int.MaxValue;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                var r_sku = CacheServiceFactory.Product.GetSkuInfo(merchId, item.SkuId);

                var status = new StatusModel();
                if (item.IsDisabled)
                    status = new StatusModel(1, "无效");
                else
                    status = new StatusModel(2, "有效");

                olist.Add(new
                {
                    Id = item.Id,
                    SkuId = item.SkuId,
                    Name = r_sku.Name,
                    MainImgUrl = r_sku.MainImgUrl,
                    MemberPrice = item.MemberPrice,
                    IsDisabled = item.IsDisabled,
                    StatTime = item.StatTime,
                    EndTime = item.EndTime,
                    Status = status
                });

            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);


            return result;

        }

    }
}
