using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class SellChannelStockCacheService : BaseDbContext
    {
        private static readonly string key_Format_SellStock = "SellStock:{0}";

        public void ReSet()
        {
            var redis = new RedisClient<PrdProductSkuStockModel>();

            var merchSellChannelStocks = CurrentDb.SellChannelStock.ToList();

            foreach (var merchSellChannelStock in merchSellChannelStocks)
            {
                redis.KRemove(string.Format(key_Format_SellStock, merchSellChannelStock.MerchId, merchSellChannelStock.PrdProductSkuId));

                var sellStock = redis.KGet(string.Format(key_Format_SellStock, merchSellChannelStock.MerchId, merchSellChannelStock.PrdProductSkuId));
                if (sellStock == null)
                {
                    sellStock = new PrdProductSkuStockModel();
                    sellStock.Id = merchSellChannelStock.PrdProductSkuId;

                    var stock = new PrdProductSkuStockModel.Stock();
                    stock.RefType = merchSellChannelStock.RefType;
                    stock.RefId = merchSellChannelStock.RefId;
                    stock.SlotId = merchSellChannelStock.SlotId;
                    stock.SumQuantity = merchSellChannelStock.SumQuantity;
                    stock.LockQuantity = merchSellChannelStock.LockQuantity;
                    stock.SellQuantity = merchSellChannelStock.SellQuantity;
                    stock.IsOffSell = merchSellChannelStock.IsOffSell;
                    stock.SalePrice = merchSellChannelStock.SalePrice;
                    stock.SalePriceByVip = merchSellChannelStock.SalePriceByVip;

                    sellStock.Stocks.Add(stock);

                    redis.KSet(string.Format(key_Format_SellStock, merchSellChannelStock.MerchId, merchSellChannelStock.PrdProductSkuId), sellStock, new TimeSpan(100, 0, 0));
                }
                else
                {
                    var stock = sellStock.Stocks.Where(m => m.RefType == merchSellChannelStock.RefType && m.RefId == merchSellChannelStock.RefId && m.SlotId == merchSellChannelStock.SlotId).FirstOrDefault();
                    if (stock == null)
                    {
                        stock = new PrdProductSkuStockModel.Stock();
                        stock.RefType = merchSellChannelStock.RefType;
                        stock.RefId = merchSellChannelStock.RefId;
                        stock.SlotId = merchSellChannelStock.SlotId;
                        stock.SumQuantity = merchSellChannelStock.SumQuantity;
                        stock.LockQuantity = merchSellChannelStock.LockQuantity;
                        stock.SellQuantity = merchSellChannelStock.SellQuantity;
                        stock.IsOffSell = merchSellChannelStock.IsOffSell;
                        stock.SalePrice = merchSellChannelStock.SalePrice;
                        stock.SalePriceByVip = merchSellChannelStock.SalePriceByVip;
                        sellStock.Stocks.Add(stock);
                        redis.KSet(string.Format(key_Format_SellStock, merchSellChannelStock.MerchId, merchSellChannelStock.PrdProductSkuId), sellStock, new TimeSpan(100, 0, 0));
                    }
                }
            }
        }

        public PrdProductSkuStockModel GetStock(string productSkuId)
        {
            var redis = new RedisClient<PrdProductSkuStockModel>();

            var sellStock = redis.KGet(string.Format(key_Format_SellStock, productSkuId));

            if (sellStock == null)
            {
                sellStock = new PrdProductSkuStockModel();
                sellStock.Id = productSkuId;

                var merchSellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.PrdProductSkuId == productSkuId).ToList();

                foreach (var merchSellChannelStock in merchSellChannelStocks)
                {
                    var stock = new PrdProductSkuStockModel.Stock();
                    stock.RefType = merchSellChannelStock.RefType;
                    stock.RefId = merchSellChannelStock.RefId;
                    stock.SlotId = merchSellChannelStock.SlotId;
                    stock.SumQuantity = merchSellChannelStock.SumQuantity;
                    stock.LockQuantity = merchSellChannelStock.LockQuantity;
                    stock.SellQuantity = merchSellChannelStock.SellQuantity;
                    stock.IsOffSell = merchSellChannelStock.IsOffSell;
                    stock.SalePrice = merchSellChannelStock.SalePrice;
                    stock.SalePriceByVip = merchSellChannelStock.SalePriceByVip;
                    sellStock.Stocks.Add(stock);
                }

                redis.KSet(string.Format(key_Format_SellStock, productSkuId), sellStock, new TimeSpan(100, 0, 0));
            }

            return sellStock;
        }
    }
}
