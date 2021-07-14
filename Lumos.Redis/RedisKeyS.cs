﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.Redis
{
    public static class RedisKeyS
    {
        public static readonly string PRD_SPU_INF = "prd_spu_inf:{0}";//商户所有商品SPU信息表 0：商户ID
        public static readonly string PRD_SPU_SKEY = "prd_spu_skey:{0}";//商户所有商品SPU-搜索关键字映射表 0：商户ID
        public static readonly string PRD_SKU_INF = "prd_sku_inf:{0}";//商户所有商品SKU信息表 0：商户ID
        public static readonly string PRD_SKU_SKEY = "prd_sku_skey:{0}";//商户所有商品SKU-搜索关键字映射表 0：商户ID
        //public static readonly string PRD_SKU_YKEY = "prd_sku_ykey:{0}";//店铺所有商品ID-条形码映射表 0：店铺ID
        public static readonly string IR_SN = "irsn";//SN序列号增长因子
       // public static readonly string IR_DEVICEID = "irdeviceid";//SN序列号增长因子
        public static readonly string IR_PICKCODE = "irpc";//取货码增长因子

        public static readonly string STSLPRD = "stslprd:{0}";//店铺可售商品的集合

        public static readonly string DEVICE_SHIP = "device_ship:{0}";
    }
}
