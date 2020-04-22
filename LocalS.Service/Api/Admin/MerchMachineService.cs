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

namespace LocalS.Service.Api.Admin
{
    public class MerchMachineService : BaseDbContext
    {
        public CustomJsonResult InitGetList(string operater)
        {
            var result = new CustomJsonResult();

            var merchs = CurrentDb.Merch.ToList();

            List<object> formSelectMerchs = new List<object>();
            foreach (var merch in merchs)
            {
                formSelectMerchs.Add(new { value = merch.Id, label = merch.Name });
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { formSelectMerchs = formSelectMerchs });

            return result;
        }

        public CustomJsonResult GetList(string operater, RupMerchMachineGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.Machine
                         where (rup.Id == null || u.Id == rup.Id)
                         select new { u.Id, u.Name, u.CreateTime });


            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderBy(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                var machine = BizFactory.Machine.GetOne(item.Id);
                olist.Add(new
                {
                    Id = item.Id,
                    Name = item.Name,
                    MerchId = machine.MerchId,
                    MerchName = string.IsNullOrEmpty(machine.MerchId) == true ? "未绑定商户" : machine.MerchName,
                    CtrlSdkVersion = machine.CtrlSdkVersion,
                    AppVersion = machine.AppVersion,
                    JPushRegId = machine.JPushRegId,
                    LogoImgUrl = machine.LogoImgUrl,
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }


        public CustomJsonResult InitEdit(string operater, RupMerchMachineInitEdit rup)
        {
            var result = new CustomJsonResult();

            var machine = CurrentDb.Machine.Where(m => m.Id == rup.Id).FirstOrDefault();


            var data = new
            {
                Id = machine.Id,
                Name = machine.Name,
                ImeiId = machine.ImeiId,
                MacAddress = machine.MacAddress,
                DeviceId = machine.DeviceId,
                AppVersionCode = machine.AppVersionCode,
                AppVersionName = machine.AppVersionName,
                CtrlSdkVersionCode = machine.CtrlSdkVersionCode,
                KindIsHidden = machine.KindIsHidden,
                KindRowCellSize = machine.KindRowCellSize,
                IsTestMode = machine.IsTestMode,
                CameraByChkIsUse = machine.CameraByChkIsUse,
                CameraByJgIsUse = machine.CameraByJgIsUse,
                CameraByRlIsUse = machine.CameraByRlIsUse,
                ExIsHas = machine.ExIsHas,
                SannerIsUse = machine.SannerIsUse,
                SannerComId = machine.SannerComId,
                FingerVeinnerIsUse = machine.FingerVeinnerIsUse,
                MstVern = machine.MstVern,
                OstVern = machine.OstVern
            };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", data);

            return result;
        }

        public CustomJsonResult Edit(string operater, RopMerchMachineEdit rop)
        {
            var result = new CustomJsonResult();

            var machine = CurrentDb.Machine.Where(m => m.Id == rop.Id).FirstOrDefault();

            machine.CameraByChkIsUse = rop.CameraByChkIsUse;
            machine.CameraByJgIsUse = rop.CameraByJgIsUse;
            machine.CameraByRlIsUse = rop.CameraByRlIsUse;
            machine.ExIsHas = rop.ExIsHas;
            machine.SannerIsUse = rop.SannerIsUse;
            machine.SannerComId = rop.SannerComId;
            machine.FingerVeinnerIsUse = rop.FingerVeinnerIsUse;
            machine.MstVern = rop.MstVern;
            machine.OstVern = rop.OstVern;
            machine.KindIsHidden = rop.KindIsHidden;
            machine.KindRowCellSize = rop.KindRowCellSize;

            CurrentDb.SaveChanges();


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            return result;
        }

        public CustomJsonResult BindOnMerch(string operater, RopMerchMachineBindOnMerch rop)
        {
            CustomJsonResult result = new CustomJsonResult();
            using (TransactionScope ts = new TransactionScope())
            {
                var machine = CurrentDb.Machine.Where(m => m.Id == rop.MachineId).FirstOrDefault();
                if (machine == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该机器");
                }

                if (!string.IsNullOrEmpty(machine.CurUseMerchId))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "已绑定商户");
                }

                var merchMachine = CurrentDb.MerchMachine.Where(m => m.MerchId == rop.MerchId && m.MachineId == rop.MachineId).FirstOrDefault();
                if (merchMachine == null)
                {
                    merchMachine = new MerchMachine();
                    merchMachine.Id = GuidUtil.New();
                    merchMachine.Name = machine.Name;
                    merchMachine.MerchId = rop.MerchId;
                    merchMachine.MachineId = rop.MachineId;
                    merchMachine.LogoImgUrl = null;
                    merchMachine.IsStopUse = false;
                    merchMachine.CreateTime = DateTime.Now;
                    merchMachine.Creator = operater;
                    CurrentDb.MerchMachine.Add(merchMachine);
                }
                else
                {
                    merchMachine.IsStopUse = false;
                    merchMachine.Mender = operater;
                    merchMachine.MendTime = DateTime.Now;
                }

                machine.CurUseMerchId = rop.MerchId;
                machine.CurUseStoreId = null;
                machine.Mender = operater;
                machine.MendTime = DateTime.Now;


                var machineBindLog = new MachineBindLog();
                machineBindLog.Id = GuidUtil.New();
                machineBindLog.MachineId = rop.MachineId;
                machineBindLog.MerchId = rop.MerchId;
                machineBindLog.StoreId = null;
                machineBindLog.BindType = E_MachineBindType.BindOnMerch;
                machineBindLog.CreateTime = DateTime.Now;
                machineBindLog.Creator = operater;
                machineBindLog.RemarkByDev = "绑定商户";
                CurrentDb.MachineBindLog.Add(machineBindLog);

                CurrentDb.SaveChanges();
                ts.Complete();
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "绑定成功");
            }
            return result;
        }

        public CustomJsonResult BindOffMerch(string operater, RopMerchMachineBindOffMerch rop)
        {
            CustomJsonResult result = new CustomJsonResult();
            using (TransactionScope ts = new TransactionScope())
            {
                var machine = CurrentDb.Machine.Where(m => m.Id == rop.MachineId).FirstOrDefault();

                if (machine == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该机器");
                }

                var merchMachine = CurrentDb.MerchMachine.Where(m => m.MerchId == rop.MerchId && m.MachineId == rop.MachineId).FirstOrDefault();
                if (merchMachine != null)
                {
                    merchMachine.IsStopUse = true;
                    merchMachine.Mender = operater;
                    merchMachine.MendTime = DateTime.Now;
                }

                var machineBindLog = new MachineBindLog();
                machineBindLog.Id = GuidUtil.New();
                machineBindLog.MachineId = rop.MachineId;
                machineBindLog.MerchId = machine.CurUseMerchId;
                machineBindLog.StoreId = machine.CurUseStoreId;
                machineBindLog.BindType = E_MachineBindType.BindOnMerch;
                machineBindLog.CreateTime = DateTime.Now;
                machineBindLog.Creator = operater;
                machineBindLog.RemarkByDev = "解绑商户";
                CurrentDb.MachineBindLog.Add(machineBindLog);

                machine.CurUseMerchId = null;
                machine.CurUseStoreId = null;
                machine.Mender = operater;
                machine.MendTime = DateTime.Now;

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "移除成功");
            }
            return result;
        }
    }
}
