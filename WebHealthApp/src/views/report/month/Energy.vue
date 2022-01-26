<template>
  <div class="pg-energy" style="height:500px">

    <div class="pt1">
      <img class="pt1_t1" src="@/assets/images/ts/bg_energy_pt1_t1.png" alt="">
      <img class="pt1_t2" src="@/assets/images/ts/bg_energy_pt1_t2.png" alt="">
      <img class="pt1_t3" src="@/assets/images/ts/bg_energy_pt1_t2.png" alt="">
      <div class="at1">
        <div class="at_a1"><span class="at_title"> 本月综合得分</span><span class="at_score">{{ rd.healthScore }}</span></div>
      </div>
      <div class="at2">
        <div class="dv-section">
          <div ref="chart_ByHealthScore" style="width:100%;height:300px;margin:auto;padding: 10px 5px;" />
        </div>
        <div class="dv-section">
          <div ref="chart_BySm" style="width:100%;height:320px;margin:auto;padding: 10px 5px;" />
        </div>
      </div>
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

var chartByHealthScore
var chartBySm

export default {
  name: 'Energy',
  components: { DvItem },
  data() {
    return {
      loading: false,
      rd: {
        scoreRatio: '',
        healthScore: '',
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
        healthScorePt: [0],
        hrvXzznlPt: [0],
        JbfxXlscfxPt: [0]
      }
    }
  },
  mounted() {
    if (!chartByHealthScore) {
      chartByHealthScore = echarts.init(this.$refs.chart_ByHealthScore, null, { renderer: 'svg' })
    }
    if (!chartBySm) {
      chartBySm = echarts.init(this.$refs.chart_BySm, null, { renderer: 'svg' })
    }
    window.addEventListener('beforeunload', this.clearChart)
  },
  created() {
    this._getEnergy()
  },
  beforeDestroy() {
    if (chartByHealthScore) {
      chartByHealthScore.clear()
      chartByHealthScore.dispose()
      chartByHealthScore = null
    }

    if (chartBySm) {
      chartBySm.clear()
      chartBySm.dispose()
      chartBySm = null
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

          this.rd = d

          this.$nextTick(function() {
            this.getChartByHealthScore()
            this.getChartBySm()
          }, 2000)
        }
        this.loading = false
      })
    },
    clearChart() {
      this.$destroy()
    },
    getChartByHealthScore() {
      var datePt = this.rd.datePt
      var valuePt = this.rd.healthScorePt
      var option = {
        grid: [{
          x: 45,
          y: 50,
          x2: 45,
          y2: 25
        }],
        title: {
          text: '月度生命趋势图', // 主标题
          x: 'center',
          y: 'top'
        },
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
          name: '分数',
          showSymbol: false,
          data: valuePt
        }
        ]
      }

      chartByHealthScore.setOption(option, null)
    },
    getChartBySm() {
      var datePt = this.rd.datePt
      var valuePt1 = this.rd.hrvXzznlPt
      var valuePt2 = this.rd.jbfxXlscfxPt
      var option = {
        grid: [{
          x: 45,
          y: 50,
          x2: 45,
          y2: 50
        }],
        title: {
          text: '月度检测趋势图势图',
          x: 'center',
          y: 'top'
        },
        tooltip: {
          trigger: 'axis'
        },
        legend: {
          data: ['心脏总能量', '心率失常风险指数'],
          // y: 'bottom',
          top: '90%'
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
          name: '心脏总能量',
          showSymbol: false,
          data: valuePt1
        }, {
          type: 'line',
          name: '心率失常风险指数',
          showSymbol: false,
          data: valuePt2
        }
        ]
      }

      chartBySm.setOption(option, null)
    }
  }
}
</script>

<style lang="scss" scoped>

.pg-energy{
  padding: 16px;
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
  height: 150px;
  background: url('~@/assets/images/ts/bg_energy_pt1_at1.png') no-repeat;
  background-size: 100% 100%;

  .at_a1{
        padding-top: 100px;
    padding-left: 20px;
    color:#ffbd73;

  .at_title{
      font-size: 21px;
    }
    .at_score{
      font-size: 32px;
      margin-left: 10px;
    }
  }
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
