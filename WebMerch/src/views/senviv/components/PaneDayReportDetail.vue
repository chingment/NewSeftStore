<template>
  <div id="day_report_detail">

    <table class="clz" cellspacing="0" cellpadding="0">
      <thead>
        <tr>
          <th>类别</th>
          <th>指标</th>
          <th>测量值</th>
          <th>判断</th>
          <th>参考范围</th>
        </tr>
      </thead>
      <tbody>
        <tr><td rowspan="7">睡眠</td></tr>
        <tr>
          <td>实际睡眠时长</td>
          <td><span :style="{'color': rd.smSmsc.color}">{{ rd.smSmsc.value }}</span> </td>
          <td><span :style="{'color': rd.smSmsc.color}">{{ rd.smSmsc.sign }}</span></td>
          <td><span>{{ rd.smSmsc.refRange }}</span></td>
        </tr>
        <tr>
          <td>浅度睡眠</td>
          <td><span :style="{'color': rd.smQdsmsc.color}">{{ rd.smQdsmsc.value }}</span> </td>
          <td><span :style="{'color': rd.smQdsmsc.color}">{{ rd.smQdsmsc.sign }}</span></td>
          <td><span>{{ rd.smQdsmsc.refRange }}</span></td>
        </tr>
        <tr>
          <td>深度睡眠</td>
          <td><span :style="{'color': rd.smSdsmsc.color}">{{ rd.smSdsmsc.value }}</span> </td>
          <td><span :style="{'color': rd.smSdsmsc.color}">{{ rd.smSdsmsc.sign }}</span></td>
          <td><span>{{ rd.smSdsmsc.refRange }}</span></td>
        </tr>
        <tr>
          <td>REM睡眠</td>
          <td><span :style="{'color': rd.smSemqsc.color}">{{ rd.smSemqsc.value }}</span> </td>
          <td><span :style="{'color': rd.smSemqsc.color}">{{ rd.smSemqsc.sign }}</span></td>
          <td><span>{{ rd.smSemqsc.refRange }}</span></td>
        </tr>
        <tr>
          <td>睡眠周期</td>
          <td><span :style="{'color': rd.smSmzq.color}">{{ rd.smSmzq.value }}</span> </td>
          <td><span :style="{'color': rd.smSmzq.color}">{{ rd.smSmzq.sign }}</span></td>
          <td><span>{{ rd.smSmzq.refRange }}</span></td>
        </tr>
        <tr>
          <td>体动次数</td>
          <td><span :style="{'color': rd.smTdcs.color}">{{ rd.smTdcs.value }}</span> </td>
          <td><span :style="{'color': rd.smTdcs.color}">{{ rd.smTdcs.sign }}</span></td>
          <td><span>{{ rd.smTdcs.refRange }}</span></td>
        </tr>

        <tr><td rowspan="4">心率</td></tr>
        <tr>
          <td>当次基准心率</td>
          <td><span :style="{'color': rd.xlDcjzxl.color}">{{ rd.xlDcjzxl.value }}</span> </td>
          <td><span :style="{'color': rd.xlDcjzxl.color}">{{ rd.xlDcjzxl.sign }}</span></td>
          <td><span>{{ rd.xlDcjzxl.refRange }}</span></td>
        </tr>
        <tr>
          <td>长期基准心率</td>
          <td><span :style="{'color': rd.xlCqjzxl.color}">{{ rd.xlCqjzxl.value }}</span> </td>
          <td><span :style="{'color': rd.xlCqjzxl.color}">{{ rd.xlCqjzxl.sign }}</span></td>
          <td><span>{{ rd.xlCqjzxl.refRange }}</span></td>
        </tr>
        <tr>
          <td>当次平均心率</td>
          <td><span :style="{'color': rd.xlDcpjxl.color}">{{ rd.xlDcpjxl.value }}</span> </td>
          <td><span :style="{'color': rd.xlDcpjxl.color}">{{ rd.xlDcpjxl.sign }}</span></td>
          <td><span>{{ rd.xlDcpjxl.refRange }}</span></td>
        </tr>

        <tr><td rowspan="6">呼吸</td></tr>
        <tr>
          <td>当次基准呼吸</td>
          <td><span :style="{'color': rd.hxDcjzhx.color}">{{ rd.hxDcjzhx.value }}</span> </td>
          <td><span :style="{'color': rd.hxDcjzhx.color}">{{ rd.hxDcjzhx.sign }}</span></td>
          <td><span>{{ rd.hxDcjzhx.refRange }}</span></td>
        </tr>
        <tr>
          <td>长期基准呼吸</td>
          <td><span :style="{'color': rd.hxCqjzhx.color}">{{ rd.hxCqjzhx.value }}</span> </td>
          <td><span :style="{'color': rd.hxCqjzhx.color}">{{ rd.hxCqjzhx.sign }}</span></td>
          <td><span>{{ rd.hxCqjzhx.refRange }}</span></td>
        </tr>
        <tr>
          <td>平均呼吸</td>
          <td><span :style="{'color': rd.hxDcPj.color}">{{ rd.hxDcPj.value }}</span> </td>
          <td><span :style="{'color': rd.hxDcPj.color}">{{ rd.hxDcPj.sign }}</span></td>
          <td><span>{{ rd.hxDcPj.refRange }}</span></td>
        </tr>
        <tr>
          <td>呼吸暂停次数</td>
          <td><span :style="{'color': rd.hxZtcs.color}">{{ rd.hxZtcs.value }}</span> </td>
          <td><span :style="{'color': rd.hxZtcs.color}">{{ rd.hxZtcs.sign }}</span></td>
          <td><span>{{ rd.hxZtcs.refRange }}</span></td>
        </tr>
        <tr>
          <td>AHI指数</td>
          <td><span :style="{'color': rd.hxZtAhizs.color}">{{ rd.hxZtAhizs.value }}</span> </td>
          <td><span :style="{'color': rd.hxZtAhizs.color}">{{ rd.hxZtAhizs.sign }}</span></td>
          <td><span>{{ rd.hxZtAhizs.refRange }}</span></td>
        </tr>

        <tr><td rowspan="9">HRV</td></tr>
        <tr>
          <td>心率失常风险</td>
          <td>6h</td>
          <td>x</td>
          <td>6-9h</td>
        </tr>
        <tr>
          <td>心率减力</td>
          <td>6h</td>
          <td>x</td>
          <td>12600~19440s</td>
        </tr>
        <tr>
          <td>心脏总能量</td>
          <td>6h</td>
          <td>x</td>
          <td>3780~8100s</td>
        </tr>
        <tr>
          <td>交感神经张力指数</td>
          <td>6h</td>
          <td>x</td>
          <td>3780~8100s</td>
        </tr>
        <tr>
          <td>迷走神经张力指数</td>
          <td>6h</td>
          <td>x</td>
          <td>3780~8100s</td>
        </tr>
        <tr>
          <td>自主神经平衡指数</td>
          <td>6h</td>
          <td>x</td>
          <td>3780~8100s</td>
        </tr>
        <tr>
          <td>荷尔蒙指数</td>
          <td>6h</td>
          <td>x</td>
          <td>3780~8100s</td>
        </tr>
        <tr>
          <td>体温及血管舒缩指数</td>
          <td>6h</td>
          <td>x</td>
          <td>3780~8100s</td>
        </tr>
      </tbody>
    </table>

  </div>
</template>

<script>

import { MessageBox } from 'element-ui'

import { getDayReportDetail } from '@/api/senviv'

export default {
  name: 'PaneDayReportDetail',
  props: {
    reportId: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      loading: false,
      userInfo: {
        headImgurl: '',
        signName: '',
        sex: '',
        age: '',
        height: '',
        weight: '',
        signTags: []
      },
      rd: {

      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  mounted() {

  },
  created() {
    this._getDayReportDetail()
  },
  methods: {
    _getDayReportDetail() {
      this.loading = true
      getDayReportDetail({ reportId: this.reportId }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.userInfo = d.userInfo
          this.rd = d.reportData
        }
        this.loading = false
      })
    }
  }
}
</script>

<style lang="scss" scoped>

.clz{
    font-size:11px;
            color:#333333;
            border-collapse: collapse;
                   border-top: 1px solid #666666;
        border-left: 1px solid #666666;
    th{
        text-align: center;
            padding: 8px;
            background-color: #dedede;
                    border-right: 1px solid #666666;
        border-bottom: 1px solid #666666;
    }

    td{
        text-align: center;
        padding: 8px;
        background-color: #ffffff;
        border-right: 1px solid #666666;
        border-bottom: 1px solid #666666;
    }
 }

</style>
