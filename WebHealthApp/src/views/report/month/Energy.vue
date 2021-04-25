<template>
  <div class="pg-energy">

    <div class="dv-section">
      <div ref="chart_BySmsc" style="width:100%;height:200px;margin:auto;padding: 10px 20px;" />
      <div class="desrb">
        <div class="lf"><img class="dv-icon" src="@/assets/images/icon_energy_smsc.png" alt=""> <span class="dv-name">日均睡觉时长</span><span class="dv-value">5h10'</span> </div>
        <div class="rf"><span class="dv-refRange1">参考范围</span><span class="dv-refRange2">7h~9h</span></div>
      </div>
    </div>

    <div class="dv-section">
      <div ref="chart_ByHrvxzznl" style="width:100%;height:200px;margin:auto;padding: 10px 20px;" />
      <div class="desrb">
        <div class="lf"><img class="dv-icon" src="@/assets/images/icon_energy_hrvxlzl.png" alt=""> <span class="dv-name">心脏总能量</span><span class="dv-value">5h10'</span> </div>
        <div class="rf"><span class="dv-refRange1">参考范围</span><span class="dv-refRange2">7h~9h</span></div>
      </div>
    </div>

    <div class="dv-section">
      <div ref="chart_ByHxztcs" style="width:100%;height:200px;margin:auto;padding: 10px 20px;" />
      <div class="desrb">
        <div class="lf"><img class="dv-icon" src="@/assets/images/icon_energy_hxztcs.png" alt=""> <span class="dv-name">呼吸暂停次数</span><span class="dv-value">5h10'</span> </div>
        <div class="rf"><span class="dv-refRange1">参考范围</span><span class="dv-refRange2">7h~9h</span></div>
      </div>
    </div>

    <div class="dv-section">
      <div ref="chart_ByHxdtcs" style="width:100%;height:200px;margin:auto;padding: 10px 20px;" />
      <div class="desrb">
        <div class="lf"><img class="dv-icon" src="@/assets/images/icon_energy_smdtcs.png" alt=""> <span class="dv-name">低通气次数</span><span class="dv-value">5h10'</span> </div>
        <div class="rf"><span class="dv-refRange1">参考范围</span><span class="dv-refRange2">7h~9h</span></div>
      </div>
    </div>

  </div>
</template>

<script>

import echarts from 'echarts'

var chartBySmsc
var chartByHrvxzznl
var chartByHxztcs
var chartByHxdtcs

export default {
  name: 'Energy',
  mounted() {
    if (!chartBySmsc) {
      chartBySmsc = echarts.init(this.$refs.chart_BySmsc, null, { renderer: 'svg' })
    }
    if (!chartByHrvxzznl) {
      chartByHrvxzznl = echarts.init(this.$refs.chart_ByHrvxzznl, null, { renderer: 'svg' })
    }
    if (!chartByHxztcs) {
      chartByHxztcs = echarts.init(this.$refs.chart_ByHxztcs, null, { renderer: 'svg' })
    }
    if (!chartByHxdtcs) {
      chartByHxdtcs = echarts.init(this.$refs.chart_ByHxdtcs, null, { renderer: 'svg' })
    }
    window.addEventListener('beforeunload', this.clearChart)
  },
  created() {
    this.$nextTick(function() {
      this.getChartBySmsc()
      this.getChartByHrvxzznl()
      this.getChartByHxztcs()
      this.getChartByHxdtcs()
    }, 2000)
  },
  beforeDestroy() {
    if (chartBySmsc) {
      chartBySmsc.clear()
      chartBySmsc.dispose()
      chartBySmsc = null
    }

    if (chartByHrvxzznl) {
      chartByHrvxzznl.clear()
      chartByHrvxzznl.dispose()
      chartByHrvxzznl = null
    }

    if (chartByHxztcs) {
      chartByHxztcs.clear()
      chartByHxztcs.dispose()
      chartByHxztcs = null
    }

    if (chartByHxdtcs) {
      chartByHxdtcs.clear()
      chartByHxdtcs.dispose()
      chartByHxdtcs = null
    }

    // 清空 resize 事件处理函数
    window.removeEventListener('resize', this.resize)
    // 清空 beforeunload 事件处理函数
    window.removeEventListener('beforeunload', this.clearChart)
  },
  methods: {
    clearChart() {
      this.$destroy()
    },
    getChartBySmsc() {
      var datePt = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 26, 27, 28, 29, 30]
      var valuePt = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 26, 27, 28, 29, 30]
      var option = {
        grid: [{
          x: 25,
          y: 25,
          x2: 25,
          y2: 25
        }],
        tooltip: {
          trigger: 'axis'
        },
        xAxis: [{
          data: datePt,
          show: true
        }],
        yAxis: {
          type: 'value'
        },
        series: [{
          type: 'line',
          name: '当吃基准心率',
          showSymbol: false,
          data: valuePt
        }
        ]
      }

      chartBySmsc.setOption(option, null)
    },
    getChartByHrvxzznl() {
      var datePt = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 26, 27, 28, 29, 30]
      var valuePt = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 26, 27, 28, 29, 30]
      var option = {
        grid: [{
          x: 25,
          y: 25,
          x2: 25,
          y2: 25
        }],
        tooltip: {
          trigger: 'axis'
        },
        xAxis: [{
          data: datePt,
          show: true
        }],
        yAxis: {
          type: 'value'
        },
        series: [{
          type: 'line',
          name: '当吃基准心率',
          showSymbol: false,
          data: valuePt
        }
        ]
      }

      chartByHrvxzznl.setOption(option, null)
    },
    getChartByHxztcs() {
      var datePt = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 26, 27, 28, 29, 30]
      var valuePt = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 26, 27, 28, 29, 30]
      var option = {
        grid: [{
          x: 25,
          y: 25,
          x2: 25,
          y2: 25
        }],
        tooltip: {
          trigger: 'axis'
        },
        xAxis: [{
          data: datePt,
          show: true
        }],
        yAxis: {
          type: 'value'
        },
        series: [{
          type: 'line',
          name: '当吃基准心率',
          showSymbol: false,
          data: valuePt
        }
        ]
      }

      chartByHxztcs.setOption(option, null)
    },
    getChartByHxdtcs() {
      var datePt = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 26, 27, 28, 29, 30]
      var valuePt = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 26, 27, 28, 29, 30]
      var option = {
        grid: [{
          x: 25,
          y: 25,
          x2: 25,
          y2: 25
        }],
        tooltip: {
          trigger: 'axis'
        },
        xAxis: [{
          data: datePt,
          show: true
        }],
        yAxis: {
          type: 'value'
        },
        series: [{
          type: 'line',
          name: '当吃基准心率',
          showSymbol: false,
          data: valuePt
        }
        ]
      }

      chartByHxdtcs.setOption(option, null)
    }
  }
}
</script>

<style lang="scss" scoped>

.pg-energy{
  padding: 10px;
}

.dv-section{
  background: #fff;
      box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
    border: 1px solid #ebeef5;
    background-color: #fff;
    color: #303133;
    -webkit-transition: .3s;
    transition: .3s;
    border-radius: 4px;
    overflow: hidden;
    margin-bottom: 10px
}

.desrb{
  display: flex;
    padding: 10px 20px;
        font-size: 14px;
        color: #707070;
  .lf{
    display: flex;
    align-items: center;
        flex: 1;
  }

  .rf{
       display: flex;
      justify-content: flex-end;
    align-items: center;
    flex: 1;
  }

  .dv-icon{
    width: 22px;
    height: 22px;
  }

  .dv-name{
    margin-left: 5px;
  }

  .dv-value{
    margin-left: 5px;
  }

  .dv-refRange2{
    margin-left: 5px;
  }
}

</style>
