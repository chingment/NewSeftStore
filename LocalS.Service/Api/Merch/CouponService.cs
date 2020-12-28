using LocalS.BLL;
using LocalS.Entity;
using LocalS.Service.UI;
using Lumos;
using Lumos.Redis;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.Merch
{

    public class CouponService : BaseDbContext
    {
        public string GetCategoryName(E_Coupon_Category category)
        {
            string name = "";
            switch (category)
            {
                case E_Coupon_Category.All:
                    name = "全场赠券";
                    break;
                case E_Coupon_Category.NewUser:
                    name = "新用户赠券";
                    break;
                case E_Coupon_Category.Memeber:
                    name = "会员赠券";
                    break;
                case E_Coupon_Category.Shopping:
                    name = "购物赠券";
                    break;
                default:
                    name = "未知";
                    break;
            }
            return name;
        }

        public string GetUseAreaTypeName(E_Coupon_UseAreaType useAreaType)
        {
            string name = "";
            switch (useAreaType)
            {
                case E_Coupon_UseAreaType.All:
                    name = "全场使用";
                    break;
                case E_Coupon_UseAreaType.Store:
                    name = "指定店铺";
                    break;
                case E_Coupon_UseAreaType.ProductKind:
                    name = "指定分类";
                    break;
                case E_Coupon_UseAreaType.ProductSpu:
                    name = "指定商品";
                    break;
                default:
                    name = "未知";
                    break;
            }
            return name;
        }

        public string GetFaceTypeName(E_Coupon_FaceType faceType)
        {
            string name = "";
            switch (faceType)
            {
                case E_Coupon_FaceType.ShopVoucher:
                    name = "购物代金券";
                    break;
                case E_Coupon_FaceType.ShopDiscount:
                    name = "购物折扣券";
                    break;
                case E_Coupon_FaceType.RentVoucher:
                    name = "租金代金券";
                    break;
                case E_Coupon_FaceType.DepositVoucher:
                    name = "押金代金券";
                    break;
                case E_Coupon_FaceType.EntranceTicket:
                    name = "入场券";
                    break;
                default:
                    name = "未知";
                    break;
            }
            return name;
        }

        public string GetUseModeName(E_Coupon_UseMode useMode)
        {
            string name = "";
            switch (useMode)
            {
                case E_Coupon_UseMode.Pay:
                    name = "支付使用";
                    break;
                case E_Coupon_UseMode.ScanCode:
                    name = "扫码使用";
                    break;
                default:
                    name = "未知";
                    break;
            }
            return name;
        }

        public CustomJsonResult GetList(string operater, string merchId, RupCouponGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.Coupon
                         where
                          (rup.Name == null || u.Name.Contains(rup.Name)) &&
                         u.IsDelete == false &&
                         u.MerchId == merchId
                         select new { u.Id, u.Name, u.UseMode, u.Category, u.ShopMode, u.UseAreaType, u.UseAreaValue, u.AtLeastAmount, u.FaceType, u.FaceValue, u.StartTime, u.EndTime, u.PerLimitNum, u.IsDelete, u.CreateTime });

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                string atLeastAmount = "无限制";
                if (item.AtLeastAmount > 0)
                {
                    atLeastAmount = string.Format("满{0}元可用", item.AtLeastAmount);
                }

                string faceValue = "";

                if (item.FaceType == E_Coupon_FaceType.ShopVoucher)
                {
                    faceValue = string.Format("{0}元", item.FaceValue);
                }
                else if (item.FaceType == E_Coupon_FaceType.ShopDiscount)
                {
                    faceValue = string.Format("{0}折", item.FaceValue);
                }
                else if (item.FaceType == E_Coupon_FaceType.RentVoucher)
                {
                    faceValue = string.Format("{0}元", item.FaceValue);
                }
                else if (item.FaceType == E_Coupon_FaceType.DepositVoucher)
                {
                    faceValue = string.Format("{0}元", item.FaceValue);
                }
                else if (item.FaceType == E_Coupon_FaceType.EntranceTicket)
                {
                    faceValue = string.Format("{0}元", item.FaceValue);
                }

                string validDate = string.Format("{0}至{1}", item.StartTime.ToUnifiedFormatDate(), item.EndTime.ToUnifiedFormatDate());

                string status = "";

                if (DateTime.Now < item.StartTime)
                {
                    status = "未生效";
                }
                else if (DateTime.Now >= item.StartTime && DateTime.Now <= item.EndTime)
                {
                    status = "已生效";
                }
                else
                {
                    status = "已过期";
                }

                olist.Add(new
                {
                    Id = item.Id,
                    Name = item.Name,
                    Category = GetCategoryName(item.Category),
                    UseAreaType = GetUseAreaTypeName(item.UseAreaType),
                    AtLeastAmount = atLeastAmount,
                    FaceType = GetFaceTypeName(item.FaceType),
                    UseMode = GetUseModeName(item.UseMode),
                    FaceValue = faceValue,
                    ValidDate = validDate,
                    Status = status
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public CustomJsonResult InitAdd(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var ret = new RetCouponInitAdd();

            var d_stores = CurrentDb.Store.Where(m => m.MerchId == merchId && m.IsDelete == false).OrderByDescending(r => r.CreateTime).ToList();

            foreach (var d_store in d_stores)
            {
                var optionsStore = new OptionNode();
                optionsStore.Value = d_store.Id;
                optionsStore.Label = d_store.Name;
                ret.OptionsStores.Add(optionsStore);
            }

            ret.OptionsMemberLevels.Add(new OptionNode { Label = "普通用户", Value = "0" });

            var d_memberLevelSts = CurrentDb.MemberLevelSt.Where(m => m.MerchId == merchId).ToList();

            foreach (var d_memberLevelSt in d_memberLevelSts)
            {
                var optionsStore = new OptionNode();
                optionsStore.Value = d_memberLevelSt.Level.ToString();
                optionsStore.Label = d_memberLevelSt.Name;
                ret.OptionsMemberLevels.Add(optionsStore);
            }

            ret.OptionsProductKinds = MerchServiceFactory.PrdProduct.GetKindTree();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }


        public CustomJsonResult Add(string operater, string merchId, RupCouponAdd rop)
        {
            var result = new CustomJsonResult();

            if (string.IsNullOrEmpty(rop.Name))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "名称不能为空");
            }

            if (rop.Category == E_Coupon_Category.Unknow)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择优惠券类型");
            }

            //if (rop.UseMode == E_Coupon_UseMode.Unknow)
            //{
            //    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择使用方式");
            //}

            if (rop.FaceType == E_Coupon_FaceType.Unknow)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择券种");
            }

            if (rop.UseTimeType == E_Coupon_UseTimeType.Unknow)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择使用时间类型");
            }

            if (rop.UseAreaType == E_Coupon_UseAreaType.Unknow)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择可使用范围");
            }

            if (rop.Category == E_Coupon_Category.Memeber || rop.Category == E_Coupon_Category.NewUser)
            {
                rop.IssueQuantity = -1;//改为-1 不限制
            }
            else
            {
                if (rop.IssueQuantity < 0)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "发行量必须大于零");
                }
            }

            if (rop.FaceValue < 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "券值必须大于零");
            }

            if (rop.PerLimitNum < 1)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "每人限领必须大于零");
            }

            using (TransactionScope ts = new TransactionScope())
            {
                var d_coupon = new Coupon();
                d_coupon.Id = IdWorker.Build(IdType.NewGuid);
                d_coupon.MerchId = merchId;
                d_coupon.Name = rop.Name;
                d_coupon.Category = rop.Category;
                d_coupon.ShopMode = E_Coupon_ShopMode.Unknow;
                d_coupon.UseMode = rop.UseMode;
                d_coupon.IssueQuantity = rop.IssueQuantity;
                d_coupon.FaceType = rop.FaceType;
                d_coupon.FaceValue = rop.FaceValue;
                d_coupon.PerLimitNum = rop.PerLimitNum;
                d_coupon.AtLeastAmount = rop.AtLeastAmount;
                d_coupon.StartTime = DateTime.Parse(rop.ValidDate[0]);
                d_coupon.EndTime = DateTime.Parse(rop.ValidDate[1]);
                d_coupon.UseTimeType = rop.UseTimeType;

                if (rop.UseTimeType == E_Coupon_UseTimeType.ValidDay)
                {
                    d_coupon.UseTimeValue = rop.UseTimeValue.ToString();
                }
                else if (rop.UseTimeType == E_Coupon_UseTimeType.TimeArea)
                {
                    d_coupon.UseTimeValue = rop.UseTimeValue.ToJsonString();
                }

                d_coupon.PerLimitTimeType = rop.PerLimitTimeType;

                if (rop.PerLimitTimeType == E_Coupon_PerLimitTimeType.UnLimit)
                {
                    d_coupon.PerLimitTimeNum = 0;
                }
                else
                {
                    d_coupon.PerLimitTimeNum = rop.PerLimitTimeNum;
                }
                d_coupon.LimitMemberLevels = rop.LimitMemberLevels.ToJsonString();
                d_coupon.IsSuperposition = false;
                d_coupon.IsDelete = false;
                d_coupon.Description = rop.Description;
                d_coupon.UseAreaType = rop.UseAreaType;
                d_coupon.UseAreaValue = rop.UseAreaValue.ToJsonString();
                d_coupon.CreateTime = DateTime.Now;
                d_coupon.Creator = operater;
                CurrentDb.Coupon.Add(d_coupon);
                CurrentDb.SaveChanges();

                if (rop.UseAreaType != E_Coupon_UseAreaType.All)
                {
                    var useAreaValue = rop.UseAreaValue.ToJsonObject<JArray>();

                    foreach (var item in useAreaValue)
                    {

                        var d_couponUseAreaObj = new CouponUseAreaObj();
                        d_couponUseAreaObj.Id = IdWorker.Build(IdType.NewGuid);
                        d_couponUseAreaObj.CouponId = d_coupon.Id;
                        d_couponUseAreaObj.ObjId = item["id"].ToString();
                        d_couponUseAreaObj.ObjName = item["name"].ToString();
                        d_couponUseAreaObj.ObjType = item["type"].ToString();
                        d_couponUseAreaObj.CreateTime = DateTime.Now;
                        d_couponUseAreaObj.Creator = operater;
                        CurrentDb.CouponUseAreaObj.Add(d_couponUseAreaObj);

                    }

                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            }


            return result;
        }

        public CustomJsonResult InitEdit(string operater, string merchId, string couponId)
        {
            var result = new CustomJsonResult();

            var ret = new RetCouponInitEdit();

            var d_coupon = CurrentDb.Coupon.Where(m => m.Id == couponId && m.MerchId == merchId).FirstOrDefault();

            if (d_coupon != null)
            {
                ret.Coupon.Id = d_coupon.Id;
                ret.Coupon.Name = d_coupon.Name;
                ret.Coupon.Category = d_coupon.Category;
                ret.Coupon.ShopMode = d_coupon.ShopMode;
                ret.Coupon.IssueQuantity = d_coupon.IssueQuantity;
                ret.Coupon.FaceType = d_coupon.FaceType;
                ret.Coupon.FaceValue = d_coupon.FaceValue;
                ret.Coupon.PerLimitNum = d_coupon.PerLimitNum;
                ret.Coupon.AtLeastAmount = d_coupon.AtLeastAmount;
                ret.Coupon.ValidDate = new string[2] { d_coupon.StartTime.ToUnifiedFormatDate(), d_coupon.EndTime.ToUnifiedFormatDate() };
                ret.Coupon.UseAreaType = d_coupon.UseAreaType;
                if (d_coupon.UseAreaType != E_Coupon_UseAreaType.All)
                {
                    ret.Coupon.UseAreaValue = d_coupon.UseAreaValue.ToJsonObject<object>();
                }

                ret.Coupon.UseMode = d_coupon.UseMode;
                ret.Coupon.UseTimeType = d_coupon.UseTimeType;
                if (d_coupon.UseTimeType == E_Coupon_UseTimeType.ValidDay)
                {
                    ret.Coupon.UseTimeValue = int.Parse(d_coupon.UseTimeValue);
                }
                else if (d_coupon.UseTimeType == E_Coupon_UseTimeType.TimeArea)
                {
                    ret.Coupon.UseTimeValue = d_coupon.UseTimeValue.ToJsonObject<string[]>();
                }

                ret.Coupon.PerLimitTimeType = d_coupon.PerLimitTimeType;
                ret.Coupon.PerLimitTimeNum = d_coupon.PerLimitTimeNum;
                ret.Coupon.LimitMemberLevels = d_coupon.LimitMemberLevels.ToJsonObject<List<string>>();
                ret.Coupon.Description = d_coupon.Description;
                ret.Coupon.IsSuperposition = d_coupon.IsSuperposition;
            }

            var d_stores = CurrentDb.Store.Where(m => m.MerchId == merchId && m.IsDelete == false).OrderByDescending(r => r.CreateTime).ToList();

            foreach (var d_store in d_stores)
            {
                var optionsStore = new OptionNode();

                optionsStore.Value = d_store.Id;
                optionsStore.Label = d_store.Name;
                ret.OptionsStores.Add(optionsStore);
            }

            ret.OptionsMemberLevels.Add(new OptionNode { Label = "普通用户", Value = "0" });

            var d_memberLevelSts = CurrentDb.MemberLevelSt.Where(m => m.MerchId == merchId).ToList();

            foreach (var d_memberLevelSt in d_memberLevelSts)
            {
                var optionsStore = new OptionNode();
                optionsStore.Value = d_memberLevelSt.Level.ToString();
                optionsStore.Label = d_memberLevelSt.Name;
                ret.OptionsMemberLevels.Add(optionsStore);
            }

            ret.OptionsProductKinds = MerchServiceFactory.PrdProduct.GetKindTree();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult Edit(string operater, string merchId, RupCouponEdit rop)
        {
            var result = new CustomJsonResult();

            if (string.IsNullOrEmpty(rop.Name))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "名称不能为空");
            }

            if (rop.Category == E_Coupon_Category.Unknow)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择优惠券类型");
            }

            //if (rop.UseMode == E_Coupon_UseMode.Unknow)
            //{
            //    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择使用方式");
            //}

            if (rop.FaceType == E_Coupon_FaceType.Unknow)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择券种");
            }

            if (rop.UseTimeType == E_Coupon_UseTimeType.Unknow)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择使用时间类型");
            }

            if (rop.UseAreaType == E_Coupon_UseAreaType.Unknow)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择可使用范围");
            }

            if (rop.Category == E_Coupon_Category.Memeber || rop.Category == E_Coupon_Category.NewUser)
            {
                rop.IssueQuantity = -1;//改为-1 不限制
            }
            else
            {
                if (rop.IssueQuantity < 0)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "发行量必须大于零");
                }
            }

            if (rop.FaceValue < 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "券值必须大于零");
            }

            if (rop.PerLimitNum < 1)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "每人限领必须大于零");
            }

            using (TransactionScope ts = new TransactionScope())
            {
                var d_coupon = CurrentDb.Coupon.Where(m => m.Id == rop.Id && m.MerchId == merchId).FirstOrDefault();
                if (d_coupon == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到对应的数据");
                }

                d_coupon.Name = rop.Name;
                d_coupon.Category = rop.Category;
                d_coupon.ShopMode = E_Coupon_ShopMode.Unknow;
                d_coupon.UseMode = rop.UseMode;
                d_coupon.IssueQuantity = rop.IssueQuantity;
                d_coupon.FaceType = rop.FaceType;
                d_coupon.FaceValue = rop.FaceValue;
                d_coupon.PerLimitNum = rop.PerLimitNum;
                d_coupon.AtLeastAmount = rop.AtLeastAmount;
                d_coupon.StartTime = DateTime.Parse(rop.ValidDate[0]);
                d_coupon.EndTime = DateTime.Parse(rop.ValidDate[1]);
                d_coupon.UseTimeType = rop.UseTimeType;

                if (rop.UseTimeType == E_Coupon_UseTimeType.ValidDay)
                {
                    d_coupon.UseTimeValue = rop.UseTimeValue.ToString();
                }
                else if (rop.UseTimeType == E_Coupon_UseTimeType.TimeArea)
                {
                    d_coupon.UseTimeValue = rop.UseTimeValue.ToJsonString();
                }

                d_coupon.PerLimitTimeType = rop.PerLimitTimeType;

                if (rop.PerLimitTimeType == E_Coupon_PerLimitTimeType.UnLimit)
                {
                    d_coupon.PerLimitTimeNum = 0;
                }
                else
                {
                    d_coupon.PerLimitTimeNum = rop.PerLimitTimeNum;
                }

                d_coupon.LimitMemberLevels = rop.LimitMemberLevels.ToJsonString();

                d_coupon.IsSuperposition = false;
                d_coupon.IsDelete = false;
                d_coupon.Description = rop.Description;
                d_coupon.UseAreaType = rop.UseAreaType;
                d_coupon.UseAreaValue = rop.UseAreaValue.ToJsonString();
                d_coupon.MendTime = DateTime.Now;
                d_coupon.Mender = operater;
                CurrentDb.SaveChanges();

                var d_couponUseAreaObjs = CurrentDb.CouponUseAreaObj.Where(m => m.CouponId == d_coupon.Id).ToList();

                foreach (var item in d_couponUseAreaObjs)
                {
                    CurrentDb.CouponUseAreaObj.Remove(item);
                }

                if (rop.UseAreaType != E_Coupon_UseAreaType.All)
                {
                    var useAreaValue = rop.UseAreaValue.ToJsonObject<JArray>();

                    foreach (var item in useAreaValue)
                    {

                        var d_couponUseAreaObj = new CouponUseAreaObj();
                        d_couponUseAreaObj.Id = IdWorker.Build(IdType.NewGuid);
                        d_couponUseAreaObj.CouponId = d_coupon.Id;
                        d_couponUseAreaObj.ObjId = item["id"].ToString();
                        d_couponUseAreaObj.ObjName = item["name"].ToString();
                        d_couponUseAreaObj.ObjType = item["type"].ToString();
                        d_couponUseAreaObj.CreateTime = DateTime.Now;
                        d_couponUseAreaObj.Creator = operater;
                        CurrentDb.CouponUseAreaObj.Add(d_couponUseAreaObj);

                    }

                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            }


            return result;
        }

    }
}
