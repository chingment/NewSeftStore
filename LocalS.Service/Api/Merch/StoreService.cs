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
    public class StoreService : BaseService
    {
        public CustomJsonResult GetList(string operater, string merchId, RupStoreGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.Store
                         where (rup.Name == null || u.Name.Contains(rup.Name))
                         &&
                         u.MerchId == merchId
                         &&
                         u.IsDelete == false
                         select new { u.Id, u.Name, u.MainImgUrl, u.BriefDes, u.ContactPhone, u.ContactName, u.ContactAddress, u.CreateTime });


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
                    BriefDes = item.BriefDes,
                    ContactName = item.ContactName,
                    ContactPhone = item.ContactPhone,
                    ContactAddress = item.ContactAddress
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);


            return result;
        }

        public CustomJsonResult InitManage(string operater, string merchId, string storeId)
        {
            var ret = new RetStoreInitManage();

            var d_Stores = CurrentDb.Store.Where(m => m.MerchId == merchId).ToList();


            foreach (var d_Store in d_Stores)
            {
                if (!d_Store.IsDelete)
                {
                    if (d_Store.Id == storeId)
                    {
                        ret.CurStore = new StoreModel();
                        ret.CurStore.Id = d_Store.Id;
                        ret.CurStore.Name = d_Store.Name;
                    }

                    ret.Stores.Add(new StoreModel { Id = d_Store.Id, Name = d_Store.Name });
                }
            }

            if (ret.CurStore == null)
            {
                if (ret.Stores.Count > 0)
                {
                    ret.CurStore = ret.Stores[0];
                }
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult InitManageBaseInfo(string operater, string merchId, string storeId)
        {
            var result = new CustomJsonResult();

            var d_Store = CurrentDb.Store.Where(m => m.MerchId == merchId && m.Id == storeId).FirstOrDefault();

            var ret = new
            {

                Id = d_Store.Id,
                Name = d_Store.Name,
                ContactName = d_Store.ContactName,
                ContactAddress = d_Store.ContactAddress,
                ContactPhone = d_Store.ContactPhone,
                BriefDes = d_Store.BriefDes,
                MainImgUrl = d_Store.MainImgUrl,
                IsTestMode = d_Store.IsTestMode
            };
            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult InitManageShop(string operater, string merchId, string storeId)
        {
            var result = new CustomJsonResult();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new {  });
            return result;
        }

        public CustomJsonResult GetDevices(string operater, string merchId, RupStoreGetDevices rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.MerchDevice
                         join m in CurrentDb.Device on u.DeviceId equals m.Id into temp
                         from tt in temp.DefaultIfEmpty()
                         where
                         u.MerchId == merchId
                         &&
                         u.CurUseStoreId == rup.StoreId
                         &&
                         u.CurUseShopId == rup.ShopId
                         select new { u.Id, u.DeviceId, tt.ExIsHas, tt.Name, u.CurUseStoreId, u.IsStopUse, u.CreateTime });

            var list = query.OrderBy(m => m.IsStopUse).ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                var l_Device = CurrentDb.Device.Where(m => m.Id == item.DeviceId).FirstOrDefault();
                if (l_Device != null)
                {
                    olist.Add(new
                    {
                        Id = item.DeviceId,
                        Name = item.DeviceId,
                        MainImgUrl = l_Device.MainImgUrl
                    });
                }
            }



            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", olist);

            return result;
        }

        public CustomJsonResult GetKinds(string operater, string merchId, string storeId)
        {
            var result = new CustomJsonResult();

            var storeKinds = CurrentDb.StoreKind.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.IsDelete == false).OrderByDescending(m => m.Priority).ToList();

            List<object> objs = new List<object>();

            foreach (var storeKind in storeKinds)
            {
                objs.Add(new { Id = storeKind.Id, Name = storeKind.Name, Description = storeKind.Description, DisplayImgUrls = storeKind.DisplayImgUrls.ToJsonObject<List<ImgSet>>() });
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", objs);

            return result;
        }

        public CustomJsonResult SaveKind(string operater, string merchId, RopStoreSaveKind rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var d_Store = CurrentDb.Store.Where(m => m.Id == rop.StoreId).FirstOrDefault();

                string oldName = null;
                if (string.IsNullOrEmpty(rop.KindId))
                {
                    var storeKind = CurrentDb.StoreKind.Where(m => m.MerchId == merchId && m.StoreId == rop.StoreId && m.Name == rop.Name).FirstOrDefault();
                    if (storeKind != null)
                    {
                        if (!storeKind.IsDelete)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该分类已经存在");
                        }
                    }

                    if (storeKind == null)
                    {
                        storeKind = new StoreKind();
                        storeKind.Id = IdWorker.Build(IdType.NewGuid);
                        storeKind.Name = rop.Name;
                        storeKind.MerchId = merchId;
                        storeKind.StoreId = rop.StoreId;
                        storeKind.DisplayImgUrls = rop.DisplayImgUrls.ToJsonString();
                        storeKind.Description = rop.Description;
                        storeKind.IsDelete = false;
                        storeKind.CreateTime = DateTime.Now;
                        storeKind.Creator = operater;
                        CurrentDb.StoreKind.Add(storeKind);
                        CurrentDb.SaveChanges();
                    }
                    else
                    {
                        storeKind.Name = rop.Name;
                        storeKind.DisplayImgUrls = rop.DisplayImgUrls.ToJsonString();
                        storeKind.Description = rop.Description;
                        storeKind.IsDelete = false;
                        storeKind.MendTime = DateTime.Now;
                        storeKind.Mender = operater;
                        CurrentDb.SaveChanges();
                    }
                }
                else
                {
                    var isExist = CurrentDb.StoreKind.Where(m => m.MerchId == merchId && m.StoreId == rop.StoreId && m.Id != rop.KindId && m.Name == rop.Name && m.IsDelete == false).FirstOrDefault();
                    if (isExist != null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该分类已经存在");
                    }

                    var storeKind = CurrentDb.StoreKind.Where(m => m.MerchId == merchId && m.StoreId == rop.StoreId && m.Id == rop.KindId).FirstOrDefault();

                    if (storeKind == null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到对应信息");
                    }

                    oldName = storeKind.Name;

                    storeKind.Name = rop.Name;
                    storeKind.DisplayImgUrls = rop.DisplayImgUrls.ToJsonString();
                    storeKind.Description = rop.Description;
                    storeKind.MendTime = DateTime.Now;
                    storeKind.Mender = operater;
                    CurrentDb.SaveChanges();

                }

                CurrentDb.SaveChanges();
                ts.Complete();


                MqFactory.Global.PushOperateLog(operater, AppId.MERCH, merchId, EventCode.store_save_kind, string.Format("店铺：{0}，分类：{1}，保存成功", d_Store.Name, rop.Name), rop);


                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;
        }

        public CustomJsonResult RemoveKind(string operater, string merchId, RopStoreRemoveKind rop)
        {
            var result = new CustomJsonResult();
            var storeKindSpu = CurrentDb.StoreKind.Where(m => m.Id == rop.KindId && m.StoreId == rop.StoreId).FirstOrDefault();
            if (storeKindSpu == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "查找数据失败");
            }

            if (storeKindSpu.IsDelete)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "已被删除");
            }

            var d_Store = CurrentDb.Store.Where(m => m.Id == rop.StoreId).FirstOrDefault();
            var d_StoreKind = CurrentDb.StoreKind.Where(m => m.Id == rop.KindId).FirstOrDefault();
            storeKindSpu.IsDelete = true;
            storeKindSpu.MendTime = DateTime.Now;
            storeKindSpu.Mender = operater;
            CurrentDb.SaveChanges();


            MqFactory.Global.PushOperateLog(operater, AppId.MERCH, merchId, EventCode.store_remove_kind, string.Format("店铺：{0}，分类：{1}，移除成功", d_Store.Name, d_StoreKind.Name), rop);


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "删除成功");

            return result;
        }

        public CustomJsonResult SaveKindSpu(string operater, string merchId, RopStoreSaveKindSpu rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                if (string.IsNullOrEmpty(rop.StoreId))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败，请选择店铺");
                }

                if (string.IsNullOrEmpty(rop.KindId))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败，请选择分类");
                }

                if (string.IsNullOrEmpty(rop.SpuId))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败，请选择商品");
                }

                var storeKindSpu = CurrentDb.StoreKindSpu.Where(m => m.MerchId == merchId && m.StoreId == rop.StoreId && m.StoreKindId == rop.KindId && m.SpuId == rop.SpuId).FirstOrDefault();
                if (storeKindSpu == null)
                {
                    storeKindSpu = new StoreKindSpu();
                    storeKindSpu.Id = IdWorker.Build(IdType.NewGuid);
                    storeKindSpu.MerchId = merchId;
                    storeKindSpu.StoreId = rop.StoreId;
                    storeKindSpu.StoreKindId = rop.KindId;
                    storeKindSpu.SpuId = rop.SpuId;
                    storeKindSpu.IsDelete = false;
                    storeKindSpu.CreateTime = DateTime.Now;
                    storeKindSpu.Creator = operater;
                    CurrentDb.StoreKindSpu.Add(storeKindSpu);
                }
                else
                {
                    storeKindSpu.IsDelete = false;
                    storeKindSpu.MendTime = DateTime.Now;
                    storeKindSpu.Mender = operater;
                }


                if (rop.Stocks != null)
                {
                    foreach (var stock in rop.Stocks)
                    {
                        var sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == rop.StoreId && m.SkuId == stock.SkuId && m.ShopMode == E_ShopMode.Mall).FirstOrDefault();

                        if (sellChannelStock == null)
                        {
                            sellChannelStock = new SellChannelStock();
                            sellChannelStock.Id = IdWorker.Build(IdType.NewGuid);
                            sellChannelStock.MerchId = merchId;
                            sellChannelStock.StoreId = rop.StoreId;
                            sellChannelStock.ShopMode = E_ShopMode.Mall;
                            sellChannelStock.ShopId = "0";
                            sellChannelStock.DeviceId = "0";
                            sellChannelStock.CabinetId = "0";
                            sellChannelStock.SlotId = "0";
                            sellChannelStock.SpuId = rop.SpuId;
                            sellChannelStock.SkuId = stock.SkuId;
                            sellChannelStock.SalePrice = stock.SalePrice;
                            sellChannelStock.IsOffSell = stock.IsOffSell;
                            sellChannelStock.SellQuantity = stock.SumQuantity;
                            sellChannelStock.WaitPayLockQuantity = 0;
                            sellChannelStock.WaitPickupLockQuantity = 0;
                            sellChannelStock.SumQuantity = stock.SumQuantity;
                            sellChannelStock.MaxQuantity = stock.SumQuantity;
                            sellChannelStock.IsUseRent = stock.IsUseRent;
                            sellChannelStock.RentMhPrice = stock.RentMhPrice;
                            sellChannelStock.DepositPrice = stock.DepositPrice;
                            sellChannelStock.CreateTime = DateTime.Now;
                            sellChannelStock.Creator = operater;
                            CurrentDb.SellChannelStock.Add(sellChannelStock);
                        }
                        else
                        {
                            sellChannelStock.SalePrice = stock.SalePrice;
                            sellChannelStock.IsOffSell = stock.IsOffSell;
                            sellChannelStock.SumQuantity = stock.SumQuantity;
                            sellChannelStock.SellQuantity = stock.SumQuantity - sellChannelStock.WaitPayLockQuantity - sellChannelStock.WaitPickupLockQuantity;
                            sellChannelStock.MaxQuantity = stock.SumQuantity;
                            sellChannelStock.IsUseRent = stock.IsUseRent;
                            sellChannelStock.RentMhPrice = stock.RentMhPrice;
                            sellChannelStock.DepositPrice = stock.DepositPrice;
                            sellChannelStock.MendTime = DateTime.Now;
                            sellChannelStock.Mender = operater;
                        }
                    }
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;
        }

        public CustomJsonResult GetKindSpu(string operater, string merchId, RupStoreGetKindSpu rup)
        {
            var result = new CustomJsonResult();

            var d_Spu = CurrentDb.PrdSpu.Where(m => m.MerchId == merchId && m.Id == rup.SpuId).FirstOrDefault();
            var d_Skus = CurrentDb.PrdSku.Where(m => m.MerchId == merchId && m.SpuId == rup.SpuId).ToList();
            var d_StoreKind = CurrentDb.StoreKind.Where(m => m.MerchId == merchId && m.StoreId == rup.StoreId && m.Id == rup.KindId).FirstOrDefault();
            var d_StoreKindSpu = CurrentDb.StoreKindSpu.Where(m => m.MerchId == merchId && m.StoreId == rup.StoreId && m.SpuId == rup.SpuId).FirstOrDefault();

            List<object> stocks = new List<object>();

            foreach (var d_Sku in d_Skus)
            {
                var sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == rup.StoreId && m.ShopMode == E_ShopMode.Mall && m.SkuId == d_Sku.Id).FirstOrDefault();
                if (sellChannelStock == null)
                {
                    stocks.Add(new { SkuId = d_Sku.Id, CumCode = d_Sku.CumCode, SumQuantity = 10000, SpecIdx = d_Sku.SpecIdx, SalePrice = d_Sku.SalePrice, IsOffSell = false, IsUseRent = false, RentMhPrice = 0, DepositPrice = 0 });
                }
                else
                {
                    stocks.Add(new { SkuId = d_Sku.Id, CumCode = d_Sku.CumCode, SumQuantity = sellChannelStock.SumQuantity, SpecIdx = d_Sku.SpecIdx, SalePrice = sellChannelStock.SalePrice, IsOffSell = sellChannelStock.IsOffSell, IsUseRent = sellChannelStock.IsUseRent, RentMhPrice = sellChannelStock.RentMhPrice, DepositPrice = sellChannelStock.DepositPrice });
                }
            }

            var ret = new { Id = d_Spu.Id, KindName = d_StoreKind.Name, Name = d_Spu.Name, MainImgUrl = d_Spu.MainImgUrl, Stocks = stocks, IsSupRentService = d_Spu.IsSupRentService };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
            return result;
        }

        public CustomJsonResult RemoveKindSpu(string operater, string merchId, RopStoreSaveKindSpu rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                if (string.IsNullOrEmpty(rop.StoreId))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败，请选择店铺");
                }

                if (string.IsNullOrEmpty(rop.KindId))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败，请选择分类");
                }

                if (string.IsNullOrEmpty(rop.SpuId))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败，请选择商品");
                }

                var d_StoreKindSpu = CurrentDb.StoreKindSpu.Where(m => m.MerchId == merchId && m.StoreId == rop.StoreId && m.StoreKindId == rop.KindId && m.SpuId == rop.SpuId).FirstOrDefault();
                if (d_StoreKindSpu != null)
                {
                    d_StoreKindSpu.IsDelete = true;
                    d_StoreKindSpu.MendTime = DateTime.Now;
                    d_StoreKindSpu.Mender = operater;
                }

                var d_SellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == rop.StoreId && m.ShopMode == E_ShopMode.Mall && m.SpuId == rop.SpuId).ToList();

                foreach (var d_SellChannelStock in d_SellChannelStocks)
                {
                    if ((d_SellChannelStock.WaitPayLockQuantity + d_SellChannelStock.WaitPickupLockQuantity) > 0)
                    {
                        return new CustomJsonResult(ResultType.Success, ResultCode.Success, "移除失败，该商品有订单进行中");
                    }

                    CurrentDb.SellChannelStock.Remove(d_SellChannelStock);
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "移除成功");
            }

            return result;
        }

        public CustomJsonResult GetKindSpus(string operater, string merchId, RupStoreGetKindSpus rup)
        {

            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.StoreKindSpu
                         where u.MerchId == merchId && u.StoreId == rup.StoreId
                         &&
                         u.StoreKindId == rup.KindId && u.IsDelete == false
                         select new { u.StoreId, u.StoreKindId, u.SpuId, u.CreateTime });


            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                var spu = CacheServiceFactory.Product.GetSpuInfo(merchId, item.SpuId);
                olist.Add(new
                {
                    SpuId = spu.Id,
                    StoreId = item.StoreId,
                    KindId = item.StoreKindId,
                    Name = spu.Name,
                    MainImgUrl = spu.MainImgUrl,
                    CreateTime = item.CreateTime,
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);


            return result;

        }

        public CustomJsonResult GetShops(string operater, string merchId, RupStoreGetShops rup)
        {
            var result = new CustomJsonResult();

            var d_Store = CurrentDb.Store.Where(m => m.Id == rup.StoreId).FirstOrDefault();

            var query = (from s in CurrentDb.StoreShop
                         join m in CurrentDb.Shop on s.ShopId equals m.Id into temp
                         from u in temp.DefaultIfEmpty()
                         where
                         u.MerchId == merchId
                         && s.StoreId == rup.StoreId
                         select new { u.Id, u.Name, u.Address, u.MainImgUrl, u.IsOpen, u.AreaCode, u.AreaName, u.MerchId, s.StoreId, u.ContactName, u.ContactPhone, u.ContactAddress, u.CreateTime });


            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = int.MaxValue;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                var l_DeviceCount = CurrentDb.Device.Where(m => m.CurUseMerchId == merchId && m.CurUseStoreId == rup.StoreId && m.CurUseShopId == item.Id).Count();

                olist.Add(new
                {
                    Id = item.Id,
                    StoreId = item.StoreId,
                    Name = item.Name,
                    MainImgUrl = item.MainImgUrl,
                    Address = item.Address,
                    Status = MerchServiceFactory.Shop.GetStatus(item.IsOpen),
                    DeviceCount = l_DeviceCount,
                    CreateTime = item.CreateTime,
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public CustomJsonResult RemoveShop(string operater, string merchId, RopStoreRemoveShop rop)
        {
            var result = new CustomJsonResult();


            var d_StoreShopDevice_Count = CurrentDb.MerchDevice.Where(m => m.MerchId == merchId && m.CurUseStoreId == rop.StoreId && m.CurUseShopId == rop.ShopId).Count();

            if (d_StoreShopDevice_Count > 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请先解绑关联的设备");
            }

            var d_Store = CurrentDb.Store.Where(m => m.Id == rop.StoreId).FirstOrDefault();
            var d_Shop = CurrentDb.Shop.Where(m => m.Id == rop.ShopId).FirstOrDefault();
            var d_StoreShop = CurrentDb.StoreShop.Where(m => m.ShopId == rop.ShopId && m.StoreId == rop.StoreId).FirstOrDefault();

            if (d_StoreShop != null)
            {
                CurrentDb.StoreShop.Remove(d_StoreShop);
                CurrentDb.SaveChanges();
            }

            MqFactory.Global.PushOperateLog(operater, AppId.MERCH, merchId, EventCode.store_remove_shop, string.Format("将门店（{1}）从店铺（{0}）移除成功", d_Store.Name, d_Shop.Name), rop);


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "移除成功");

            return result;
        }

        public CustomJsonResult AddShop(string operater, string merchId, RopStoreAddShop rop)
        {
            var result = new CustomJsonResult();

            var d_Store = CurrentDb.Store.Where(m => m.Id == rop.StoreId).FirstOrDefault();
            var d_Shop = CurrentDb.Shop.Where(m => m.Id == rop.ShopId).FirstOrDefault();

            var d_StoreShop = CurrentDb.StoreShop.Where(m => m.ShopId == rop.ShopId && m.StoreId == rop.StoreId).FirstOrDefault();


            if (d_StoreShop == null)
            {
                d_StoreShop = new StoreShop();

                d_StoreShop.Id = IdWorker.Build(IdType.NewGuid);
                d_StoreShop.MerchId = merchId;
                d_StoreShop.StoreId = rop.StoreId;
                d_StoreShop.ShopId = rop.ShopId;
                d_StoreShop.Creator = operater;
                d_StoreShop.CreateTime = DateTime.Now;
                CurrentDb.StoreShop.Add(d_StoreShop);
                CurrentDb.SaveChanges();
            }


            MqFactory.Global.PushOperateLog(operater, AppId.MERCH, merchId, EventCode.store_add_shop, string.Format("选择门店（{0}）到店铺（{1}）添加成功", d_Store.Name, d_Shop.Name), rop);


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "添加成功");
            return result;
        }
    }
}

