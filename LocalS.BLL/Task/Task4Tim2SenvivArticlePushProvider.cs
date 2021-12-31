using LocalS.BLL.Biz;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using MyWeiXinSdk;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.BLL.Task
{
    public class Task4Tim2SenvivArticlePushProvider : BaseService, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            //var d_SenvivUser = CurrentDb.sysu.Where(m => m.Id == senvivUser.userid).FirstOrDefault();


        }
    }
}