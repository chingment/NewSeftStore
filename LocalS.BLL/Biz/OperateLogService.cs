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

            var sysUserOperateLog = new SysUserOperateLog();
            sysUserOperateLog.Id = IdWorker.Build(IdType.NewGuid);
            sysUserOperateLog.UserId = model.Operater;
            sysUserOperateLog.EventCode = model.EventCode;
            sysUserOperateLog.EventName = EventCode.GetEventName(model.EventCode);
            sysUserOperateLog.AppId = model.AppId;
            sysUserOperateLog.Remark = model.EventRemark;
            sysUserOperateLog.CreateTime = DateTime.Now;
            sysUserOperateLog.Creator = model.Operater;
            CurrentDb.SysUserOperateLog.Add(sysUserOperateLog);
            CurrentDb.SaveChanges();

            string trgerName = "";
            string merchId = "";
            string merchName = "";

            if (model.AppId == AppId.MERCH)
            {
                merchId = model.TrgerId;
                merchName = BizFactory.Merch.GetMerchName(merchId);
                trgerName = merchName;
            }
            else if (model.AppId == AppId.WXMINPRAGROM)
            {
                var store = BizFactory.Store.GetOne(model.TrgerId);
                trgerName = store.Name;
                if (store != null)
                {
                    merchId = store.MerchId;
                    merchName = store.MerchName;
                }
            }
            else if (model.AppId == AppId.STORETERM)
            {
                var machine = BizFactory.Machine.GetOne(model.TrgerId);
                if (machine != null)
                {
                    merchName = machine.MerchName;
                    merchId = machine.MerchId;
                    trgerName = machine.MachineId;
                }
            }

            var merchOperateLog = new MerchOperateLog();
            merchOperateLog.Id = IdWorker.Build(IdType.NewGuid);
            merchOperateLog.AppId = model.AppId;
            merchOperateLog.TrgerId = model.TrgerId;
            merchOperateLog.TrgerName = trgerName;
            merchOperateLog.MerchId = merchId;
            merchOperateLog.MerchName = merchName;
            merchOperateLog.OperateUserId = model.Operater;
            merchOperateLog.OperateUserName = "";
            merchOperateLog.EventCode = model.EventCode;
            merchOperateLog.EventName = EventCode.GetEventName(model.EventCode);
            merchOperateLog.EventLevel = EventCode.GetEventLevel(model.EventCode);
            merchOperateLog.Remark = model.EventCode;
            merchOperateLog.Creator = model.Operater;
            merchOperateLog.CreateTime = DateTime.Now;
            CurrentDb.MerchOperateLog.Add(merchOperateLog);
            CurrentDb.SaveChanges();
        }
    }
}
