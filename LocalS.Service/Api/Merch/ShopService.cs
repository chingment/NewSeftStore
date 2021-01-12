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

        public CustomJsonResult Add(string operater, string merchId, RopShopAdd rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var isExistStore = CurrentDb.Store.Where(m => m.MerchId == merchId && m.Name == rop.Name).FirstOrDefault();
                if (isExistStore != null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "名称已存在,请使用其它");
                }

                var store = new Store();
                store.Id = IdWorker.Build(IdType.NewGuid);
                store.MerchId = merchId;
                store.Name = rop.Name;
                store.ContactAddress = rop.ContactAddress;
                //store.BriefDes = rop.BriefDes;
                //store.IsOpen = false;
                //store.DisplayImgUrls = rop.DisplayImgUrls.ToJsonString();
                //store.MainImgUrl = ImgSet.GetMain_O(store.DisplayImgUrls);
                store.CreateTime = DateTime.Now;
                store.Creator = operater;
                CurrentDb.Store.Add(store);
                CurrentDb.SaveChanges();
                ts.Complete();

                MqFactory.Global.PushEventNotify(operater, AppId.MERCH, merchId, "", "", EventCode.StoreAdd, string.Format("新建店铺（{0}）成功", rop.Name));

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            }

            return result;
        }

        public CustomJsonResult Edit(string operater, string merchId, RopShopEdit rop)
        {
            CustomJsonResult result = new CustomJsonResult();
            using (TransactionScope ts = new TransactionScope())
            {

                var isExistStore = CurrentDb.Store.Where(m => m.MerchId == merchId && m.Id != rop.Id && m.Name == rop.Name).FirstOrDefault();
                if (isExistStore != null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "名称已存在,请使用其它");
                }

                var store = CurrentDb.Store.Where(m => m.Id == rop.Id).FirstOrDefault();

                store.Name = rop.Name;
                store.ContactAddress = rop.ContactAddress;
                //store.BriefDes = rop.BriefDes;
                //store.DisplayImgUrls = rop.DisplayImgUrls.ToJsonString();
                //store.MainImgUrl = ImgSet.GetMain_O(store.DisplayImgUrls);
                //store.IsOpen = rop.IsOpen;
                store.MendTime = DateTime.Now;
                store.Mender = operater;
                CurrentDb.SaveChanges();
                ts.Complete();

                MqFactory.Global.PushEventNotify(operater, AppId.MERCH, merchId, "", "", EventCode.StoreEdit, string.Format("保存店铺（{0}）信息成功", rop.Name));

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }
            return result;
        }
    }
}
