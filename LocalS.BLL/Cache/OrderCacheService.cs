using LocalS.Entity;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Cache
{
    public class OrderCacheService
    {
        private static readonly string redis_key_all_successorder_by_merchId = "info_successorder_all:{0}";

        private bool AddSuccessOrder(string merchId, Order order)
        {
            return RedisManager.Db.HashSet(string.Format(redis_key_all_successorder_by_merchId, merchId), order.Id, order.ToJsonString(), StackExchange.Redis.When.Always);
        }

    }
}
