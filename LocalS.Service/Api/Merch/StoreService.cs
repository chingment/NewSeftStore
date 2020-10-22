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
                         &&
                         u.IsDelete == false
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
                store.Id = IdWorker.Build(IdType.NewGuid);
                store.MerchId = merchId;
                store.Name = rop.Name;
                store.Address = rop.Address;
                store.BriefDes = rop.BriefDes;
                store.IsOpen = false;
                store.DisplayImgUrls = rop.DisplayImgUrls.ToJsonString();
                store.MainImgUrl = ImgSet.GetMain_O(store.DisplayImgUrls);
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

        public CustomJsonResult InitManageBaseInfo(string operater, string merchId, string storeId)
        {
            var result = new CustomJsonResult();

            var ret = new RetStoreInitManageBaseInfo();

            var store = BizFactory.Store.GetOne(storeId);

            ret.Id = store.StoreId;
            ret.Name = store.Name;
            ret.Address = store.Address;
            ret.AddressPoint = store.AddressPoint;
            ret.BriefDes = store.BriefDes;
            ret.DisplayImgUrls = store.DisplayImgUrls;
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
                store.MainImgUrl = ImgSet.GetMain_O(store.DisplayImgUrls);
                store.IsOpen = rop.IsOpen;
                store.Lat = rop.AddressPoint.Lat;
                store.Lng = rop.AddressPoint.Lng;
                store.MendTime = DateTime.Now;
                store.Mender = operater;
                CurrentDb.SaveChanges();
                ts.Complete();

                MqFactory.Global.PushEventNotify(operater, AppId.MERCH, merchId, "", "", EventCode.StoreEdit, string.Format("保存店铺（{0}）信息成功", rop.Name));

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }
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
                        ret.CurStore.Id = store.StoreId;
                        ret.CurStore.Name = store.Name;
                    }

                    ret.Stores.Add(new StoreModel { Id = store.StoreId, Name = store.Name });
                }
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult InitManageMachine(string operater, string merchId, string storeId)
        {
            var ret = new RetStoreInitManageMachine();

            var store = BizFactory.Store.GetOne(storeId);

            ret.StoreName = store.Name;

            var merchMachines = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId).OrderBy(m => m.CurUseStoreId).ToList();

            foreach (var merchMachine in merchMachines)
            {
                bool disabled = false;
                string value = merchMachine.MachineId;
                string label = "";

                if (merchMachine.IsStopUse)
                {
                    label = string.Format("{0}(停止使用)", merchMachine.MachineId);
                    disabled = true;
                }
                else
                {

                    if (string.IsNullOrEmpty(merchMachine.CurUseStoreId))
                    {
                        label = string.Format("{0}(未使用)", merchMachine.MachineId);
                        disabled = false;
                    }
                    else
                    {
                        var l_store = BizFactory.Store.GetOne(merchMachine.CurUseStoreId);

                        label = string.Format("{0}(店铺:[{1}]已使用)", merchMachine.MachineId, l_store.Name);
                        disabled = true;
                    }
                }

                ret.FormSelectMachines.Add(new { value = value, label = label, disabled = disabled });
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult ManageMachineGetMachineList(string operater, string merchId, RupStoreManageMachineGetMachineList rup)
        {
            return MerchServiceFactory.Machine.GetList(operater, merchId, new RupMachineGetList { Limit = rup.Limit, Page = rup.Page, StoreId = rup.StoreId });
        }

        public CustomJsonResult AddMachine(string operater, string merchId, RopStoreAddMachine rop)
        {
            CustomJsonResult result = new CustomJsonResult();
            using (TransactionScope ts = new TransactionScope())
            {
                var machine = CurrentDb.Machine.Where(m => m.Id == rop.MachineId).FirstOrDefault();
                if (machine == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该机器");
                }

                if (machine.CurUseMerchId != merchId)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该机器不是对应商户");
                }

                var merchMachine = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId && m.MachineId == rop.MachineId).FirstOrDefault();
                if (merchMachine == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到商户的机器");
                }

                if (!string.IsNullOrEmpty(merchMachine.CurUseStoreId))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "已被使用");
                }

                var store = CurrentDb.Store.Where(m => m.Id == rop.StoreId).FirstOrDefault();


                machine.CurUseStoreId = rop.StoreId;
                machine.Mender = operater;
                machine.MendTime = DateTime.Now;

                merchMachine.CurUseStoreId = rop.StoreId;
                merchMachine.Mender = operater;
                merchMachine.MendTime = DateTime.Now;


                var machineBindLog = new MachineBindLog();
                machineBindLog.Id = IdWorker.Build(IdType.NewGuid);
                machineBindLog.MachineId = rop.MachineId;
                machineBindLog.MerchId = merchId;
                machineBindLog.StoreId = rop.StoreId;
                machineBindLog.BindType = E_MachineBindType.BindOnStore;
                machineBindLog.CreateTime = DateTime.Now;
                machineBindLog.Creator = operater;
                machineBindLog.RemarkByDev = "绑定店铺";
                CurrentDb.MachineBindLog.Add(machineBindLog);

                CurrentDb.SaveChanges();
                ts.Complete();

                MqFactory.Global.PushEventNotify(operater, AppId.MERCH, merchId, "", "", EventCode.StoreAddMachine, string.Format("机器（{0}）绑定店铺（{1}）成功", merchMachine.MachineId, store.Name));

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "添加成功");
            }
            return result;
        }

        public CustomJsonResult RemoveMachine(string operater, string merchId, RopStoreRemoveMachine rop)
        {
            CustomJsonResult result = new CustomJsonResult();
            using (TransactionScope ts = new TransactionScope())
            {
                var machine = CurrentDb.Machine.Where(m => m.Id == rop.MachineId).FirstOrDefault();
                if (machine == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该机器");
                }

                if (machine.CurUseMerchId != merchId)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该机器不是对应商户");
                }

                var merchMachine = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId && m.MachineId == rop.MachineId).FirstOrDefault();

                if (merchMachine == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到商户的机器");
                }

                if (string.IsNullOrEmpty(merchMachine.CurUseStoreId))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "已被移除");
                }

                var store = CurrentDb.Store.Where(m => m.Id == rop.StoreId).FirstOrDefault();

                var machineBindLog = new MachineBindLog();
                machineBindLog.Id = IdWorker.Build(IdType.NewGuid);
                machineBindLog.MachineId = rop.MachineId;
                machineBindLog.MerchId = machine.CurUseMerchId;
                machineBindLog.StoreId = machine.CurUseStoreId;
                machineBindLog.BindType = E_MachineBindType.BindOffStore;
                machineBindLog.CreateTime = DateTime.Now;
                machineBindLog.Creator = operater;
                machineBindLog.RemarkByDev = "解绑店铺";
                CurrentDb.MachineBindLog.Add(machineBindLog);


                machine.CurUseStoreId = null;
                machine.Mender = operater;
                machine.MendTime = DateTime.Now;

                merchMachine.CurUseStoreId = null;
                merchMachine.Mender = operater;
                merchMachine.MendTime = DateTime.Now;

                MqFactory.Global.PushEventNotify(operater, AppId.MERCH, merchId, "", "", EventCode.StoreRemoveMachine, string.Format("机器（{0}）解绑店铺（{1}）成功", merchMachine.MachineId, store.Name));

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "移除成功");
            }
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
                    storeKindSpu.IsSellMall = rop.IsSellMall;
                    storeKindSpu.CreateTime = DateTime.Now;
                    storeKindSpu.Creator = operater;
                    CurrentDb.StoreKindSpu.Add(storeKindSpu);
                }
                else
                {
                    storeKindSpu.IsSellMall = rop.IsSellMall;
                    storeKindSpu.IsDelete = false;
                    storeKindSpu.MendTime = DateTime.Now;
                    storeKindSpu.Mender = operater;
                }


                if (rop.Stocks != null)
                {
                    foreach (var stock in rop.Stocks)
                    {
                        var sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == rop.StoreId && m.PrdProductSkuId == stock.SkuId && m.SellChannelRefType == E_SellChannelRefType.Mall).FirstOrDefault();
                        if (rop.IsSellMall)
                        {
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
                                sellChannelStock.SalePriceByVip = stock.SalePrice;
                                sellChannelStock.IsOffSell = stock.IsOffSell;
                                sellChannelStock.SellQuantity = stock.SumQuantity;
                                sellChannelStock.WaitPayLockQuantity = 0;
                                sellChannelStock.WaitPickupLockQuantity = 0;
                                sellChannelStock.SumQuantity = stock.SumQuantity;
                                sellChannelStock.MaxQuantity = stock.SumQuantity;
                                sellChannelStock.CreateTime = DateTime.Now;
                                sellChannelStock.Creator = operater;
                                CurrentDb.SellChannelStock.Add(sellChannelStock);
                            }
                            else
                            {
                                sellChannelStock.SalePrice = stock.SalePrice;
                                sellChannelStock.SalePriceByVip = stock.SalePrice;
                                sellChannelStock.IsOffSell = stock.IsOffSell;
                                sellChannelStock.SumQuantity = stock.SumQuantity;
                                sellChannelStock.SellQuantity = stock.SumQuantity - sellChannelStock.WaitPayLockQuantity - sellChannelStock.WaitPickupLockQuantity;
                                sellChannelStock.MaxQuantity = stock.SumQuantity;
                                sellChannelStock.MendTime = DateTime.Now;
                                sellChannelStock.Mender = operater;
                            }
                        }
                        else
                        {
                            if (sellChannelStock != null)
                            {
                                if ((sellChannelStock.WaitPayLockQuantity + sellChannelStock.WaitPickupLockQuantity) > 0)
                                {
                                    return new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存失败，该商品有客户进行中");
                                }

                                CurrentDb.SellChannelStock.Remove(sellChannelStock);
                            }
                        }
                    }
                }


                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;
        }

        public CustomJsonResult GetKindSpuInfo(string operater, string merchId, RupStoreGetKindSpu rup)
        {
            var result = new CustomJsonResult();

            var product = CurrentDb.PrdProduct.Where(m => m.MerchId == merchId && m.Id == rup.ProductId).FirstOrDefault();

            var productSkus = CurrentDb.PrdProductSku.Where(m => m.MerchId == merchId && m.PrdProductId == product.Id).ToList();
            var storeKindSpu = CurrentDb.StoreKindSpu.Where(m => m.MerchId == merchId && m.StoreId == rup.StoreId && m.PrdProductId == product.Id).FirstOrDefault();

            List<object> stocks = new List<object>();

            foreach (var productSku in productSkus)
            {
                var sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == rup.StoreId && m.SellChannelRefType == E_SellChannelRefType.Mall && m.PrdProductSkuId == productSku.Id).FirstOrDefault();
                if (sellChannelStock == null)
                {
                    stocks.Add(new { SkuId = productSku.Id, CumCode = productSku.CumCode, SumQuantity = 10000, SpecIdx = productSku.SpecIdx, SalePrice = productSku.SalePrice, IsOffSell = false });
                }
                else
                {
                    stocks.Add(new { SkuId = productSku.Id, CumCode = productSku.CumCode, SumQuantity = sellChannelStock.SumQuantity, SpecIdx = productSku.SpecIdx, SalePrice = sellChannelStock.SalePrice, IsOffSell = sellChannelStock.IsOffSell });
                }
            }

            var ret = new { Id = product.Id, IsSellMall = storeKindSpu.IsSellMall, Name = product.Name, MainImgUrl = product.MainImgUrl, Stocks = stocks };

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


                var storeKindSpu = CurrentDb.StoreKindSpu.Where(m => m.MerchId == merchId && m.StoreId == rop.StoreId && m.StoreKindId == rop.KindId && m.PrdProductId == rop.ProductId).FirstOrDefault();
                if (storeKindSpu != null)
                {
                    storeKindSpu.IsDelete = true;
                    storeKindSpu.MendTime = DateTime.Now;
                    storeKindSpu.Mender = operater;
                }

                var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == rop.StoreId && m.SellChannelRefType == E_SellChannelRefType.Mall && m.PrdProductId == rop.ProductId).ToList();

                foreach (var sellChannelStock in sellChannelStocks)
                {
                    if ((sellChannelStock.WaitPayLockQuantity + sellChannelStock.WaitPickupLockQuantity) > 0)
                    {
                        return new CustomJsonResult(ResultType.Success, ResultCode.Success, "移除失败，该商品有客户进行中");
                    }

                    CurrentDb.SellChannelStock.Remove(sellChannelStock);
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
    }
}

