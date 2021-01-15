using LocalS.BLL.Mq;
using LocalS.BLL.Task;
using LocalS.Entity;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.BLL.Biz
{
    public class OperateLogService : BaseService
    {
        public void Handle(OperateLogModel model)
        {
            Handle(model.Operater, model.AppId, model.TrgerId, model.EventCode, model.EventRemark);

        }

        private void Handle(string operater, string appId, string trgerId, string eventCode, string eventRemark)
        {
            // string merchName = BizFactory.Merch.GetMerchName(merchId);
            // string operaterUserName = BizFactory.Merch.GetClientName(merchId, operater);

            var sysUserOperateLog = new SysUserOperateLog();
            sysUserOperateLog.Id = IdWorker.Build(IdType.NewGuid);
            sysUserOperateLog.UserId = operater;
            sysUserOperateLog.EventCode = eventCode;
            sysUserOperateLog.EventName = EventCode.GetEventName(eventCode);
            sysUserOperateLog.AppId = appId;
            sysUserOperateLog.Remark = eventRemark;
            sysUserOperateLog.CreateTime = DateTime.Now;
            sysUserOperateLog.Creator = operater;
            CurrentDb.SysUserOperateLog.Add(sysUserOperateLog);
            CurrentDb.SaveChanges();

            var merchOperateLog = new MerchOperateLog();
            merchOperateLog.Id = IdWorker.Build(IdType.NewGuid);
            merchOperateLog.AppId = appId;
            merchOperateLog.TrgerId = trgerId;
            merchOperateLog.TrgerName = "";
            merchOperateLog.OperateUserId = operater;
            merchOperateLog.OperateUserName = "";
            merchOperateLog.EventCode = eventCode;
            merchOperateLog.EventName = EventCode.GetEventName(eventCode);
            merchOperateLog.EventLevel = EventCode.GetEventLevel(eventCode);
            merchOperateLog.Remark = eventRemark;
            merchOperateLog.Creator = operater;
            merchOperateLog.CreateTime = DateTime.Now;
            CurrentDb.MerchOperateLog.Add(merchOperateLog);
            CurrentDb.SaveChanges();

        }
    }
}
