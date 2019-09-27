using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class PrdProductService : BaseDbContext
    {
        public PrdProductModel GetProductInfo(string productId)
        {
            return CacheServiceFactory.PrdProduct.GetProductInfo(productId);
        }

        public PrdProductSkuModel GetProductSkuInfo(string productSkuId)
        {
            return CacheServiceFactory.PrdProduct.GetProductSkuInfo(productSkuId);
        }
    }
}
