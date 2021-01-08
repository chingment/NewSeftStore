using LocalS.BLL.Biz;
using LocalS.Entity;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class ByPointService : BaseService
    {
        public CustomJsonResult Record(string operater, RopByPoint rop)
        {
            var result = new CustomJsonResult();

            var m_AppTraceLog = new BI_AppTraceLog();
            m_AppTraceLog.Id= IdWorker.Build(IdType.NewGuid);
            m_AppTraceLog.AppId = rop.AppId;
            m_AppTraceLog.Page = rop.Page;
            m_AppTraceLog.Action = rop.Action;
            m_AppTraceLog.Param = rop.Param.ToJsonString();
            m_AppTraceLog.UsrSign = rop.UsrSign;
            m_AppTraceLog.Creator = rop.UsrSign;
            m_AppTraceLog.CreateTime = DateTime.Now;

            CurrentDb.BI_AppTraceLog.Add(m_AppTraceLog);
            CurrentDb.SaveChanges();


            return result;
        }
    }
}
