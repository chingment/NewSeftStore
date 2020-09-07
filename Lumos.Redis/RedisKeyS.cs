using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.Redis
{
    public static class RedisKeyS
    {
        public static readonly string PRD_SPU_INF = "prd_spu_inf:{0}";//商户所有商品 0：商户ID
        public static readonly string PRD_SKU_INF = "prd_sku_inf:{0}";//商户所有商品 0：商户ID
        public static readonly string PRD_SKU_SKEY = "prd_sku_skey:{0}";//商户所有商品ID-条形码映射表 0：商户ID
        public static readonly string PRD_SKU_YKEY = "prd_sku_ykey:{0}";//店铺所有商品ID-条形码映射表 0：店铺ID
        public static readonly string IR_SN = "irsn";//SN序列号增长因子
        public static readonly string IR_MACHINEID = "irmachineid";//SN序列号增长因子
        public static readonly string IR_PICKCODE = "irpc";//取货码增长因子

        public static readonly string STSLPRD = "stslprd:{1}";//店铺可售商品的集合
    }
}
