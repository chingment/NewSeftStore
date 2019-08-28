using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class PrdProductService : BaseDbContext
    {
        public PrdProductModel GetModelById(string storeId, string productId)
        {
            return CacheServiceFactory.PrdProduct.GetModelById(storeId, productId);
        }

        public PrdProductSkuModel GetSkuModelById(string storeId, string productSkuId)
        {
            return CacheServiceFactory.PrdProduct.GetSkuModelById(storeId, productSkuId);
        }
    }
}
