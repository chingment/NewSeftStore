﻿using LocalS.BLL;
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

        public StatusModel GetStatus(E_MachineRunStatus runstatus, DateTime? lastRequestTime)
        {
            var status = new StatusModel();

            switch (runstatus)
            {
                case E_MachineRunStatus.Running:
                    status.Text = "运行中";
                    status.Value = 2;
                    break;
                case E_MachineRunStatus.Setting:
                    status.Text = "设置中";
                    status.Value = 3;
                    break;
                case E_MachineRunStatus.Stoped:
                    status.Text = "已停止";
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
                         select new { u.Id, u.MachineId, u.Name, u.StoreId, u.CreateTime });


            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = int.MaxValue;

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
                    Status = GetStatus(machine.RunStatus, machine.LastRequestTime),
                    LastRequestTime = machine.LastRequestTime,
                    CreateTime = item.CreateTime,

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
            var machine = CurrentDb.Machine.Where(m => m.Id == machineId).FirstOrDefault();

            ret.Id = merchMachine.MachineId;
            ret.Name = merchMachine.Name;
            ret.Status = GetStatus(machine.RunStatus, machine.LastRequestTime);
            ret.LastRequestTime = machine.LastRequestTime.ToUnifiedFormatDateTime();

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


        public CustomJsonResult ManageStockGetStockList(string operater, string merchId, RupMachineGetStockList rup)
        {
            var result = new CustomJsonResult();

            string[] productSkuIds = new string[] { };
            if (!string.IsNullOrEmpty(rup.ProductSkuName))
            {
                productSkuIds = CurrentDb.PrdProductSku.Where(m => m.Name.Contains(rup.ProductSkuName)).Select(m => m.Id).ToArray();
            }


            var query = (from u in CurrentDb.SellChannelStock
                         where
                         u.MerchId == merchId &&
                         u.RefType == E_SellChannelRefType.Machine &&
                         u.RefId == rup.MachineId
                         select new { u.Id, u.PrdProductSkuId, u.MerchId, u.RefType, u.SlotId, u.RefId, u.SalePrice, u.IsOffSell, u.LockQuantity, u.SumQuantity, u.SellQuantity });

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;

            if (productSkuIds.Length > 0)
            {
                query = query.Where(m => productSkuIds.Contains(m.PrdProductSkuId));
            }

            query = query.OrderByDescending(r => r.PrdProductSkuId).Skip(pageSize * (pageIndex)).Take(pageSize);

            List<object> olist = new List<object>();

            var list = query.ToList();
            foreach (var item in list)
            {
                var bizProductSku = CacheServiceFactory.ProductSku.GetInfoAndStock(item.MerchId, new string[] { rup.MachineId }, item.PrdProductSkuId);
                if (bizProductSku != null)
                {
                    olist.Add(new
                    {
                        Id = bizProductSku.Id,
                        Name = bizProductSku.Name,
                        DisplayImgUrls = bizProductSku.DisplayImgUrls,
                        MainImgUrl = bizProductSku.MainImgUrl,
                        BriefDes = bizProductSku.BriefDes,
                        DetailsDes = bizProductSku.DetailsDes,
                        SumQuantity = item.SumQuantity,
                        LockQuantity = item.LockQuantity,
                        SellQuantity = item.SellQuantity,
                        SalePrice = item.SalePrice,
                        IsOffSell = item.IsOffSell,
                        SlotId = item.SlotId
                    });
                }
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }


        public CustomJsonResult ManageStockGetStockList2(string operater, string merchId, string machineId)
        {
            var result = new CustomJsonResult();

            var machine = BizFactory.Machine.GetOne(machineId);

            List<object> olist = new List<object>();

            var machineStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.RefType == E_SellChannelRefType.Machine && m.RefId == machineId).ToList();

            List<SlotRowModel> rows = new List<SlotRowModel>();

            for (int i = machine.CabinetMaxRow_1; i > 0; i--)
            {
                SlotRowModel row = new SlotRowModel();
                row.No = i;

                for (int j = 0; j < machine.CabinetMaxCol_1; i++)
                {
                    var slotId = "n1" + "r" + i + "c" + j;

                    var col = new SlotColModel();
                    col.No = j;
                    col.SlotId = slotId;

                    var slotStock = machineStocks.Where(m => m.SlotId == slotId).FirstOrDefault();
                    if (slotStock == null)
                    {
                        col.SlotInfo.Id = slotId;
                    }
                    else
                    {
                        var bizProductSku = CacheServiceFactory.ProductSku.GetInfo(merchId, slotStock.PrdProductSkuId);
                        if (bizProductSku != null)
                        {
                            col.SlotInfo.Id = slotId;
                            col.SlotInfo.ProductSkuId = bizProductSku.Id;
                            col.SlotInfo.ProductSkuName = bizProductSku.Name;
                            col.SlotInfo.ProductSkuMainImgUrl = bizProductSku.MainImgUrl;
                            col.SlotInfo.SumQuantity = slotStock.SumQuantity;
                            col.SlotInfo.LockQuantity = slotStock.LockQuantity;
                            col.SlotInfo.SellQuantity = slotStock.SellQuantity;
                            col.SlotInfo.MaxQuantity = 10;
                        }
                    }
                    row.Cols.Add(col);
                }
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", rows);

            return result;
        }

        public CustomJsonResult ManageStockEditStock(string operater, string merchId, RopMachineEditStock rop)
        {
            var result = new CustomJsonResult();

            result = BizFactory.ProductSku.OperateStock(operater, merchId, rop.ProductSkuId, rop.MachineId, rop.SlotId, rop.SellQuantity, rop.LockQuantity, rop.IsOffSell, rop.SalePrice);

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
                merchMachine.MendTime = DateTime.Now;
                merchMachine.Mender = operater;
                CurrentDb.SaveChanges();
                ts.Complete();
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }
            return result;
        }

    }
}
