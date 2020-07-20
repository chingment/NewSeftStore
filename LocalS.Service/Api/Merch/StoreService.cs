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

            ret.Id = store.Id;
            ret.Name = store.Name;
            ret.Address = store.Address;
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
                if (store.Id == storeId)
                {
                    ret.CurStore.Id = store.Id;
                    ret.CurStore.Name = store.Name;
                }

                ret.Stores.Add(new StoreModel { Id = store.Id, Name = store.Name });
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult InitManageMachine(string operater, string merchId, string storeId)
        {
            var ret = new RetStoreInitManageMachine();

            var store = BizFactory.Store.GetOne(storeId);

            ret.StoreName = store.Name;

            var merchMachines = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId).ToList();

            foreach (var merchMachine in merchMachines)
            {
                bool disabled = false;
                string value = merchMachine.MachineId;
                string label = "";
                if (string.IsNullOrEmpty(merchMachine.CurUseStoreId))
                {
                    label = string.Format("{0}(未使用)", merchMachine.Name);
                    disabled = false;
                }
                else
                {
                    var l_store = BizFactory.Store.GetOne(merchMachine.CurUseStoreId);

                    label = string.Format("{0}(店铺:[{1}]已使用)", merchMachine.Name, l_store.Name);
                    disabled = true;
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

                MqFactory.Global.PushEventNotify(operater, AppId.MERCH, merchId, "", "", EventCode.StoreAddMachine, string.Format("机器（{0}）绑定店铺（{1}）成功", merchMachine.Name, store.Name));

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

                MqFactory.Global.PushEventNotify(operater, AppId.MERCH, merchId, "", "", EventCode.StoreRemoveMachine, string.Format("机器（{0}）解绑店铺（{1}）成功", merchMachine.Name, store.Name));

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
                if (string.IsNullOrEmpty(rop.Id))
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
                    var isExist = CurrentDb.StoreKind.Where(m => m.MerchId == merchId && m.StoreId == rop.StoreId && m.Id != rop.Id && m.Name == rop.Name && m.IsDelete == false).FirstOrDefault();
                    if (isExist != null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该分类已经存在");
                    }

                    var storeKind = CurrentDb.StoreKind.Where(m => m.MerchId == merchId && m.StoreId == rop.StoreId && m.Id == rop.Id).FirstOrDefault();

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

                if (rop.IsSynElseStore)
                {
                    var stores = CurrentDb.Store.Where(m => m.MerchId == merchId && m.Id != rop.StoreId).ToList();

                    foreach (var store in stores)
                    {
                        StoreKind storeKind = null;
                        if (string.IsNullOrEmpty(oldName))
                        {
                            storeKind = CurrentDb.StoreKind.Where(m => m.MerchId == merchId && m.StoreId == store.Id && m.Name == rop.Name).FirstOrDefault();
                        }
                        else
                        {
                            storeKind = CurrentDb.StoreKind.Where(m => m.MerchId == merchId && m.StoreId == store.Id && m.Name == oldName).FirstOrDefault();
                        }

                        if (storeKind == null)
                        {
                            storeKind = new StoreKind();
                            storeKind.Id = IdWorker.Build(IdType.NewGuid);
                            storeKind.Name = rop.Name;
                            storeKind.MerchId = merchId;
                            storeKind.StoreId = store.Id;
                            storeKind.DisplayImgUrls = rop.DisplayImgUrls.ToJsonString();
                            storeKind.Description = rop.Description;
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
                            storeKind.MendTime = DateTime.Now;
                            storeKind.Mender = operater;
                            CurrentDb.SaveChanges();
                        }
                    }
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

            List<Store> stores = CurrentDb.Store.Where(m => m.MerchId == merchId && m.Id != rop.StoreId).ToList();
            if (rop.IsSynElseStore)
            {
                stores = CurrentDb.Store.Where(m => m.MerchId == merchId).ToList();
            }
            else
            {
                stores = CurrentDb.Store.Where(m => m.MerchId == merchId && m.Id == rop.StoreId).ToList();
            }

            var storeKind = CurrentDb.StoreKind.Where(m => m.Id == rop.Id).FirstOrDefault();

            foreach (var store in stores)
            {
                var storeKindSpu = CurrentDb.StoreKind.Where(m => m.MerchId == merchId && m.StoreId == store.Id && m.Name == storeKind.Name).FirstOrDefault();
                if (storeKindSpu != null)
                {
                    storeKindSpu.IsDelete = true;
                    storeKindSpu.MendTime = DateTime.Now;
                    storeKindSpu.Mender = operater;
                    CurrentDb.SaveChanges();
                }
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

                List<Store> stores = CurrentDb.Store.Where(m => m.MerchId == merchId && m.Id != rop.StoreId).ToList();
                if (rop.IsSynElseStore)
                {
                    stores = CurrentDb.Store.Where(m => m.MerchId == merchId).ToList();
                }
                else
                {
                    stores = CurrentDb.Store.Where(m => m.MerchId == merchId && m.Id == rop.StoreId).ToList();
                }


                foreach (var store in stores)
                {
                    var storeKindSpu = CurrentDb.StoreKindSpu.Where(m => m.MerchId == merchId && m.StoreId == store.Id && m.StoreKindId == rop.KindId && m.PrdProductId == rop.ProductId).FirstOrDefault();
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
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;
        }

        public CustomJsonResult GetKindSpus(string operater, string merchId, RupStoreGetKindSpus rup)
        {

            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.StoreKindSpu
                         where u.MerchId == merchId && u.StoreId == rup.StoreId
                         &&
                         u.StoreKindId == rup.KindId
                         select new { u.PrdProductId, u.CreateTime });


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
                    Id = product.Id,
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

