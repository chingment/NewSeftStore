using System;
using LocalS.BLL;
using LocalS.Service.UI;
using Lumos;
using Lumos.DbRelay;
using Lumos.Session;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace LocalS.Service.Api.Merch
{
    public class HomeService : BaseService
    {
        public CustomJsonResult GetInitData(string operater, string merchId)
        {
            var result = new CustomJsonResult();


            var d_SysMerchUser = CurrentDb.SysMerchUser.Where(m => m.Id == operater).FirstOrDefault();


            var ret = new { WorkBench = d_SysMerchUser.WorkBench };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);


            return result;
        }

        public CustomJsonResult SaveWorkBench(string operater, string merchId, RopHomeSaveWorkBench rop)
        {
            var result = new CustomJsonResult();


            var d_SysMerchUser = CurrentDb.SysMerchUser.Where(m => m.Id == operater).FirstOrDefault();

            d_SysMerchUser.WorkBench = rop.WorkBench;

            CurrentDb.SaveChanges();


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "");


            return result;
        }
    }
}
