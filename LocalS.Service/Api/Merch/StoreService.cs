﻿using LocalS.BLL;
using LocalS.Entity;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.Merch
{
    public class StoreService : BaseDbContext
    {
        public string GetStatusText(bool isClose)
        {
            string text = "";
            if (isClose)
            {
                text = "";
            }
            else
            {
                text = "正常";
            }

            return text;
        }

        public int GetStatusValue(bool isClose)
        {
            int text = 0;
            if (isClose)
            {
                text = 2;
            }
            else
            {
                text = 1;
            }

            return text;
        }

        public CustomJsonResult GetList(string operater, string merchId, RupStoreGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.Store
                         where (rup.Name == null || u.Name.Contains(rup.Name))
                         &&
                         u.MerchId == merchId
                         select new { u.Id, u.Name, u.MainImgUrl, u.IsClose, u.BriefDes, u.Address, u.CreateTime });


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
                    Id = item.Id,
                    Name = item.Name,
                    MainImgUrl = item.MainImgUrl,
                    Address = item.Address,
                    Status = new { text = GetStatusText(item.IsClose), value = GetStatusValue(item.IsClose) },
                    CreateTime = item.CreateTime,
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);


            return result;
        }

        public CustomJsonResult InitAdd(string operater, string merchId)
        {
            var result = new CustomJsonResult();
            var ret = new RetStoreInitAdd();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);

            return result;
        }

        public CustomJsonResult Add(string operater, string merchId, RopStoreAdd rop)
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
                store.Id = GuidUtil.New();
                store.MerchId = merchId;
                store.Name = rop.Name;
                store.Address = rop.Address;
                store.BriefDes = rop.BriefDes;
                store.IsClose = true;
                store.DispalyImgUrls = rop.DispalyImgUrls.ToJsonString();
                store.MainImgUrl = ImgSet.GetMain(store.DispalyImgUrls);
                store.CreateTime = DateTime.Now;
                store.Creator = operater;
                CurrentDb.Store.Add(store);


                //默认 快递商品库存
                var storeSellChannel = new StoreSellChannel();
                storeSellChannel.Id = GuidUtil.New();
                storeSellChannel.Name = "快递商品";
                storeSellChannel.MerchId = merchId;
                storeSellChannel.StoreId = store.Id;
                storeSellChannel.RefType = E_StoreSellChannelRefType.Express;
                storeSellChannel.RefId = GuidUtil.Empty();
                storeSellChannel.CreateTime = DateTime.Now;
                store.Creator = operater;


                CurrentDb.SaveChanges();
                ts.Complete();
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");

            }

            return result;
        }

        public CustomJsonResult InitEdit(string operater, string merchId, string storeId)
        {
            var result = new CustomJsonResult();

            var ret = new RetStoreInitEdit();

            var store = CurrentDb.Store.Where(m => m.Id == storeId).FirstOrDefault();

            ret.Id = store.Id;
            ret.Name = store.Name;
            ret.Address = store.Address;
            ret.BriefDes = store.BriefDes;
            ret.DispalyImgUrls = store.DispalyImgUrls.ToJsonObject<List<ImgSet>>();
            ret.IsClose = store.IsClose;
            ret.Status = new { text = GetStatusText(store.IsClose), value = GetStatusValue(store.IsClose) };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);

            return result;
        }

        public CustomJsonResult Edit(string operater, string merchId, RopStoreEdit rop)
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
                store.Address = rop.Address;
                store.BriefDes = rop.BriefDes;
                store.DispalyImgUrls = rop.DispalyImgUrls.ToJsonString();
                store.MainImgUrl = ImgSet.GetMain(store.DispalyImgUrls);
                store.IsClose = rop.IsClose;
                store.MendTime = DateTime.Now;
                store.Mender = operater;
                CurrentDb.SaveChanges();
                ts.Complete();
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");
            }
            return result;
        }

        public CustomJsonResult InitManage(string operater, string merchId, string storeId)
        {
            var ret = new RetStoreInitManage();

            var stores = CurrentDb.Store.Where(m => m.MerchId == merchId).ToList();


            foreach (var store in stores)
            {
                if (store.Id == storeId)
                {
                    ret.CurStore.Id = store.Id;
                    ret.CurStore.Name = store.Name;
                }

                ret.Stores.Add(new StoreModel { Id = store.Id, Name = store.Name });
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", ret);
        }

        public CustomJsonResult InitManageProductSkus(string operater, string merchId, string storeId)
        {
            var ret = new RetStoreInitManageProductSkus();

            var storeSellChannels = CurrentDb.StoreSellChannel.Where(m => m.MerchId == merchId && m.StoreId == storeId).OrderBy(m => m.RefType).ToList();


            ret.SellChannels.Add(new StoreSellChannelModel { Name = "全部", RefType = E_StoreSellChannelRefType.Unknow, RefId = GuidUtil.Empty() });

            foreach (var storeSellChannel in storeSellChannels)
            {
                ret.SellChannels.Add(new StoreSellChannelModel { Name = storeSellChannel.Name, RefType = storeSellChannel.RefType, RefId = storeSellChannel.RefId });
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", ret);
        }

        public CustomJsonResult GetProductSkuList(string operater, string merchId, RupStoreGetProductSkuList rup)
        {
            var result = new CustomJsonResult();


            var query = (from u in CurrentDb.StoreSellChannelStock
                         where
                         u.MerchId == merchId && u.StoreId == rup.StoreId
                         select new { u.Id, u.ProductSkuId, u.MerchId, u.StoreId, u.RefType, u.RefId, u.SalePrice, u.IsOffSell, u.LockQuantity, u.SumQuantity, u.SellQuantity });

            if (rup.RefType != E_StoreSellChannelRefType.Unknow)
            {
                query = query.Where(m => m.MerchId == merchId && m.StoreId == rup.StoreId && m.RefType == rup.RefType && m.RefId == rup.RefId);
            }

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;

            query = query.OrderByDescending(r => r.ProductSkuId).Skip(pageSize * (pageIndex)).Take(pageSize);

            List<object> olist = new List<object>();

            var list = query.ToList();
            foreach (var item in list)
            {

                var prdProduct = CurrentDb.PrdProduct.Where(m => m.Id == item.ProductSkuId).FirstOrDefault();
                if (prdProduct != null)
                {
                    var productSkuModel = new ProductSkuModel();
                    productSkuModel.Id = prdProduct.Id;
                    productSkuModel.Name = prdProduct.Name;
                    productSkuModel.DispalyImgUrls = prdProduct.DispalyImgUrls.ToJsonObject<List<ImgSet>>();
                    productSkuModel.MainImgUrl = ImgSet.GetMain(prdProduct.DispalyImgUrls);
                    productSkuModel.BriefDes = prdProduct.BriefDes;
                    productSkuModel.DetailsDes = prdProduct.DetailsDes;
                    productSkuModel.SpecDes = prdProduct.SpecDes;

                    productSkuModel.SumQuantity = item.SumQuantity;
                    productSkuModel.LockQuantity = item.LockQuantity;
                    productSkuModel.SellQuantity = item.SellQuantity;
                    productSkuModel.SalePrice = item.SalePrice;
                    productSkuModel.IsOffSell = item.IsOffSell;


                    olist.Add(productSkuModel);
                }
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }
    }
}

