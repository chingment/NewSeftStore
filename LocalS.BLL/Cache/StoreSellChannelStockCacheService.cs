using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class StoreSellChannelStockCacheService : BaseDbContext
    {
        public void ReSet()
        {
            var storeSellChannelStocks = CurrentDb.StoreSellChannelStock.ToList();
        }

        public List<StoreSellChannelStockModel> GetStock(string storeId, string productSkuId)
        {
            return null;
        }
    }
}
