using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class PrdProductService : BaseDbContext
    {
        public PrdProductModel GetProduct(string productId)
        {
            return CacheServiceFactory.PrdProduct.GetProduct(productId);
        }

        public PrdProductSkuModel GetProductSku(string productSkuId)
        {
            return CacheServiceFactory.PrdProduct.GetProductSku(productSkuId);
        }
    }
}
