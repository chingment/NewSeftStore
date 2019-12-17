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
    public class MachineService : BaseDbContext
    {

        public StatusModel GetStatus(string curUseStoreId, bool isStopUse, E_MachineRunStatus runstatus, DateTime? lastRequestTime)
        {
            var status = new StatusModel();

            if (isStopUse)
            {
                return new StatusModel(3, "停止使用");
            }

            if (string.IsNullOrEmpty(curUseStoreId))
            {
                return new StatusModel(3, "未绑定店铺");
            }

            switch (runstatus)
            {
                case E_MachineRunStatus.Running:
                    status.Text = "运行中";
                    status.Value = 2;
                    break;
                case E_MachineRunStatus.Setting:
                    status.Text = "维护中";
                    status.Value = 3;
                    break;
                case E_MachineRunStatus.Stoped:
                    status.Text = "停止";
                    status.Value = 1;
                    break;
                default:
                    status.Text = "未知状态";
                    status.Value = 1;
                    break;
            }

            return status;
        }

        public CustomJsonResult GetList(string operater, string merchId, RupMachineGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.MerchMachine
                         where (rup.Name == null || u.Name.Contains(rup.Name))
                         &&
                         u.MerchId == merchId
                         select new { u.Id, u.MachineId, u.Name, u.CurUseStoreId, u.IsStopUse, u.CreateTime });


            if (!string.IsNullOrEmpty(rup.StoreId))
            {
                query = query.Where(m => m.CurUseStoreId == rup.StoreId);
            }

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = int.MaxValue;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                var machine = BizFactory.Machine.GetOne(item.MachineId);

                olist.Add(new
                {
                    Id = item.MachineId,
                    Name = item.Name,
                    MainImgUrl = machine.MainImgUrl,
                    AppVersion = machine.AppVersion,
                    CtrlSdkVersion = machine.CtrlSdkVersion,
                    Status = GetStatus(item.CurUseStoreId, item.IsStopUse, machine.RunStatus, machine.LastRequestTime),
                    LastRequestTime = machine.LastRequestTime,
                    CreateTime = item.CreateTime,
                    StoreId = machine.StoreId,
                    StoreName = string.IsNullOrEmpty(machine.StoreId) ? "未绑定店铺" : machine.StoreName
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);


            return result;
        }


        public CustomJsonResult InitManage(string operater, string merchId, string machineId)
        {
            var ret = new RetMachineInitManage();

            var merchMachines = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId).ToList();


            foreach (var merchMachine in merchMachines)
            {
                if (merchMachine.MachineId == machineId)
                {
                    ret.CurMachine.Id = merchMachine.MachineId;
                    ret.CurMachine.Name = merchMachine.Name;
                }

                ret.Machines.Add(new MachineModel { Id = merchMachine.MachineId, Name = merchMachine.Name });
            }


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult InitManageBaseInfo(string operater, string merchId, string machineId)
        {
            var result = new CustomJsonResult();

            var ret = new RetMachineInitManageBaseInfo();

            var merchMachine = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId && m.MachineId == machineId).FirstOrDefault();
            var machine = BizFactory.Machine.GetOne(machineId);

            ret.Id = merchMachine.MachineId;
            ret.Name = merchMachine.Name;
            ret.LogoImgUrl = merchMachine.LogoImgUrl;
            ret.Status = GetStatus(merchMachine.CurUseStoreId, merchMachine.IsStopUse, machine.RunStatus, machine.LastRequestTime);
            ret.LastRequestTime = machine.LastRequestTime.ToUnifiedFormatDateTime();
            ret.AppVersion = machine.AppVersion;
            ret.CtrlSdkVersion = machine.CtrlSdkVersion;
            if (string.IsNullOrEmpty(machine.StoreId))
            {
                ret.StoreName = "未绑定店铺";
            }
            else
            {
                ret.StoreName = machine.StoreName;
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;

        }

        public CustomJsonResult InitManageStock(string operater, string merchId, string machineId)
        {
            var result = new CustomJsonResult();

            var ret = new RetMachineInitManageStock();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;

        }

        public CustomJsonResult ManageStockGetStocks(string operater, string merchId, string machineId, string cabinetId)
        {
            var result = new CustomJsonResult();

            var machine = BizFactory.Machine.GetOne(machineId);


            if (machine.CabinetRowColLayout_1 == null || machine.CabinetRowColLayout_1.Length == 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "识别不到机器列数");
            }

            List<object> olist = new List<object>();

            var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == machine.StoreId && m.SellChannelRefType == E_SellChannelRefType.Machine && m.SellChannelRefId == machineId).ToList();

            List<SlotRowModel> rows = new List<SlotRowModel>();

            int rowsLength = machine.CabinetRowColLayout_1.Length;

            for (int i = rowsLength - 1; i >= 0; i--)
            {
                SlotRowModel row = new SlotRowModel();
                row.No = i;

                int cols = machine.CabinetRowColLayout_1[i];

                for (int j = 0; j < cols; j++)
                {
                    var slotId = string.Format("n{0}r{1}c{2}", cabinetId, i, j);

                    var col = new SlotColModel();
                    col.No = j;
                    col.SlotId = slotId;

                    var slotStock = sellChannelStocks.Where(m => m.SlotId == slotId).FirstOrDefault();
                    if (slotStock != null)
                    {
                        var bizProductSku = CacheServiceFactory.ProductSku.GetInfo(merchId, slotStock.PrdProductSkuId);
                        if (bizProductSku != null)
                        {
                            col.ProductSkuId = bizProductSku.Id;
                            col.Name = bizProductSku.Name;
                            col.MainImgUrl = bizProductSku.MainImgUrl;
                            col.SumQuantity = slotStock.SumQuantity;
                            col.LockQuantity = slotStock.WaitPayLockQuantity + slotStock.WaitPickupLockQuantity;
                            col.SellQuantity = slotStock.SellQuantity;
                            col.MaxQuantity = 10;
                            col.SalePrice = slotStock.SalePrice;
                            col.IsOffSell = slotStock.IsOffSell;
                            col.Version = slotStock.Version;
                        }
                    }

                    row.Cols.Add(col);
                }

                rows.Add(row);
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", rows);

            return result;
        }

        public CustomJsonResult ManageStockEditStock(string operater, string merchId, RopMachineEditStock rop)
        {
            var result = new CustomJsonResult();

            var machine = BizFactory.Machine.GetOne(rop.MachineId);

            result = BizFactory.ProductSku.AdjustStockQuantity(operater, merchId, machine.StoreId, rop.MachineId, rop.SlotId, rop.ProductSkuId, rop.Version, rop.SumQuantity);

            return result;
        }

        public CustomJsonResult Edit(string operater, string merchId, RopMachineEdit rop)
        {
            CustomJsonResult result = new CustomJsonResult();
            using (TransactionScope ts = new TransactionScope())
            {

                var isExist = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId && m.MachineId != rop.Id && m.Name == rop.Name).FirstOrDefault();
                if (isExist != null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "名称已存在,请使用其它");
                }

                var merchMachine = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId && m.MachineId == rop.Id).FirstOrDefault();
                merchMachine.Name = rop.Name;
                merchMachine.LogoImgUrl = rop.LogoImgUrl;
                merchMachine.MendTime = DateTime.Now;
                merchMachine.Mender = operater;
                CurrentDb.SaveChanges();
                ts.Complete();
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            if (result.Result == ResultType.Success)
            {
                BizFactory.Machine.SendUpdateHomeLogo(rop.Id, rop.LogoImgUrl);
            }

            return result;
        }

    }
}
