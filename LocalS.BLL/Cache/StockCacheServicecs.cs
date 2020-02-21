using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Cache
{
    public class StockCacheServicecs
    {
        public void SaveStock(string storeId, string machineId, string productSkuId, string slotId)
        {
             //RedisManager.Db.HashSetAsync(string.Format(RedisKeyS.STSLPRD, storeId), p.Always);

            /*
              查询店铺商品的库存
              查询店铺可售商品机器的ID
              //若指定商品机器ID
              查询机器下商品ID的可售货道 
             
              获取货道库存 返回库存数
             
             */

        }
    }
}
