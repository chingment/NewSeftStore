<template>
  <div class="pg-energy">

    <div class="dv-section">
      <div ref="chart_BySmsc" style="width:100%;height:200px;margin:auto;padding: 10px 20px;" />
      <div class="desrb">
        <div class="lf"><img class="dv-icon" src="@/assets/images/icon_energy_smsc.png" alt=""> <span class="dv-name">日均睡觉时长</span><span class="dv-value">{{ rd.smSmsc.value }}</span> </div>
        <div class="rf"><span class="dv-refRange1">参考范围</span><span class="dv-refRange2">{{ rd.smSmsc.refRange }}</span></div>
      </div>
    </div>

    <div class="dv-section">
      <div ref="chart_ByHrvxzznl" style="width:100%;height:200px;margin:auto;padding: 10px 20px;" />
      <div class="desrb">
        <div class="lf"><img class="dv-icon" src="@/assets/images/icon_energy_hrvxlzl.png" alt=""> <span class="dv-name">心脏总能量</span><span class="dv-value">{{ rd.hrvXzznl.value }}</span> </div>
        <div class="rf"><span class="dv-refRange1">参考范围</span><span class="dv-refRange2">{{ rd.hrvXzznl.refRange }}</span></div>
      </div>
    </div>

    <div class="dv-section">
      <div ref="chart_ByHxztcs" style="width:100%;height:200px;margin:auto;padding: 10px 20px;" />
      <div class="desrb">
        <div class="lf"><img class="dv-icon" src="@/assets/images/icon_energy_hxztcs.png" alt=""> <span class="dv-name">呼吸暂停次数</span><span class="dv-value">{{ rd.hxZtcs.value }}</span> </div>
        <div class="rf"><span class="dv-refRange1">参考范围</span><span class="dv-refRange2">{{ rd.hxZtcs.refRange }}</span></div>
      </div>
    </div>

    <div class="dv-section">
      <div ref="chart_ByHxdtcs" style="width:100%;height:200px;margin:auto;padding: 10px 20px;" />
      <div class="desrb">
        <div class="lf"><img class="dv-icon" src="@/assets/images/icon_energy_smdtcs.png" alt=""> <span class="dv-name">低通气次数</span><span class="dv-value">{{ rd.hxZtahizs.value }}</span> </div>
        <div class="rf"><span class="dv-refRange1">参考范围</span><span class="dv-refRange2">{{ rd.hxZtahizs.refRange }}</span></div>
      </div>
    </div>

  </div>
</template>

<script>

import { getEnergy } from '@/api/monthreport'
import DvItem from '@/components/DvItem.vue'
import echarts from 'echarts'

var chartBySmsc
var chartByHrvxzznl
var chartByHxztcs
var chartByHxdtcs

export default {
  name: 'Energy',
  components: { DvItem },
  data() {
    return {
      loading: false,
      rd: {
        hrvXzznl: { color: '', value: '', refRange: '' },
        hxZtcs: { color: '', value: '', refRange: '' },
        smSmsc: { color: '', value: '', refRange: '' },
        hxZtahizs: { color: '', value: '', refRange: '' },
        datePt: [0],
        hrvXzznlPt: [0],
        hxZtcsPt: [0],
        smSmscPt: [0],
        hxZtahizsPt: [0]
      }
    }
  },
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
    this._getEnergy()
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
    _getEnergy() {
      this.loading = true
      getEnergy({ rptId: this.$route.query.rptId }).then(res => {
        if (res.result === 1) {
          var d = res.data

          this.rd.hrvXzznl = d.hrvXzznl
          this.rd.hxZtcs = d.hxZtcs
          this.rd.smSmsc = d.smSmsc
          this.rd.hxZtahizs = d.hxZtahizs
          this.rd.datePt = d.datePt
          this.rd.hrvXzznlPt = d.hrvXzznlPt
          this.rd.hxZtcsPt = d.hxZtcsPt
          this.rd.smSmscPt = d.smSmscPt
          this.rd.hxZtahizsPt = d.hxZtahizsPt

          this.$nextTick(function() {
            this.getChartBySmsc()
            this.getChartByHrvxzznl()
            this.getChartByHxztcs()
            this.getChartByHxdtcs()
          }, 2000)
        }
        this.loading = false
      })
    },
    clearChart() {
      this.$destroy()
    },
    getChartBySmsc() {
      var datePt = this.rd.datePt
      var valuePt = this.rd.smSmscPt
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
          name: '日均睡觉时长',
          showSymbol: false,
          data: valuePt
        }
        ]
      }

      chartBySmsc.setOption(option, null)
    },
    getChartByHrvxzznl() {
      var datePt = this.rd.datePt
      var valuePt = this.rd.hrvXzznlPt
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
          type: 'value',
          axisLabel: {
            formatter: function(value) {
              var texts = []
              if (value == 0) {
                texts.push('0')
              } else if (value == 1000) {
                texts.push('1')
              } else if (value == 2000) {
                texts.push('2')
              } else if (value == 3000) {
                texts.push('3')
              } else if (value == 4000) {
                texts.push('4')
              }
              return texts
            }
          }
        },
        series: [{
          type: 'line',
          name: '心脏总能量',
          showSymbol: false,
          data: valuePt
        }
        ]
      }

      chartByHrvxzznl.setOption(option, null)
    },
    getChartByHxztcs() {
      var datePt = this.rd.datePt
      var valuePt = this.rd.hxZtcsPt
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
          name: '呼吸暂停次数',
          showSymbol: false,
          data: valuePt
        }
        ]
      }

      chartByHxztcs.setOption(option, null)
    },
    getChartByHxdtcs() {
      var datePt = this.rd.datePt
      var valuePt = this.rd.hxZtahizsPt
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
          name: '低通气次数',
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
