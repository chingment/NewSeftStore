using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class PrdProductService : BaseDbContext
    {
        public PrdProductModel GetProduct(string storeId, string productId)
        {
            return CacheServiceFactory.PrdProduct.GetProduct(storeId, productId);
        }

        public PrdProductSkuModel GetProductSku(string storeId, string productSkuId)
        {
            return CacheServiceFactory.PrdProduct.GetProductSku(storeId, productSkuId);
        }
    }
}
