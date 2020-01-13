using LocalS.BLL.Mq;
using LocalS.Entity;
using Lumos;
using Lumos.DbRelay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class OperateLogService : BaseDbContext
    {
        public CustomJsonResult Add(OperateLogModel rop)
        {
            var result = new CustomJsonResult();

            var sysUserOperateLog = new SysUserOperateLog();
            sysUserOperateLog.Id = GuidUtil.New();
            sysUserOperateLog.UserId = rop.Operater;
            sysUserOperateLog.Action = rop.Action;
            sysUserOperateLog.AppId = rop.AppId;
            sysUserOperateLog.Remark = rop.Remark;
            sysUserOperateLog.CreateTime = DateTime.Now;
            sysUserOperateLog.Creator = rop.Operater;
            CurrentDb.SysUserOperateLog.Add(sysUserOperateLog);
            CurrentDb.SaveChanges();

            switch (rop.Action)
            {
                case "Login":
                case "Logout":

                    if (rop.Parms != null)
                    {
                        var loginLogModel = rop.Parms.ToJsonObject<LoginLogModel>();

                        if (loginLogModel != null)
                        {
                            var userLoginHis = new SysUserLoginHis();
                            userLoginHis.Id = GuidUtil.New();
                            userLoginHis.UserId = rop.Operater;
                            userLoginHis.LoginFun = loginLogModel.LoginFun;
                            userLoginHis.LoginWay = loginLogModel.LoginWay;
                            userLoginHis.LoginTime = DateTime.Now;
                            userLoginHis.Result = loginLogModel.LoginResult;
                            userLoginHis.Description = rop.Remark;
                            userLoginHis.RemarkByDev = loginLogModel.RemarkByDev;
                            userLoginHis.CreateTime = DateTime.Now;
                            userLoginHis.Creator = rop.Operater;
                            CurrentDb.SysUserLoginHis.Add(userLoginHis);
                            CurrentDb.SaveChanges();
                        }
                    }
                    break;
            }

            if (!string.IsNullOrEmpty(rop.MachineId))
            {
                var machine = BizFactory.Machine.GetOne(rop.MachineId);

                var machineOperateLog = new MachineOperateLog();
                machineOperateLog.Id = GuidUtil.New();

                if (machine != null)
                {
                    machineOperateLog.MerchId = machine.MerchId;
                    machineOperateLog.StoreId = machine.StoreId;
                }

                machineOperateLog.MachineId = rop.MachineId;
                machineOperateLog.OperateUserId = rop.Operater;
                machineOperateLog.Action = rop.Action;
                machineOperateLog.Remark = rop.Remark;
                machineOperateLog.Creator = rop.Operater;
                machineOperateLog.CreateTime = DateTime.Now;


                CurrentDb.MachineOperateLog.Add(machineOperateLog);
                CurrentDb.SaveChanges();
            }


            return result;
        }
    }
}
