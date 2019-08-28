using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public static class CacheServiceFactory
    {
        public static PrdProductCacheService PrdProduct
        {
            get
            {
                return new PrdProductCacheService();
            }
        }

        public static StoreSellChannelStockCacheService StoreSellChannelStock
        {
            get
            {
                return new StoreSellChannelStockCacheService();
            }
        }
    }
}
