using LocalS.BLL.Mq;
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
            sysUserOperateLog.OperateType = rop.Type;
            sysUserOperateLog.AppId = rop.AppId;
            sysUserOperateLog.Remark = rop.Remark;
            sysUserOperateLog.CreateTime = DateTime.Now;
            sysUserOperateLog.Creator = rop.Operater;
            CurrentDb.SysUserOperateLog.Add(sysUserOperateLog);
            CurrentDb.SaveChanges();

            return result;
        }
    }
}
