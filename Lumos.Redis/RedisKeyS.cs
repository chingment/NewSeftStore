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
        //public static readonly string PRD_SKU_SBR = "prd_sku_sbr:{0}";//商户所有商品ID-条形码映射表 0：商户ID
        //public static readonly string PRD_SKU_SPY = "prd_sku_spy:{0}";//商户所有商品ID-拼音映射表 0：商户ID
        //public static readonly string PRD_SKU_SNA = "prd_sku_sna:{0}";//商户所有商品ID-名称映射表 0：商户ID
        //public static readonly string PRD_SKU_SCC = "prd_sku_scc:{0}";//商户所有商品ID-编码表 0：商户ID
        public static readonly string IR_SN = "irsn";//SN序列号增长因子
        public static readonly string IR_MACHINEID = "irmachineid";//SN序列号增长因子
        public static readonly string IR_PICKCODE = "irpc";//取货码增长因子

        public static readonly string STSLPRD = "stslprd:{1}";//店铺可售商品的集合
    }
}
