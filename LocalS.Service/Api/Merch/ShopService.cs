using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
using LocalS.Entity;
using LocalS.Service.UI;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.Merch
{
    public class ShopService : BaseService
    {
        public StatusModel GetStatus(bool isOpen)
        {
            var status = new StatusModel();

            if (isOpen)
            {
                status.Value = 2;
                status.Text = "营业中";
            }
            else
            {
                status.Value = 1;
                status.Text = "已关闭";
            }

            return status;
        }

        public CustomJsonResult GetList(string operater, string merchId, RupShopGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.Shop
                         where (rup.Name == null || u.Name.Contains(rup.Name))
                         &&
                         u.MerchId == merchId
                         &&
                         u.IsDelete == false
                         select new { u.Id, u.Name, u.MainImgUrl, u.IsOpen, u.BriefDes, u.Address, u.AreaName, u.ContactAddress, u.ContactPhone, u.ContactName, u.CreateTime });


            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = int.MaxValue;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<string> d_shopIds = new List<string>();

            if (rup.OpCode == "select")
            {
                d_shopIds = CurrentDb.StoreShop.Where(m => m.StoreId == rup.StoreId).Select(m => m.ShopId).Distinct().ToList();
            }

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                string opTips = "";

                bool isCanSelect = false;

                if (rup.OpCode == "select")
                {
                    if (!d_shopIds.Contains(item.Id))
                    {
                        isCanSelect = true;
                    }
                    else
                    {
                        opTips = "已选择";
                    }

                }

                olist.Add(new
                {
                    Id = item.Id,
                    Name = item.Name,
                    MainImgUrl = item.MainImgUrl,
                    BriefDes = item.BriefDes,
                    Address = item.Address,
                    ContactAddress = item.ContactAddress,
                    ContactPhone = item.ContactPhone,
                    ContactName = item.ContactName,
                    IsCanSelect = isCanSelect,
                    OpTips = opTips,
                    CreateTime = item.CreateTime,
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);


            return result;
        }

        public CustomJsonResult GetDetails(string operater, string merchId, RupShopGetDetails rup)
        {
            var result = new CustomJsonResult();


            var d_Shop = CurrentDb.Shop.Where(m => m.MerchId == merchId && m.Id == rup.Id).FirstOrDefault();
            if (d_Shop == null)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到数据");
            var ret = new
            {
                Id = d_Shop.Id,
                Name = d_Shop.Name,
                Address = d_Shop.Address,
                AreaCode = d_Shop.AreaCode,
                AreaName = d_Shop.AreaName,
                BriefDes = d_Shop.BriefDes,
                MapPoint = new MapPoint(d_Shop.Lng, d_Shop.Lat),
                MainImgUrl = d_Shop.MainImgUrl,
                DisplayImgUrls = d_Shop.DisplayImgUrls.ToJsonObject<List<ImgSet>>(),
                ContactName = d_Shop.ContactName,
                ContactAddress = d_Shop.ContactAddress,
                ContactPhone = d_Shop.ContactPhone,
            };
            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult Save(string operater, string merchId, RopShopSave rop)
        {
            CustomJsonResult result = new CustomJsonResult();


            if (string.IsNullOrEmpty(rop.Id))
            {
                var d_Shop = CurrentDb.Shop.Where(m => m.MerchId == merchId && m.Name == rop.Name).FirstOrDefault();
                if (d_Shop != null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "名称已存在,请使用其它");
                }

                d_Shop = new Shop();
                d_Shop.Id = IdWorker.Build(IdType.NewGuid);
                d_Shop.MerchId = merchId;
                d_Shop.Name = rop.Name;
                d_Shop.Address = rop.Address;
                d_Shop.AreaCode = rop.AreaCode;
                d_Shop.AreaName = rop.AreaName;
                d_Shop.Lat = rop.AddressPoint.Lat;
                d_Shop.Lng = rop.AddressPoint.Lng;
                d_Shop.ContactName = rop.ContactName;
                d_Shop.ContactPhone = rop.ContactPhone;
                d_Shop.ContactAddress = rop.ContactAddress;
                d_Shop.BriefDes = rop.BriefDes;
                d_Shop.IsOpen = false;
                d_Shop.DisplayImgUrls = rop.DisplayImgUrls.ToJsonString();
                d_Shop.MainImgUrl = ImgSet.GetMain_O(d_Shop.DisplayImgUrls);
                d_Shop.CreateTime = DateTime.Now;
                d_Shop.Creator = operater;
                CurrentDb.Shop.Add(d_Shop);
                CurrentDb.SaveChanges();
            }
            else
            {
                var isExistShop = CurrentDb.Shop.Where(m => m.MerchId == merchId && m.Id != rop.Id && m.Name == rop.Name).FirstOrDefault();
                if (isExistShop != null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "名称已存在,请使用其它");
                }

                var d_Shop = CurrentDb.Shop.Where(m => m.MerchId == merchId && m.Id == rop.Id).FirstOrDefault();
                if (d_Shop == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到对应记录");
                }

                d_Shop.Name = rop.Name;
                d_Shop.Address = rop.Address;
                d_Shop.AreaCode = rop.AreaCode;
                d_Shop.AreaName = rop.AreaName;
                d_Shop.Lat = rop.AddressPoint.Lat;
                d_Shop.Lng = rop.AddressPoint.Lng;
                d_Shop.ContactName = rop.ContactName;
                d_Shop.ContactPhone = rop.ContactPhone;
                d_Shop.ContactAddress = rop.ContactAddress;
                d_Shop.BriefDes = rop.BriefDes;
                d_Shop.IsOpen = false;
                d_Shop.DisplayImgUrls = rop.DisplayImgUrls.ToJsonString();
                d_Shop.MainImgUrl = ImgSet.GetMain_O(d_Shop.DisplayImgUrls);
                d_Shop.Mender = operater;
                d_Shop.MendTime = DateTime.Now;

                CurrentDb.SaveChanges();
            }




            MqFactory.Global.PushEventNotify(operater, AppId.MERCH, merchId, EventCode.StoreAdd, string.Format("新建店铺（{0}）成功", rop.Name));

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            return result;
        }

    }
}
