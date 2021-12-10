using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Entity;
using LocalS.Service.UI;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace LocalS.Service.Api.Merch
{
    public class SenvivWorkBenchService : BaseService
    {
        public CustomJsonResult GetInitData(string operater, string merchId)
        {
            var result = new CustomJsonResult();


            var merchIds = BizFactory.Merch.GetRelIds(merchId);

            var users = CurrentDb.SenvivUser.Where(m => merchIds.Contains(m.MerchId)).ToList();

            var userCount = users.Count();
            var careLevel0 = users.Where(m => m.CareLevel == E_SenvivUserCareLevel.None).Count();
            var careLevel1 = users.Where(m => m.CareLevel == E_SenvivUserCareLevel.One).Count();
            var careLevel2 = users.Where(m => m.CareLevel == E_SenvivUserCareLevel.Two).Count();
            var careLevel3 = users.Where(m => m.CareLevel == E_SenvivUserCareLevel.Three).Count();
            var careLevel4 = users.Where(m => m.CareLevel == E_SenvivUserCareLevel.Four).Count();

            var ret = new
            {
                userCount = userCount,
                careLevel = new
                {
                    level0 = careLevel0,
                    level1 = careLevel1,
                    level2 = careLevel2,
                    level3 = careLevel3,
                    level4 = careLevel4,
                },
                todoTask = new
                {
                    waitHandle = 0,
                    handled = 0
                }
            };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
