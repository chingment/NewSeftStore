using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public static class CacheServiceFactory
    {
        public static PrdProductCacheService Product
        {
            get
            {
                return new PrdProductCacheService();
            }
        }

        public static PrdProductSkuCacheService ProductSku
        {
            get
            {
                return new PrdProductSkuCacheService();
            }
        }
    }
}
