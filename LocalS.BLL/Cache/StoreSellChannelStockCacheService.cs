using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class StoreSellChannelStockCacheService : BaseDbContext
    {
        private static readonly string key_Format_SellStock = "SellStock:{0}:{1}";

        public void ReSet()
        {
            var redis = new RedisClient<PrdProductSkuStockModel>();

            var storeSellChannelStocks = CurrentDb.StoreSellChannelStock.ToList();


            foreach (var storeSellChannelStock in storeSellChannelStocks)
            {
                redis.KRemove(string.Format(key_Format_SellStock, storeSellChannelStock.StoreId, storeSellChannelStock.PrdProductSkuId));

                var sellStock = redis.KGet(string.Format(key_Format_SellStock, storeSellChannelStock.StoreId, storeSellChannelStock.PrdProductSkuId));
                if (sellStock == null)
                {
                    sellStock = new PrdProductSkuStockModel();
                    sellStock.Id = storeSellChannelStock.PrdProductSkuId;

                    var stock = new PrdProductSkuStockModel.Stock();
                    stock.RefType = storeSellChannelStock.RefType;
                    stock.RefId = storeSellChannelStock.RefId;
                    stock.SlotId = storeSellChannelStock.SlotId;
                    stock.SumQuantity = storeSellChannelStock.SumQuantity;
                    stock.LockQuantity = storeSellChannelStock.LockQuantity;
                    stock.SellQuantity = storeSellChannelStock.SellQuantity;
                    stock.IsOffSell = storeSellChannelStock.IsOffSell;
                    stock.SalePrice = storeSellChannelStock.SalePrice;
                    stock.SalePriceByVip = storeSellChannelStock.SalePriceByVip;

                    sellStock.Stocks.Add(stock);

                    redis.KSet(string.Format(key_Format_SellStock, storeSellChannelStock.StoreId, storeSellChannelStock.PrdProductSkuId), sellStock, new TimeSpan(100, 0, 0));
                }
                else
                {
                    var stock = sellStock.Stocks.Where(m => m.RefType == storeSellChannelStock.RefType && m.RefId == storeSellChannelStock.RefId && m.SlotId == storeSellChannelStock.SlotId).FirstOrDefault();
                    if (stock == null)
                    {
                        stock = new PrdProductSkuStockModel.Stock();
                        stock.RefType = storeSellChannelStock.RefType;
                        stock.RefId = storeSellChannelStock.RefId;
                        stock.SlotId = storeSellChannelStock.SlotId;
                        stock.SumQuantity = storeSellChannelStock.SumQuantity;
                        stock.LockQuantity = storeSellChannelStock.LockQuantity;
                        stock.SellQuantity = storeSellChannelStock.SellQuantity;
                        stock.IsOffSell = storeSellChannelStock.IsOffSell;
                        stock.SalePrice = storeSellChannelStock.SalePrice;
                        stock.SalePriceByVip = storeSellChannelStock.SalePriceByVip;
                        sellStock.Stocks.Add(stock);
                        redis.KSet(string.Format(key_Format_SellStock, storeSellChannelStock.StoreId, storeSellChannelStock.PrdProductSkuId), sellStock, new TimeSpan(100, 0, 0));
                    }
                }
            }
        }

        public PrdProductSkuStockModel GetStock(string storeId, string productSkuId)
        {
            var redis = new RedisClient<PrdProductSkuStockModel>();

            var sellStock = redis.KGet(string.Format(key_Format_SellStock, storeId, productSkuId));

            if (sellStock == null)
            {
                sellStock = new PrdProductSkuStockModel();
                sellStock.Id = productSkuId;

                var storeSellChannelStocks = CurrentDb.StoreSellChannelStock.Where(m => m.StoreId == storeId && m.PrdProductId == productSkuId).ToList();

                foreach (var storeSellChannelStock in storeSellChannelStocks)
                {
                    var stock = new PrdProductSkuStockModel.Stock();
                    stock.RefType = storeSellChannelStock.RefType;
                    stock.RefId = storeSellChannelStock.RefId;
                    stock.SlotId = storeSellChannelStock.SlotId;
                    stock.SumQuantity = storeSellChannelStock.SumQuantity;
                    stock.LockQuantity = storeSellChannelStock.LockQuantity;
                    stock.SellQuantity = storeSellChannelStock.SellQuantity;
                    stock.IsOffSell = storeSellChannelStock.IsOffSell;
                    stock.SalePrice = storeSellChannelStock.SalePrice;
                    stock.SalePriceByVip = storeSellChannelStock.SalePriceByVip;
                    sellStock.Stocks.Add(stock);
                }

                redis.KSet(string.Format(key_Format_SellStock, storeId, productSkuId), sellStock, new TimeSpan(100, 0, 0));
            }

            return sellStock;
        }
    }
}
