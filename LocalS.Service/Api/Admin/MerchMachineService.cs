using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Entity;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.Admin
{
    public class MerchMachineService : BaseService
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

            var machineCabinets = CurrentDb.MachineCabinet.Where(m => m.MachineId == rup.Id).ToList();

            List<object> cabinets = new List<object>();

            foreach (var machineCabinet in machineCabinets)
            {
                string pendantRows = "";

                if (machineCabinet.CabinetId.Contains("ds"))
                {
                    var rowlayout = machineCabinet.RowColLayout.ToJsonObject<CabinetRowColLayoutByDSModel>();
                    if (rowlayout != null)
                    {
                        pendantRows = rowlayout.PendantRows.ToJsonString();
                    }
                }

                cabinets.Add(new { Id = machineCabinet.CabinetId, Name = machineCabinet.CabinetName, ComId = machineCabinet.ComId, IsUse = machineCabinet.IsUse, PendantRows = pendantRows });
            }

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
                OstVern = machine.OstVern,
                Cabinets = cabinets,
                ImIsUse = machine.ImIsUse
            };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", data);

            return result;
        }

        public CustomJsonResult Edit(string operater, RopMerchMachineEdit rop)
        {
            var result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
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
                machine.ImIsUse = rop.ImIsUse;

                if (machine.ImIsUse)
                {
                    if (string.IsNullOrEmpty(machine.ImUserName))
                    {
                        machine.ImPartner = "Em";
                        machine.ImUserName = string.Format("MH_{0}", machine.Id);
                        machine.ImPassword = "1a2b3c4d";
                        var var1 = SdkFactory.Easemob.RegisterUser(machine.ImUserName, machine.ImPassword, machine.Id);
                        if (var1.Result != ResultType.Success)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败，音频服务存在问题");
                        }
                    }
                }

                foreach (var cabinet in rop.Cabinets)
                {
                    var machineCabinet = CurrentDb.MachineCabinet.Where(m => m.CabinetId == cabinet.Id && m.MachineId == rop.Id).FirstOrDefault();
                    if (machineCabinet != null)
                    {
                        machineCabinet.ComId = cabinet.ComId;
                        machineCabinet.IsUse = cabinet.IsUse;
                        if (machineCabinet.CabinetId.StartsWith("ds"))
                        {
                            if (!string.IsNullOrEmpty(machineCabinet.RowColLayout))
                            {
                                var rowColLayout = machineCabinet.RowColLayout.ToJsonObject<CabinetRowColLayoutByDSModel>();
                                if (rowColLayout != null)
                                {
                                    rowColLayout.PendantRows = cabinet.PendantRows.ToJsonObject<List<int>>();

                                    machineCabinet.RowColLayout = rowColLayout.ToJsonString();
                                }
                            }
                        }


                        CurrentDb.SaveChanges();
                    }

                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            }

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
                    merchMachine.Id = IdWorker.Build(IdType.NewGuid);
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
                machineBindLog.Id = IdWorker.Build(IdType.NewGuid);
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
                machineBindLog.Id = IdWorker.Build(IdType.NewGuid);
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
