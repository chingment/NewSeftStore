using LocalS.BLL;
using LocalS.BLL.Biz;
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
                         select new { u.Id, u.Name, u.MainImgUrl, u.IsOpen, u.BriefDes, u.Address, u.CreateTime });


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
                    Status = GetStatus(item.IsOpen),
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

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

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
                store.IsOpen = false;
                store.DisplayImgUrls = rop.DisplayImgUrls.ToJsonString();
                store.MainImgUrl = ImgSet.GetMain(store.DisplayImgUrls);
                store.CreateTime = DateTime.Now;
                store.Creator = operater;
                CurrentDb.Store.Add(store);
                CurrentDb.SaveChanges();
                ts.Complete();
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            }

            return result;
        }

        public CustomJsonResult InitManageBaseInfo(string operater, string merchId, string storeId)
        {
            var result = new CustomJsonResult();

            var ret = new RetStoreInitManageBaseInfo();

            var store = CurrentDb.Store.Where(m => m.Id == storeId).FirstOrDefault();

            ret.Id = store.Id;
            ret.Name = store.Name;
            ret.Address = store.Address;
            ret.BriefDes = store.BriefDes;
            ret.DisplayImgUrls = store.DisplayImgUrls.ToJsonObject<List<ImgSet>>();
            ret.IsOpen = store.IsOpen;
            ret.Status = GetStatus(store.IsOpen);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

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
                store.DisplayImgUrls = rop.DisplayImgUrls.ToJsonString();
                store.MainImgUrl = ImgSet.GetMain(store.DisplayImgUrls);
                store.IsOpen = rop.IsOpen;
                store.MendTime = DateTime.Now;
                store.Mender = operater;
                CurrentDb.SaveChanges();
                ts.Complete();
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
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

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult InitManageProduct(string operater, string merchId, string storeId)
        {
            var ret = new RetStoreInitManageProduct();

            //var storeSellChannels = CurrentDb.StoreSellChannel.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.RefType == E_StoreSellChannelRefType.Machine).OrderBy(m => m.RefType).ToList();

            //foreach (var storeSellChannel in storeSellChannels)
            //{
            //    ret.SellChannels.Add(new StoreSellChannelModel { Name = storeSellChannel.Name, RefType = storeSellChannel.RefType, RefId = storeSellChannel.RefId });
            //}

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult ManageProductGetProductList(string operater, string merchId, RupStoreManageProductGetProductList rup)
        {
            var result = new CustomJsonResult();


            var query = (from u in CurrentDb.SellChannelStock
                         where
                         u.MerchId == merchId 
                         select new { u.Id, u.PrdProductSkuId, u.MerchId,  u.RefType, u.RefId, u.SalePrice, u.IsOffSell, u.LockQuantity, u.SumQuantity, u.SellQuantity });

            if (!string.IsNullOrEmpty(rup.SellChannelRefId))
            {
                query = query.Where(m => m.MerchId == merchId && m.RefType == E_SellChannelRefType.Machine && m.RefId == rup.SellChannelRefId);
            }
            else
            {
                query = query.Where(m => m.MerchId == merchId && m.RefType == E_SellChannelRefType.Machine);
            }

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;

            query = query.OrderByDescending(r => r.PrdProductSkuId).Skip(pageSize * (pageIndex)).Take(pageSize);

            List<object> olist = new List<object>();

            var list = query.ToList();
            foreach (var item in list)
            {
                var prdProductSku = CacheServiceFactory.ProductSku.GetInfo(item.MerchId,item.PrdProductSkuId);
                if (prdProductSku != null)
                {
                    var productSkuModel = new ProductSkuModel();
                    productSkuModel.Id = prdProductSku.Id;
                    productSkuModel.Name = prdProductSku.Name;
                    productSkuModel.DisplayImgUrls = prdProductSku.DisplayImgUrls.ToJsonObject<List<ImgSet>>();
                    productSkuModel.MainImgUrl = prdProductSku.MainImgUrl;
                    productSkuModel.BriefDes = prdProductSku.BriefDes;
                    productSkuModel.DetailsDes = prdProductSku.DetailsDes;
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


        public CustomJsonResult InitManageMachine(string operater, string merchId, string storeId)
        {
            var ret = new RetStoreInitManageMachine();

            var store = CurrentDb.Store.Where(m => m.Id == storeId).FirstOrDefault();

            ret.StoreName = store.Name;

            var merchMachines = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId).ToList();

            foreach (var merchMachine in merchMachines)
            {
                bool disabled = false;
                string value = merchMachine.MachineId;
                string label = "";
                if (string.IsNullOrEmpty(merchMachine.StoreId))
                {
                    label = string.Format("{0}(未使用)", merchMachine.Name);
                    disabled = false;
                }
                else
                {
                    var l_store = CurrentDb.Store.Where(m => m.Id == merchMachine.StoreId).FirstOrDefault();
                    label = string.Format("{0}(店铺:[{1}]已使用)", merchMachine.Name, l_store.Name);
                    disabled = true;
                }

                ret.FormSelectMachines.Add(new { value = value, label = label, disabled = disabled });
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult ManageMachineGetMachineList(string operater, string merchId, RupStoreManageMachineGetMachineList rup)
        {
            var result = new CustomJsonResult();


            var query = (from u in CurrentDb.MerchMachine
                         where
                         u.MerchId == merchId && u.StoreId == rup.StoreId
                         select new { u.MerchId, u.MachineId, u.Name, u.StoreId, u.CreateTime });

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);
            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                var machine = CurrentDb.Machine.Where(m => m.Id == item.MachineId).FirstOrDefault();

                olist.Add(new
                {
                    Id = item.MachineId,
                    Name = item.Name,
                    MainImgUrl = machine.MainImgUrl,
                    Status = MerchServiceFactory.Machine.GetStatus(),
                    CreateTime = item.CreateTime,
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public CustomJsonResult AddMachine(string operater, string merchId, RopStoreAddMachine rop)
        {
            CustomJsonResult result = new CustomJsonResult();
            using (TransactionScope ts = new TransactionScope())
            {
                var merchMachine = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId && m.MachineId == rop.MachineId).FirstOrDefault();
                if (merchMachine == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到商户的机器");
                }

                if (!string.IsNullOrEmpty(merchMachine.StoreId))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "已被使用");
                }

                merchMachine.StoreId = rop.StoreId;
                merchMachine.Mender = operater;
                merchMachine.MendTime = DateTime.Now;

                CurrentDb.SaveChanges();
                ts.Complete();
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "添加成功");
            }
            return result;
        }

        public CustomJsonResult RemoveMachine(string operater, string merchId, RopStoreRemoveMachine rop)
        {
            CustomJsonResult result = new CustomJsonResult();
            using (TransactionScope ts = new TransactionScope())
            {
                var merchMachine = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId && m.MachineId == rop.MachineId).FirstOrDefault();
                if (merchMachine == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到商户的机器");
                }

                if (string.IsNullOrEmpty(merchMachine.StoreId))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "已被移除");
                }

                merchMachine.StoreId = null;
                merchMachine.Mender = operater;
                merchMachine.MendTime = DateTime.Now;
                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "移除成功");
            }
            return result;
        }
    }
}

