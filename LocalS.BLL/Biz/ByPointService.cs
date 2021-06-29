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

            var d_AppTraceLog = new BI_AppTraceLog();
            d_AppTraceLog.Id= IdWorker.Build(IdType.NewGuid);
            d_AppTraceLog.AppId = rop.AppId;
            d_AppTraceLog.Page = rop.Page;
            d_AppTraceLog.Action = rop.Action;
            d_AppTraceLog.Param = rop.Param.ToJsonString();
            d_AppTraceLog.UsrSign = rop.UsrSign;
            d_AppTraceLog.Creator = rop.UsrSign;
            d_AppTraceLog.CreateTime = DateTime.Now;

            CurrentDb.BI_AppTraceLog.Add(d_AppTraceLog);
            CurrentDb.SaveChanges();


            return result;
        }
    }
}
