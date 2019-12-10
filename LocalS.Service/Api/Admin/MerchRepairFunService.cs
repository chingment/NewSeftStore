using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Admin
{
    public class MerchRepairFunService
    {
        public CustomJsonResult ReLoadProductSkuCache(string operater)
        {
            var result = new CustomJsonResult();


            CacheServiceFactory.ProductSku.ReLoad();

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "执行成功");

            return result;
        }
    }
}
