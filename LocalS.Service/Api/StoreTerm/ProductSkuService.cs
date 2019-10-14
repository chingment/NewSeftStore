using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class ProductSkuService : BaseDbContext
    {
        public CustomJsonResult Search(RupProductSkuSearch rup)
        {
            var result = new CustomJsonResult();

            var ret = new RetProductSkuSearch();

            var machine = CurrentDb.Machine.Where(m => m.Id == rup.MachineId).FirstOrDefault();
            if (result == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未登记");
            }


            ret.ProductSkus = CacheServiceFactory.ProductSku.Search(machine.MerchId, rup.Key);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
