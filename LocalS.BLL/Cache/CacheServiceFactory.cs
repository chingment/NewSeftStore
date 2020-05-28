using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public static class CacheServiceFactory
    {
        public static ProductCacheService Product
        {
            get
            {
                return new ProductCacheService();
            }
        }
    }
}
