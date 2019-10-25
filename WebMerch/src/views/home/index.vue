<template>
  <div id="home_container">

    <el-row :gutter="8">
      <el-col :xs="{span: 24}" :sm="{span: 24}" :md="{span: 24}" :lg="{span: 12}" :xl="{span: 12}" style="padding-right:8px;margin-bottom:30px;">

        <el-card class="box-card">
          <div slot="header" class="clearfix">
            <span>今日总览</span>
          </div>
          <div class="today-sum" style="height:200px;">
            <div class="it">
              <div class="t1"><span class="m1"> {{ todayGmv.sumCount }}</span><br> <span class="m3">今日订单</span></div>
            </div>
            <div class="it">
              <div class="t1"><span class="m2">  {{ todayGmv.sumTradeAmount }}</span><br> <span class="m3">今日营业额</span></div>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :xs="{span: 24}" :sm="{span: 12}" :md="{span: 12}" :lg="{span: 6}" :xl="{span: 6}" style="margin-bottom:30px;">

        <el-card class="box-card">
          <div slot="header" class="clearfix">
            <span>今日店铺销售额</span>

            <el-button style="float: right; padding: 0px 0" type="text">更多</el-button>
          </div>
          <div style="height:200px;">

            <div v-if="todayStoreGmvRl.length>0">

              <ul class="rl">
                <li v-for="(val,index) in todayStoreGmvRl" :key="val.name" class="it">
                  <div class="name">
                    <span :class="'rli-'+index">  {{ val.name }}</span>
                  </div>
                  <div class="sumQuantity">
                    {{ val.sumQuantity }}
                  </div>
                  <div class="sumTradeAmount">
                    {{ val.sumTradeAmount }}
                  </div>
                </li>
              </ul>
            </div>

          </div>
        </el-card>

      </el-col>
      <el-col :xs="{span: 24}" :sm="{span: 12}" :md="{span: 12}" :lg="{span: 6}" :xl="{span: 6}" style="margin-bottom:30px;">

        <el-card class="box-card">
          <div slot="header" class="clearfix">
            <span>最近7天销售额</span>
          </div>
          <div style="height:200px;">

            <div v-if="get7DayGmv.length>0">

              <ul class="rl">
                <li v-for="(val,index) in get7DayGmv" :key="val.datef" class="it">
                  <div class="name">
                    <span :class="'rli2-'+index">  {{ val.datef }}</span>

                  </div>
                  <div class="sumQuantity">
                    {{ val.sumCount }}
                  </div>
                  <div class="sumTradeAmount">
                    {{ val.sumTradeAmount }}
                  </div>
                </li>
              </ul>
            </div>

          </div>
        </el-card>

      </el-col>
    </el-row>

    <el-row :gutter="8">
      <el-col :xs="{span: 24}" :sm="{span: 24}" :md="{span: 24}" :lg="{span: 12}" :xl="{span: 12}" style="padding-right:8px; margin-bottom:30px;">

        <el-card class="box-card">
          <div slot="header" class="clearfix">
            <span>店铺销售排行榜</span>
          </div>
          <div style="min-height:300px;">

            <div v-if="storeGmvRl.length>0">

              <ul class="rl">
                <li v-for="(val,index) in storeGmvRl" :key="val.name" class="it">
                  <div class="name">
                    <span :class="'rli-'+index">  {{ val.name }}</span>
                  </div>
                  <div class="sumQuantity">
                    {{ val.sumQuantity }}
                  </div>
                  <div class="sumTradeAmount">
                    {{ val.sumTradeAmount }}
                  </div>
                </li>
              </ul>
            </div>
            <div v-else>
              <el-button type="text" @click="goNewStore(item)">暂无店铺，马上新建，GO GO GO</el-button>
            </div>

          </div>
        </el-card>
      </el-col>
      <el-col :xs="{span: 24}" :sm="{span: 24}" :md="{span: 24}" :lg="{span: 12}" :xl="{span: 12}" style="margin-bottom:30px; ">

        <el-card class="box-card">
          <div slot="header" class="clearfix">
            <span>热销商品排行榜</span>
            <el-button style="float: right; padding: 0px 0" type="text">更多</el-button>
          </div>
          <div style="min-height:300px;">

            <div v-if="productSkuSaleRl.length>0">

              <ul class="rl">
                <li v-for="(val,index) in productSkuSaleRl" :key="val.name" class="it">
                  <div class="name">
                    <span :class="'rli-'+index">  {{ val.name }}</span>
                  </div>
                  <div class="sumQuantity">
                    {{ val.sumQuantity }}
                  </div>
                  <div class="sumTradeAmount">
                    {{ val.sumTradeAmount }}
                  </div>
                </li>
              </ul>

            </div>
            <div v-else>
              <span>暂无销售数据</span>
            </div>

          </div>
        </el-card>

      </el-col>

    </el-row>

  </div>
</template>

<script>

import { getProductSkuSaleRl, getStoreGmvRl, getTodayStoreGmvRl, get7DayGmv } from '@/api/home'

export default {

  data() {
    return {
      todayGmv: {
        datef: '',
        sumCount: 0,
        sumTradeAmount: 0
      },
      get7DayGmv: [],
      todayStoreGmvRl: [],
      storeGmvRl: [],
      productSkuSaleRl: []
    }
  },
  created() {
    this._get7DayGmv()
    this._getTodayStoreGmvRl()
    this._getProductSkuSaleRl()
    this._getStoreGmvRl()
  },
  methods: {
    _get7DayGmv: function() {
      get7DayGmv().then(res => {
        if (res.result === 1) {
          this.get7DayGmv = res.data.days
          this.todayGmv = res.data.days[0]
        }
      })
    },
    _getProductSkuSaleRl: function() {
      getProductSkuSaleRl().then(res => {
        if (res.result === 1) {
          this.productSkuSaleRl = res.data.productSkus
        }
      })
    },
    _getStoreGmvRl: function() {
      getStoreGmvRl().then(res => {
        if (res.result === 1) {
          this.storeGmvRl = res.data.stores
        }
      })
    },
    _getTodayStoreGmvRl: function() {
      getTodayStoreGmvRl().then(res => {
        if (res.result === 1) {
          this.todayStoreGmvRl = res.data.stores
        }
      })
    }
  }
}

</script>

<style lang="scss" scoped>
#home_container{
  padding: 20px;
}

.today-sum{
  display: flex;
  .it{
    flex: 1;
    justify-content: center;
    align-items: center;
    align-content: center;
    display: flex;

    .t1{
      text-align: center;

      .m1{
       font-size: 42px;
      }

       .m2{
       font-size: 42px;
       color: #cf9236;
      }
    }
  }
}

.rl{
    list-style: none;
    padding: 0px;
    margin: 0px;

  .it{
  display: flex;
line-height: 30px;
  .name{
    flex: 2;
    text-align: left
  }

  .sumQuantity{
    flex: 1;
    text-align:center;
  }
  .sumTradeAmount{
    flex: 1;
    text-align: right;
  }
  }

  .rli-0{
    color: #2096d4;
  }
  .rli-1{
    color: #24ad8c;
  }
   .rli-2{
    color: #d747a6;
  }

}
</style>
