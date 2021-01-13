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

        public CustomJsonResult GetList(string operater, string merchId, RupStoreGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.Store
                         where (rup.Name == null || u.Name.Contains(rup.Name))
                         &&
                         u.MerchId == merchId
                         &&
                         u.IsDelete == false
                         select new { u.Id, u.Name, u.SctMode, u.MainImgUrl, u.IsOpen, u.BriefDes, u.ContactAddress, u.CreateTime });


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
                    ContactAddress = item.ContactAddress,
                    Status = GetStatus(item.IsOpen),
                    SctMode = item.SctMode,
                    CreateTime = item.CreateTime,
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);


            return result;
        }

        public CustomJsonResult InitManage(string operater, string merchId, string storeId)
        {
            var ret = new RetStoreInitManage();

            var stores = BizFactory.Store.GetAll(merchId);


            foreach (var store in stores)
            {
                if (!store.IsDelete)
                {
                    if (store.StoreId == storeId)
                    {
                        ret.CurStore = new StoreModel();
                        ret.CurStore.Id = store.StoreId;
                        ret.CurStore.Name = store.Name;
                        ret.CurStore.SctMode = store.SctMode;
                    }

                    ret.Stores.Add(new StoreModel { Id = store.StoreId, Name = store.Name, SctMode = store.SctMode });
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

            var ret = new RetStoreInitManageBaseInfo();

            var d_store = CurrentDb.Store.Where(m => m.MerchId == merchId && m.Id == storeId).FirstOrDefault();

            ret.Id = d_store.Id;
            ret.Name = d_store.Name;
            ret.ContactName = d_store.ContactName;
            ret.ContactAddress = d_store.ContactAddress;
            ret.ContactPhone = d_store.ContactPhone;
            ret.Status = GetStatus(d_store.IsOpen);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult InitManageShop(string operater, string merchId, string storeId)
        {
            var result = new CustomJsonResult();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
            return result;
        }

        public CustomJsonResult GetMachines(string operater, string merchId, RupStoreGetMachines rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.MerchMachine
                         join m in CurrentDb.Machine on u.MachineId equals m.Id into temp
                         from tt in temp.DefaultIfEmpty()
                         where
                         u.MerchId == merchId
                         &&
                         u.CurUseStoreId == rup.StoreId
                         &&
                         u.CurUseShopId == rup.ShopId
                         select new { u.Id, u.MachineId, tt.ExIsHas, u.Name, u.CurUseStoreId, u.IsStopUse, u.CreateTime });

            var list = query.OrderBy(m => m.IsStopUse).ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                var machine = CurrentDb.Machine.Where(m => m.Id == item.MachineId).FirstOrDefault();
                if (machine != null)
                {
                    olist.Add(new
                    {
                        Id = item.MachineId,
                        Name = item.MachineId,
                        MainImgUrl = machine.MainImgUrl
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

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;
        }

        public CustomJsonResult RemoveKind(string operater, string merchId, RopStoreRemoveKind rop)
        {
            var result = new CustomJsonResult();
            var storeKindSpu = CurrentDb.StoreKind.Where(m => m.Id == rop.KindId && m.StoreId == rop.StoreId).FirstOrDefault();
            if (storeKindSpu != null)
            {
                storeKindSpu.IsDelete = true;
                storeKindSpu.MendTime = DateTime.Now;
                storeKindSpu.Mender = operater;
                CurrentDb.SaveChanges();
            }

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

                if (string.IsNullOrEmpty(rop.ProductId))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败，请选择商品");
                }

                var storeKindSpu = CurrentDb.StoreKindSpu.Where(m => m.MerchId == merchId && m.StoreId == rop.StoreId && m.StoreKindId == rop.KindId && m.PrdProductId == rop.ProductId).FirstOrDefault();
                if (storeKindSpu == null)
                {
                    storeKindSpu = new StoreKindSpu();
                    storeKindSpu.Id = IdWorker.Build(IdType.NewGuid);
                    storeKindSpu.MerchId = merchId;
                    storeKindSpu.StoreId = rop.StoreId;
                    storeKindSpu.StoreKindId = rop.KindId;
                    storeKindSpu.PrdProductId = rop.ProductId;
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
                        var sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == rop.StoreId && m.PrdProductSkuId == stock.SkuId && m.SellChannelRefType == E_SellChannelRefType.Mall).FirstOrDefault();

                        if (sellChannelStock == null)
                        {
                            sellChannelStock = new SellChannelStock();
                            sellChannelStock.Id = IdWorker.Build(IdType.NewGuid);
                            sellChannelStock.MerchId = merchId;
                            sellChannelStock.StoreId = rop.StoreId;
                            sellChannelStock.SellChannelRefType = E_SellChannelRefType.Mall;
                            sellChannelStock.SellChannelRefId = SellChannelStock.MallSellChannelRefId;
                            sellChannelStock.CabinetId = "0";
                            sellChannelStock.SlotId = "0";
                            sellChannelStock.PrdProductId = rop.ProductId;
                            sellChannelStock.PrdProductSkuId = stock.SkuId;
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

            var d_Product = CurrentDb.PrdProduct.Where(m => m.MerchId == merchId && m.Id == rup.ProductId).FirstOrDefault();
            var d_ProductSkus = CurrentDb.PrdProductSku.Where(m => m.MerchId == merchId && m.PrdProductId == rup.ProductId).ToList();
            var d_StoreKind = CurrentDb.StoreKind.Where(m => m.MerchId == merchId && m.StoreId == rup.StoreId && m.Id == rup.KindId).FirstOrDefault();
            var d_StoreKindSpu = CurrentDb.StoreKindSpu.Where(m => m.MerchId == merchId && m.StoreId == rup.StoreId && m.PrdProductId == rup.ProductId).FirstOrDefault();

            List<object> stocks = new List<object>();

            foreach (var d_ProductSku in d_ProductSkus)
            {
                var sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == rup.StoreId && m.SellChannelRefType == E_SellChannelRefType.Mall && m.PrdProductSkuId == d_ProductSku.Id).FirstOrDefault();
                if (sellChannelStock == null)
                {
                    stocks.Add(new { SkuId = d_ProductSku.Id, CumCode = d_ProductSku.CumCode, SumQuantity = 10000, SpecIdx = d_ProductSku.SpecIdx, SalePrice = d_ProductSku.SalePrice, IsOffSell = false, IsUseRent = false, RentMhPrice = 0, DepositPrice = 0 });
                }
                else
                {
                    stocks.Add(new { SkuId = d_ProductSku.Id, CumCode = d_ProductSku.CumCode, SumQuantity = sellChannelStock.SumQuantity, SpecIdx = d_ProductSku.SpecIdx, SalePrice = sellChannelStock.SalePrice, IsOffSell = sellChannelStock.IsOffSell, IsUseRent = sellChannelStock.IsUseRent, RentMhPrice = sellChannelStock.RentMhPrice, DepositPrice = sellChannelStock.DepositPrice });
                }
            }

            var ret = new { Id = d_Product.Id, KindName = d_StoreKind.Name, Name = d_Product.Name, MainImgUrl = d_Product.MainImgUrl, Stocks = stocks, IsSupRentService = d_Product.IsSupRentService };

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

                if (string.IsNullOrEmpty(rop.ProductId))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败，请选择商品");
                }

                var d_StoreKindSpu = CurrentDb.StoreKindSpu.Where(m => m.MerchId == merchId && m.StoreId == rop.StoreId && m.StoreKindId == rop.KindId && m.PrdProductId == rop.ProductId).FirstOrDefault();
                if (d_StoreKindSpu != null)
                {
                    d_StoreKindSpu.IsDelete = true;
                    d_StoreKindSpu.MendTime = DateTime.Now;
                    d_StoreKindSpu.Mender = operater;
                }

                var d_SellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == rop.StoreId && m.SellChannelRefType == E_SellChannelRefType.Mall && m.PrdProductId == rop.ProductId).ToList();

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
                         select new { u.StoreId, u.StoreKindId, u.PrdProductId, u.CreateTime });


            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                var product = CacheServiceFactory.Product.GetSpuInfo(merchId, item.PrdProductId);
                olist.Add(new
                {
                    ProductId = product.Id,
                    StoreId = item.StoreId,
                    KindId = item.StoreKindId,
                    Name = product.Name,
                    MainImgUrl = product.MainImgUrl,
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
                var machineCount = CurrentDb.Machine.Where(m => m.CurUseMerchId == merchId && m.CurUseStoreId == rup.StoreId && m.CurUseShopId == item.Id).Count();

                olist.Add(new
                {
                    Id = item.Id,
                    StoreId = item.StoreId,
                    Name = item.Name,
                    MainImgUrl = item.MainImgUrl,
                    Address = item.Address,
                    Status = GetStatus(item.IsOpen),
                    MachineCount = machineCount,
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

            var d_StoreShop = CurrentDb.StoreShop.Where(m => m.ShopId == rop.ShopId && m.StoreId == rop.StoreId).FirstOrDefault();

            if (d_StoreShop != null)
            {
                CurrentDb.StoreShop.Remove(d_StoreShop);
                CurrentDb.SaveChanges();
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "移除成功");

            return result;
        }

        public CustomJsonResult AddShop(string operater, string merchId, RopStoreAddShop rop)
        {
            var result = new CustomJsonResult();

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

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "添加成功");
            return result;
        }
    }
}

