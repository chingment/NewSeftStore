using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.Redis
{
    public static class RedisKeyS
    {
        public static readonly string P = "p:{0}";//商户所有商品 0：商户ID
        public static readonly string PSBR = "psbr:{0}";//商户所有商品ID-条形码映射表 0：商户ID
        public static readonly string PSPY = "pspy:{0}";//商户所有商品ID-拼音映射表 0：商户ID
        public static readonly string PSNA = "psna:{0}";//商户所有商品ID-名称映射表 0：商户ID
        public static readonly string IRSN = "irsn";//SN序列号增长因子
        public static readonly string IRPC = "irpc";//取货码增长因子
    }
}
