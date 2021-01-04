using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class MemberRightService: BaseService
    {
        public CustomJsonResult GetLevels(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var machineCount = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId).Count();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { machineCount = machineCount });
            return result;
        }
    }
}
