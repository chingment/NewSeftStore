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
            sysUserOperateLog.EventCode = rop.EventCode;
            sysUserOperateLog.AppId = rop.AppId;
            sysUserOperateLog.Remark = rop.Remark;
            sysUserOperateLog.CreateTime = DateTime.Now;
            sysUserOperateLog.Creator = rop.Operater;
            CurrentDb.SysUserOperateLog.Add(sysUserOperateLog);
            CurrentDb.SaveChanges();

            switch (rop.EventCode)
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
                            userLoginHis.AppId = rop.AppId;
                            userLoginHis.LoginAccount = loginLogModel.LoginAccount;
                            userLoginHis.LoginFun = loginLogModel.LoginFun;
                            userLoginHis.LoginWay = loginLogModel.LoginWay;
                            userLoginHis.Ip = loginLogModel.LoginIp;
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

                var merchOperateLog = new MerchOperateLog();
                merchOperateLog.Id = GuidUtil.New();
                merchOperateLog.AppId = rop.AppId;
                if (machine != null)
                {
                    merchOperateLog.MerchId = machine.MerchId;
                    merchOperateLog.StoreId = machine.StoreId;
                }

                merchOperateLog.MachineId = rop.MachineId;
                merchOperateLog.OperateUserId = rop.Operater;
                merchOperateLog.EventCode = rop.EventCode;
                merchOperateLog.EventName = EventCode.GetEventName(rop.EventCode);
                merchOperateLog.Remark = rop.Remark;
                merchOperateLog.Creator = rop.Operater;
                merchOperateLog.CreateTime = DateTime.Now;


                CurrentDb.MerchOperateLog.Add(merchOperateLog);
                CurrentDb.SaveChanges();
            }


            return result;
        }
    }
}
