<template>
  <div>
    <el-row :gutter="40" class="panel-group">
      <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
        <div class="card-panel" @click="handleStoreCount('newVisitis')">
          <div class="card-panel-icon-wrapper icon-select">
            <svg-icon icon-class="t_store" class-name="card-panel-icon" />
          </div>
          <div class="card-panel-description">
            <div class="card-panel-text">
              店铺
            </div>
            <count-to :start-val="0" :end-val="storeCount" :duration="2600" class="card-panel-num" />
          </div>
        </div>
      </el-col>
      <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
        <div class="card-panel" @click="handleDeviceCount('messages')">
          <div class="card-panel-icon-wrapper icon-select">
            <svg-icon icon-class="t_machine" class-name="card-panel-icon" />
          </div>
          <div class="card-panel-description">
            <div class="card-panel-text">
              设备
            </div>
            <div style="display:flex">
              <div style="margin-right:10px;display:flex; align-items: center;">
                台数：
                <count-to :start-val="0" :end-val="deviceCount" :duration="3000" class="card-panel-num" />
              </div>
              <div style="display:flex; align-items: center;">
                异常：
                <count-to :start-val="0" :end-val="deviceExCount" :duration="3000" class="card-panel-num" style="color:#ff4949;font-size:30px;" />
              </div>
            </div>

          </div>
        </div>
      </el-col>
      <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
        <div class="card-panel" @click="handleSumTradeAmount('purchases')">
          <div class="card-panel-icon-wrapper icon-select">
            <svg-icon icon-class="t_money" class-name="card-panel-icon" />
          </div>
          <div class="card-panel-description">

            <div>
              <div class="card-panel-text">
                总收入
              </div>
              <count-to :start-val="0" :decimals="2" :end-val="sumTradeAmount" :duration="3000" class="card-panel-num" />
            </div>
          </div>
        </div>
      </el-col>
      <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
        <div class="card-panel" @click="handleReplenishCount('shoppings')">
          <div class="card-panel-icon-wrapper icon-select">
            <svg-icon icon-class="t_buhuo" class-name="card-panel-icon" />
          </div>
          <div class="card-panel-description">
            <div class="card-panel-text">
              补货
            </div>
            <count-to :start-val="0" :end-val="replenishCount" :duration="3600" class="card-panel-num" style="color:#ff4949;font-size:30px;" />
          </div>
        </div>
      </el-col>
    </el-row>
    <el-row :gutter="8">
      <el-col :xs="{span: 24}" :sm="{span: 24}" :md="{span: 24}" :lg="{span: 12}" :xl="{span: 12}" style="padding-right:8px;margin-bottom:30px;">

        <el-card class="box-card">
          <div slot="header" class="clearfix">
            <span>今日总览</span>
          </div>
          <div class="today-sum" style="height:300px;">
            <div class="it">
              <div class="t1" @click="todayGmvClick"><span class="m1"> {{ todayGmv.sumCount }}</span><br> <span class="d1">今日订单量</span></div>
            </div>
            <div class="it">
              <div class="t1" @click="todayGmvClick"><span class="m1"> {{ todayGmv.sumQuantity }}</span><br> <span class="d1">今日商品量</span></div>
            </div>
            <div class="it">
              <div class="t1" @click="todayGmvClick"><span class="m2">  {{ todayGmv.sumTradeAmount }}</span><br> <span class="d1">今日营业额</span></div>
            </div>
            <div class="it">
              <div class="t1" @click="sumExHdByDeviceSelfTakeClick"><span class="m3">  {{ todaySummary.sumExHdByDeviceSelfTake }}</span><br> <span class="d1">设备异常订单</span></div>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :xs="{span: 24}" :sm="{span: 12}" :md="{span: 12}" :lg="{span: 6}" :xl="{span: 6}" style="margin-bottom:30px;">

        <el-card class="box-card">
          <div slot="header" class="clearfix">
            <span>今日店铺销售额</span>

            <el-button style="float: right; padding: 0px 0;display:none" type="text">更多</el-button>
          </div>
          <div style="height:300px;">

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
            <span>最近10天销售额</span>
          </div>
          <div style="height:300px;">

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
            <el-button style="float: right; padding: 0px 0;display:none" type="text">更多</el-button>
          </div>
          <div style="min-height:300px;">

            <div v-if="skuSaleRl.length>0">

              <ul class="rl">
                <li v-for="(val,index) in skuSaleRl" :key="val.name" class="it">
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
import CountTo from 'vue-count-to'
import { getSkuSaleRl, getStoreGmvRl, getTodayStoreGmvRl, get7DayGmv, getTodaySummary, getInitData } from '@/api/shopworkbench'
export default {
  name: 'HomeIndex',
  components: {
    CountTo
  },
  data() {
    return {
      storeCount: 0,
      deviceCount: 0,
      deviceExCount: 0,
      sumTradeAmount: 0,
      replenishCount: 0,
      todaySummary: {
        sumExHdByDeviceSelfTake: 0,
        todayGmvRl: {
          sumCount: 0,
          sumQuantity: 0,
          sumTradeAmount: 0
        }
      },
      todayGmv: {
        datef: '',
        sumQuantity: 0,
        sumCount: 0,
        sumTradeAmount: 0
      },
      get7DayGmv: [],
      todayStoreGmvRl: [],
      storeGmvRl: [],
      skuSaleRl: []
    }
  },
  created() {
    this._getInitData()
    this._getTodaySummary()
    this._get7DayGmv()
    this._getTodayStoreGmvRl()
    this._getSkuSaleRl()
    this._getStoreGmvRl()
  },
  methods: {
    _getInitData: function() {
      getInitData().then(res => {
        if (res.result === 1) {
          var d = res.data
          this.storeCount = d.storeCount
          this.deviceCount = d.deviceCount
          this.sumTradeAmount = d.sumTradeAmount
          this.replenishCount = d.replenishCount
          this.deviceExCount = d.deviceExCount
        }
      })
    },
    _getTodaySummary: function() {
      getTodaySummary().then(res => {
        if (res.result === 1) {
          var d = res.data
          this.todaySummary.sumExHdByDeviceSelfTake = d.sumExHdByDeviceSelfTake
          // this.todayGmv = res.data.days[0]
        }
      })
    },
    _get7DayGmv: function() {
      get7DayGmv().then(res => {
        if (res.result === 1) {
          this.get7DayGmv = res.data.days
          this.todayGmv = res.data.days[0]
        }
      })
    },
    _getSkuSaleRl: function() {
      getSkuSaleRl().then(res => {
        if (res.result === 1) {
          this.skuSaleRl = res.data.skus
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
    },
    sumExHdByDeviceSelfTakeClick() {
      this.$cookies.set('isHasEx', '1')
      this.$router.push({
        path: '/order/listbymt'
      })
    },
    todayGmvClick() {
      this.$cookies.set('tradeStartTime', '2020-01-20')
      this.$cookies.set('tradeEndTime', '2020-01-20')
      this.$router.push({
        path: '/order/list'
      })
    },
    handleStoreCount() {
      this.$router.push({
        path: '/store/list'
      })
    },
    handleDeviceCount() {
      this.$router.push({
        path: '/device/vending'
      })
    },
    handleSumTradeAmount() {
      this.$router.push({
        path: '/order/list'
      })
    },
    handleReplenishCount() {

    }

  }
}

</script>

<style lang="scss" scoped>

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
      cursor: pointer;
      .m1{
       font-size: 42px;
       line-height: 60px;
      }

       .m2{
       font-size: 42px;
       color: #cf9236;
         line-height: 60px;
      }

           .m3{
       font-size: 42px;
       color: #ff4949;
         line-height: 60px;
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
height:30px;
overflow:hidden;
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

.panel-group {

  .card-panel-col {
    margin-bottom: 30px;
  }

  .card-panel {
    height: 108px;
    cursor: pointer;
    font-size: 12px;
    position: relative;
    overflow: hidden;
    color: #666;
    background: #fff;
    -webkit-box-shadow: 0 2px 12px 0 rgba(0,0,0,.1);
    box-shadow: 0 2px 12px 0 rgba(0,0,0,.1);
    border-color: rgba(0, 0, 0, .05);
    border-radius: 4px;
    &:hover {
      .card-panel-icon-wrapper {
        color: #fff;
      }

      .icon-select {
        background: #40c9c6;
      }
    }

    .icon-people {
      color: #40c9c6;
    }

    .icon-message {
      color: #36a3f7;
    }

    .icon-money {
      color: #f4516c;
    }

    .icon-shopping {
      color: #34bfa3
    }

    .card-panel-icon-wrapper {
      float: left;
      margin: 14px 0 0 14px;
      padding: 16px;
      transition: all 0.38s ease-out;
      border-radius: 6px;
    }

    .card-panel-icon {
      float: left;
      font-size: 48px;
    }

    .card-panel-description {
      float: right;
      font-weight: bold;
      margin: 26px;
      margin-left: 0px;

      .card-panel-text {
        line-height: 18px;
        color: rgba(0, 0, 0, 0.45);
        font-size: 16px;
        margin-bottom: 12px;
      }

      .card-panel-num {
        font-size: 20px;
      }
    }
  }
}

@media (max-width:550px) {
  .card-panel-description {
    display: none;
  }

  .card-panel-icon-wrapper {
    float: none !important;
    width: 100%;
    height: 100%;
    margin: 0 !important;

    .svg-icon {
      display: block;
      margin: 14px auto !important;
      float: none !important;
    }
  }
}

</style>
