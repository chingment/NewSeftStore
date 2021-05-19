<template>
  <div class="pg-energy" style="height:500px">

    <div class="pt1">
      <img class="pt1_t1" src="@/assets/images/ts/bg_energy_pt1_t1.png" alt="">
      <img class="pt1_t2" src="@/assets/images/ts/bg_energy_pt1_t2.png" alt="">
      <img class="pt1_t3" src="@/assets/images/ts/bg_energy_pt1_t2.png" alt="">
      <div class="at1">
        <div><span> 本月综合得分</span><span>52</span></div>
      </div>
      <div class="at2" />
      <div class="at3">

        <div class="dvit">
          <div class="dvit-head">
            <div class="t1"><span>指标(日均)</span></div>
            <div class="t2"><span>测量值</span></div>
            <div class="t3"><span>参考值</span></div>
          </div>
          <div class="dvit-item">
            <div class="t1"><span>睡眠时长</span></div>
            <div class="t2"><dv-item :value="rd.smSmsc" sign /></div>
            <div class="t3"><span>{{ rd.smSmsc.refRange }}</span></div>
          </div>
          <div class="dvit-item">
            <div class="t1"><span>浅睡眠时长</span></div>
            <div class="t2"><dv-item :value="rd.smQdsmsc" sign /></div>
            <div class="t3"><span>{{ rd.smQdsmsc.refRange }}</span></div>
          </div>
          <div class="dvit-item">
            <div class="t1"><span>深睡眠时长</span></div>
            <div class="t2"><dv-item :value="rd.smSdsmsc" sign /></div>
            <div class="t3"><span>{{ rd.smSdsmsc.refRange }}</span></div>
          </div>
          <div class="dvit-item">
            <div class="t1"><span>REM睡眠时长</span></div>
            <div class="t2"><dv-item :value="rd.smRemsmsc" sign /></div>
            <div class="t3"><span>{{ rd.smRemsmsc.refRange }}</span></div>
          </div>
          <div class="dvit-item">
            <div class="t1"><span>心脏总能量</span></div>
            <div class="t2"><dv-item :value="rd.hrvXzznl" sign /></div>
            <div class="t3"><span>{{ rd.hrvXzznl.refRange }}</span></div>
          </div>
          <div class="dvit-item">
            <div class="t1"><span>平均呼吸</span></div>
            <div class="t2"><dv-item :value="rd.hxDcpjhx" sign /></div>
            <div class="t3"><span>{{ rd.hxDcpjhx.refRange }}</span></div>
          </div>
          <div class="dvit-item">
            <div class="t1"><span>平均心率</span></div>
            <div class="t2"><dv-item :value="rd.xlDcpjxl" sign /></div>
            <div class="t3"><span>{{ rd.xlDcpjxl.refRange }}</span></div>
          </div>
          <div class="dvit-item">
            <div class="t1"><span>呼吸暂停</span></div>
            <div class="t2"><dv-item :value="rd.hxZtcs" sign /></div>
            <div class="t3"><span>{{ rd.hxZtcs.refRange }}</span></div>
          </div>
          <div class="dvit-item">
            <div class="t1"><span>体动</span></div>
            <div class="t2"><dv-item :value="rd.smTdcs" sign /></div>
            <div class="t3"><span>{{ rd.smTdcs.refRange }}</span></div>
          </div>
          <div class="dvit-item">
            <div class="t1"><span>AHI指数</span></div>
            <div class="t2"><dv-item :value="rd.hxZtahizs" sign /></div>
            <div class="t3"><span>{{ rd.hxZtahizs.refRange }}</span></div>
          </div>
        </div>

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
        scoreRatio: '',
        smSmsc: { color: '', value: '', refRange: '' },
        smQdsmsc: { color: '', value: '', refRange: '' },
        smSdsmsc: { color: '', value: '', refRange: '' },
        smRemsmsc: { color: '', value: '', refRange: '' },
        hrvXzznl: { color: '', value: '', refRange: '' },
        hxDcpjhx: { color: '', value: '', refRange: '' },
        xlDcpjxl: { color: '', value: '', refRange: '' },
        hxZtcs: { color: '', value: '', refRange: '' },
        smTdcs: { color: '', value: '', refRange: '' },
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
  background: linear-gradient(#ffb24f, #fff)
}

.pt1{
    box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
    background-color: #fff;
    color: #303133;
    -webkit-transition: .3s;
    transition: .3s;
    border-radius: 10px;
    overflow: hidden;
    margin-bottom: 10px;
    margin-top: 80px;
}

.pt1_t1{
width: 100px;
height: 100px;
position: absolute;
right: 100px;
top: 32px;
}

.pt1_t2{
width: 50px;
    height: 50px;
    position: absolute;
    left: 100px;
    top: 62px;
}

.pt1_t3{
    width: 60px;
    height: 60px;
    position: absolute;
    right: 20px;
    top: 22px;
}

.at1{
  height: 300px;
  background: url('~@/assets/images/ts/bg_energy_pt1_at1.png') no-repeat;
  background-size: contain;
}

.dvit{

.dvit-head{
  display: flex;
  color: #ffb24f;
  text-align: center;
  padding: 10px 20px;
  font-size: 14px;
}

.dvit-item{
  display: flex;
  padding: 10px 20px;
  font-size: 14px;
  color: #707070;
}

.t1{
  flex:1;
  text-align: left;
}
.t2{
  flex:1;
  text-align: center;
}
.t3{
  flex:1;
  text-align: center;
}

}

</style>
