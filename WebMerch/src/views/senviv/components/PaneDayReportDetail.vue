<template>
  <div id="day_report_detail" v-loading="loading">
    <div class="row-title clearfix">
      <div class="pull-left"> <h5>数据标签</h5>
      </div>
      <div class="pull-right" />
    </div>

    <div v-if="rd!=null">

      <el-tag
        v-for="tag in rd.dsTags"
        :key="tag"
        style="margin-right: 10px;margin-bottom: 10px"
      >
        {{ tag }}
      </el-tag>
    </div>
    <div ref="echart_sm_zx" style="width: 1150px;height: 400px;" />
    <div class="row-title clearfix">
      <div class="pull-left"> <h5>数据指标</h5>
      </div>
      <div class="pull-right" />
    </div>

    <el-row :gutter="12">
      <el-col :span="6" :xs="24" style="margin-bottom:20px">

        <table v-if="rd!=null" class="clz" cellspacing="0" cellpadding="0">
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
              <td><span :style="{'color': rd.smSemsmsc.color}">{{ rd.smSemsmsc.value }}</span> </td>
              <td><span :style="{'color': rd.smSemsmsc.color}">{{ rd.smSemsmsc.sign }}</span></td>
              <td><span>{{ rd.smSemsmsc.refRange }}</span></td>
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
              <td><span :style="{'color': rd.hxDcpj.color}">{{ rd.hxDcpj.value }}</span> </td>
              <td><span :style="{'color': rd.hxDcpj.color}">{{ rd.hxDcpj.sign }}</span></td>
              <td><span>{{ rd.hxDcpj.refRange }}</span></td>
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
              <td><span :style="{'color': rd.jbfxXlscfx.color}">{{ rd.jbfxXlscfx.value }}</span> </td>
              <td><span :style="{'color': rd.jbfxXlscfx.color}">{{ rd.jbfxXlscfx.sign }}</span></td>
              <td><span>{{ rd.jbfxXlscfx.refRange }}</span></td>
            </tr>
            <tr>
              <td>心率减力</td>
              <td><span :style="{'color': rd.jbfxXljsl.color}">{{ rd.jbfxXljsl.value }}</span> </td>
              <td><span :style="{'color': rd.jbfxXljsl.color}">{{ rd.jbfxXljsl.sign }}</span></td>
              <td><span>{{ rd.jbfxXljsl.refRange }}</span></td>
            </tr>
            <tr>
              <td>心脏总能量</td>
              <td><span :style="{'color': rd.hrvXzznl.color}">{{ rd.hrvXzznl.value }}</span> </td>
              <td><span :style="{'color': rd.hrvXzznl.color}">{{ rd.hrvXzznl.sign }}</span></td>
              <td><span>{{ rd.hrvXzznl.refRange }}</span></td>
            </tr>
            <tr>
              <td>交感神经张力指数</td>
              <td><span :style="{'color': rd.hrvJgsjzlzs.color}">{{ rd.hrvJgsjzlzs.value }}</span> </td>
              <td><span :style="{'color': rd.hrvJgsjzlzs.color}">{{ rd.hrvJgsjzlzs.sign }}</span></td>
              <td><span>{{ rd.hrvJgsjzlzs.refRange }}</span></td>
            </tr>
            <tr>
              <td>迷走神经张力指数</td>
              <td><span :style="{'color': rd.hrvMzsjzlzs.color}">{{ rd.hrvMzsjzlzs.value }}</span> </td>
              <td><span :style="{'color': rd.hrvMzsjzlzs.color}">{{ rd.hrvMzsjzlzs.sign }}</span></td>
              <td><span>{{ rd.hrvMzsjzlzs.refRange }}</span></td>
            </tr>
            <tr>
              <td>自主神经平衡指数</td>
              <td><span :style="{'color': rd.hrvZzsjzlzs.color}">{{ rd.hrvZzsjzlzs.value }}</span> </td>
              <td><span :style="{'color': rd.hrvZzsjzlzs.color}">{{ rd.hrvZzsjzlzs.sign }}</span></td>
              <td><span>{{ rd.hrvZzsjzlzs.refRange }}</span></td>
            </tr>
            <tr>
              <td>荷尔蒙指数</td>
              <td><span :style="{'color': rd.hrvHermzs.color}">{{ rd.hrvHermzs.value }}</span> </td>
              <td><span :style="{'color': rd.hrvHermzs.color}">{{ rd.hrvHermzs.sign }}</span></td>
              <td><span>{{ rd.hrvHermzs.refRange }}</span></td>
            </tr>
            <tr>
              <td>体温及血管舒缩指数</td>
              <td><span :style="{'color': rd.hrvTwjxgsszs.color}">{{ rd.hrvTwjxgsszs.value }}</span> </td>
              <td><span :style="{'color': rd.hrvTwjxgsszs.color}">{{ rd.hrvTwjxgsszs.sign }}</span></td>
              <td><span>{{ rd.hrvTwjxgsszs.refRange }}</span></td>
            </tr>
          </tbody>
        </table>

      </el-col>
      <el-col :span="6" :xs="24" style="margin-bottom:20px">
        <div ref="echart_sm_bi" style="width: 600px;height: 400px;" />
      </el-col>
    </el-row>

  </div>
</template>

<script>

import { MessageBox } from 'element-ui'
import echarts from 'echarts'
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
      rd: null,
      charts: '',
      opinion: ['男', '女'],
      opinionData: [
        { value: 335, name: '男' },
        { value: 310, name: '女' }

      ],
      isDesktop: this.$store.getters.isDesktop
    }
  },
  mounted() {

  },
  created() {
    this._getDayReportDetail()
    // this.getPie()
  },
  methods: {
    _getDayReportDetail() {
      this.loading = true
      getDayReportDetail({ reportId: this.reportId }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.userInfo = d.userInfo
          this.rd = d.reportData

          this.$nextTick(function() {
            this.getPie()
            this.getZxt()
          }, 2000)
        }
        this.loading = false
      })
    },
    getPie() {
      // 绘制图表

      var myChart = echarts.init(this.$refs.echart_sm_bi, null, { renderer: 'svg' })
      // 指定图表的配置项和数据
      var option = {
        // 标题
        title: {
          text: '睡觉结构',
          x: 'left' // 标题位置
          // textStyle: { //标题内容的样式
          //   color: '#000',
          //   fontStyle: 'normal',
          //   fontWeight: 100,
          //   fontSize: 16 //主题文字字体大小，默认为18px
          // },
        },
        // stillShowZeroSum: true,
        // 鼠标划过时饼状图上显示的数据
        tooltip: {
          trigger: 'item',
          formatter: '{a}<br/>{b}:{c} ({d}%)'
        },
        // 图例
        legend: {// 图例  标注各种颜色代表的模块
          // orient: 'vertical',//图例的显示方式  默认横向显示
          bottom: 10, // 控制图例出现的距离  默认左上角
          left: 'center', // 控制图例的位置
          // itemWidth: 16,//图例颜色块的宽度和高度
          // itemHeight: 12,
          textStyle: {// 图例中文字的样式
            color: '#000',
            fontSize: 16
          }
        },
        // 饼图中各模块的颜色
        color: ['#32dadd', '#b6a2de', '#5ab1ef', '#59f5ae', '#1d0fdc'],
        // 饼图数据
        series: {
          // name: 'bug分布',
          type: 'pie', // echarts图的类型   pie代表饼图
          radius: '70%', // 饼图中饼状部分的大小所占整个父元素的百分比
          center: ['50%', '50%'], // 整个饼图在整个父元素中的位置
          // data:''               //饼图数据
          data: this.rd.smPie.data,
          itemStyle: {
            normal: {
              label: {
                show: true // 饼图上是否出现标注文字 标注各模块代表什么  默认是true
                // position: 'inner',//控制饼图上标注文字相对于饼图的位置  默认位置在饼图外
              },
              labelLine: {
                show: true// 官网demo里外部标注上的小细线的显示隐藏    默认显示
              }
            }
          }
        }

      }
      // 使用刚指定的配置项和数据显示图表。
      myChart.setOption(option)
    },
    datetimeFormat(longTypeDate) {
      const date = new Date(longTypeDate * 1000)
      const y = date.getFullYear()
      let MM = date.getMonth() + 1
      MM = MM < 10 ? ('0' + MM) : MM
      let d = date.getDate()
      d = d < 10 ? ('0' + d) : d
      let h = date.getHours()
      h = h < 10 ? ('0' + h) : h
      let m = date.getMinutes()
      m = m < 10 ? ('0' + m) : m
      let s = date.getSeconds()
      s = s < 10 ? ('0' + s) : s
      return y + '-' + MM + '-' + d + ' ' + h + ':' + m + ':' + s
    },
    getZxt() {
      // 绘制图表
      var data = [['2000-06-05', 116], ['2000-06-06', 129], ['2000-06-07', 135], ['2000-06-08', 86], ['2000-06-09', 73], ['2000-06-10', 85], ['2000-06-11', 73], ['2000-06-12', 68], ['2000-06-13', 92], ['2000-06-14', 130], ['2000-06-15', 245], ['2000-06-16', 139], ['2000-06-17', 115], ['2000-06-18', 111], ['2000-06-19', 309], ['2000-06-20', 206], ['2000-06-21', 137], ['2000-06-22', 128], ['2000-06-23', 85], ['2000-06-24', 94], ['2000-06-25', 71], ['2000-06-26', 106], ['2000-06-27', 84], ['2000-06-28', 93], ['2000-06-29', 85], ['2000-06-30', 73], ['2000-07-01', 83], ['2000-07-02', 125], ['2000-07-03', 107], ['2000-07-04', 82], ['2000-07-05', 44], ['2000-07-06', 72], ['2000-07-07', 106], ['2000-07-08', 107], ['2000-07-09', 66], ['2000-07-10', 91], ['2000-07-11', 92], ['2000-07-12', 113], ['2000-07-13', 107], ['2000-07-14', 131], ['2000-07-15', 111], ['2000-07-16', 64], ['2000-07-17', 69], ['2000-07-18', 88], ['2000-07-19', 77], ['2000-07-20', 83], ['2000-07-21', 111], ['2000-07-22', 57], ['2000-07-23', 55], ['2000-07-24', 60]]

      var data2 = [['2000-06-05', 16], ['2000-06-06', 29], ['2000-06-07', 35], ['2000-06-08', 6], ['2000-06-09', 3], ['2000-06-10', 5], ['2000-06-11', 73], ['2000-06-12', 68], ['2000-06-13', 92], ['2000-06-14', 130], ['2000-06-15', 245], ['2000-06-16', 139], ['2000-06-17', 115], ['2000-06-18', 111], ['2000-06-19', 309], ['2000-06-20', 206], ['2000-06-21', 137], ['2000-06-22', 128], ['2000-06-23', 85], ['2000-06-24', 94], ['2000-06-25', 71], ['2000-06-26', 106], ['2000-06-27', 84], ['2000-06-28', 93], ['2000-06-29', 85], ['2000-06-30', 73], ['2000-07-01', 83], ['2000-07-02', 125], ['2000-07-03', 107], ['2000-07-04', 82], ['2000-07-05', 44], ['2000-07-06', 72], ['2000-07-07', 106], ['2000-07-08', 107], ['2000-07-09', 66], ['2000-07-10', 91], ['2000-07-11', 92], ['2000-07-12', 113], ['2000-07-13', 107], ['2000-07-14', 131], ['2000-07-15', 111], ['2000-07-16', 64], ['2000-07-17', 69], ['2000-07-18', 88], ['2000-07-19', 77], ['2000-07-20', 83], ['2000-07-21', 111], ['2000-07-22', 57], ['2000-07-23', 55], ['2000-07-24', 60]]

      var dateList = data.map(function(item) {
        return item[0]
      })
      var valueList = [20, 18, 16, 19, 16, 19, 19, 18, 18, 16, 17, 14, 15, 15, 15, 15, 16, 9, 12, 16, 15, 18, 18, 19, 10, 18, 11, 17, 13, 18, 18, 18, 19, 19, 20, 19, 21, 19, 19, 19, 19, 17, 19, 18, 19, 19, 17, 18, 17, 14, 16, 9, 17, 17, 18, 17, 17, 17, 17, 17, 17, 18, 19, 18, 16, 18, 17, 18, 18, 18, 18, 17, 18, 18, 17, 16, 17, 18, 17, 17, 18, 18, 17, 18, 17, 18, 17, 18, 18, 18, 17, 18, 17, 17, 18, 17, 17, 17, 18, 17, 17, 20, 21, 18, 20, 20, 17, 17, 18, 17, 18, 13, 10, 14, 18, 18, 18, 18, 17, 18, 17, 17, 17, 9, 17, 15, 16, 18, 17, 16, 17, 17, 17, 17, 17, 15, 17, 10, 17, 16, 16, 17, 17, 16, 17, 16, 17, 17, 17, 18, 16, 16, 17, 16, 16, 17, 17, 17, 16, 16, 17, 16, 17, 17, 17, 19, 17, 17, 17, 17, 20, 15, 16, 17, 18, 17, 18, 21, 18, 18, 16, 17, 18, 16, 17, 17, 17, 17, 16, 16, 16, 16, 16, 8, 10, 17, 17, 15, 15, 16, 16, 15, 15, 16, 15, 17, 15, 16, 16, 16, 16, 15, 16, 16, 15, 16, 18, 17, 16, 17, 17, 15, 16, 15, 15, 17, 16, 14, 15, 16, 16, 17, 16, 16, 15, 16, 15, 16, 16, 16, 17, 17, 16, 16, 16, 16, 15, 16, 16, 15, 15, 16, 16, 15, 16, 11, 16, 10, 17, 16, 16, 15, 16, 12, 13, 16, 17, 16, 14, 17, 18, 16, 16, 16, 16, 16, 15, 12, 15, 16, 16, 16, 16, 16, 13, 11, 17, 16, 16, 15, 15, 14, 15, 15, 16, 16, 15, 15, 15, 16, 16, 10, 16, 14, 15, 11, 16, 17, 16, 17, 17, 17, 18, 18, 17, 16, 16, 17, 17, 17, 16, 17, 17, 17, 17, 18, 17, 18, 18, 17, 16, 17, 16, 18, 19, 19, 18, 18, 18, 19, 20, 19, 19, 21, 16, 17, 15, 15, 15, 15, 15, 15, 15, 15, 16, 16, 16, 15, 15, 15, 16, 16, 16, 16, 16, 15, 16, 16, 15, 15, 15, 16, 16, 16, 15, 15, 16, 15, 15, 15, 15, 15, 17, 17, 16, 16, 16, 16, 16, 16, 16, 17, 19, 18, 18, 18, 16, 10, 14, 16, 15, 14, 15, 15, 15, 16, 10, 12, 11, 12, 14]

      var valueList2 = [76, 76, 76, 74, 71, 68, 60, 58, 59, 59, 59, 59, 60, 60, 60, 60, 60, 60, 60, 61, 61, 60, 60, 61, 61, 61, 62, 62, 62, 60, 60, 60, 61, 63, 63, 64, 64, 64, 64, 64, 64, 62, 62, 63, 63, 63, 62, 63, 63, 63, 63, 62, 61, 62, 63, 63, 61, 62, 61, 62, 61, 60, 59, 58, 57, 59, 57, 58, 57, 57, 57, 57, 57, 58, 58, 58, 58, 58, 58, 59, 59, 59, 58, 58, 58, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 56, 56, 57, 56, 57, 59, 63, 60, 59, 61, 62, 61, 55, 59, 58, 60, 61, 62, 62, 62, 60, 59, 58, 58, 58, 57, 57, 56, 55, 55, 55, 57, 58, 58, 58, 57, 57, 57, 56, 56, 55, 54, 54, 53, 53, 53, 53, 53, 53, 53, 53, 52, 54, 55, 55, 55, 55, 54, 54, 59, 58, 57, 55, 53, 54, 52, 51, 53, 53, 52, 52, 52, 52, 51, 55, 57, 58, 53, 50, 51, 51, 56, 54, 53, 52, 51, 51, 53, 51, 52, 52, 52, 51, 52, 51, 52, 52, 55, 57, 60, 59, 59, 56, 55, 55, 54, 53, 53, 53, 54, 54, 54, 54, 52, 54, 53, 53, 53, 53, 52, 53, 55, 54, 54, 53, 52, 52, 52, 52, 52, 51, 53, 52, 52, 52, 52, 52, 52, 52, 52, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 52, 52, 53, 53, 52, 51, 50, 51, 52, 55, 54, 50, 51, 52, 52, 52, 56, 52, 50, 51, 50, 51, 52, 53, 52, 54, 55, 56, 55, 55, 57, 57, 56, 55, 55, 55, 56, 55, 55, 55, 55, 54, 54, 54, 54, 54, 54, 53, 51, 52, 52, 52, 55, 56, 56, 58, 59, 61, 58, 58, 56, 56, 55, 56, 55, 55, 54, 55, 53, 54, 52, 56, 56, 54, 55, 57, 56, 56, 55, 55, 56, 57, 55, 56, 56, 52, 54, 55, 54, 57, 55, 56, 56, 56, 56, 56, 55, 56, 55, 55, 55, 55, 55, 55, 54, 55, 54, 54, 55, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 55, 56, 56, 56, 56, 56, 55, 55, 53, 54, 54, 57, 56, 53, 56, 58, 61, 54, 54, 53, 53, 53, 53, 55, 53, 53, 53, 53, 52]

      var my_data = [{
        'starttime': 1615914256,
        'endtime': 1615917625,
        'type': 0
      }, {
        'starttime': 1615917626,
        'endtime': 1615917628,
        'type': 1
      }, {
        'starttime': 1615917629,
        'endtime': 1615918039,
        'type': 5
      }, {
        'starttime': 1615918040,
        'endtime': 1615918245,
        'type': 1
      }, {
        'starttime': 1615918246,
        'endtime': 1615918451,
        'type': 5
      }, {
        'starttime': 1615918452,
        'endtime': 1615918656,
        'type': 1
      }, {
        'starttime': 1615918657,
        'endtime': 1615918862,
        'type': 6
      }, {
        'starttime': 1615918863,
        'endtime': 1615919479,
        'type': 5
      }, {
        'starttime': 1615919480,
        'endtime': 1615919685,
        'type': 1
      }, {
        'starttime': 1615919686,
        'endtime': 1615920324,
        'type': 5
      }, {
        'starttime': 1615920325,
        'endtime': 1615921235,
        'type': 4
      }, {
        'starttime': 1615921236,
        'endtime': 1615921314,
        'type': 3
      }, {
        'starttime': 1615921315,
        'endtime': 1615921485,
        'type': 0
      }, {
        'starttime': 1615921486,
        'endtime': 1615921825,
        'type': 4
      }, {
        'starttime': 1615921826,
        'endtime': 1615921952,
        'type': 5
      }, {
        'starttime': 1615921953,
        'endtime': 1615922652,
        'type': 1
      }, {
        'starttime': 1615922653,
        'endtime': 1615923353,
        'type': 5
      }, {
        'starttime': 1615923354,
        'endtime': 1615923586,
        'type': 1
      }, {
        'starttime': 1615923587,
        'endtime': 1615923820,
        'type': 5
      }, {
        'starttime': 1615923821,
        'endtime': 1615923924,
        'type': 1
      }, {
        'starttime': 1615923925,
        'endtime': 1615925655,
        'type': 4
      }, {
        'starttime': 1615925656,
        'endtime': 1615926215,
        'type': 0
      }, {
        'starttime': 1615926216,
        'endtime': 1615926296,
        'type': 3
      }, {
        'starttime': 1615926297,
        'endtime': 1615926855,
        'type': 0
      }, {
        'starttime': 1615926856,
        'endtime': 1615926879,
        'type': 5
      }, {
        'starttime': 1615926880,
        'endtime': 1615927113,
        'type': 1
      }, {
        'starttime': 1615927114,
        'endtime': 1615927815,
        'type': 5
      }, {
        'starttime': 1615927816,
        'endtime': 1615928282,
        'type': 1
      }, {
        'starttime': 1615928283,
        'endtime': 1615930153,
        'type': 5
      }, {
        'starttime': 1615930154,
        'endtime': 1615930224,
        'type': 1
      }, {
        'starttime': 1615930225,
        'endtime': 1615931055,
        'type': 4
      }, {
        'starttime': 1615931056,
        'endtime': 1615931616,
        'type': 0
      }, {
        'starttime': 1615931617,
        'endtime': 1615932179,
        'type': 3
      }, {
        'starttime': 1615932180,
        'endtime': 1615932470,
        'type': 0
      }, {
        'starttime': 1615932471,
        'endtime': 1615932664,
        'type': 3
      }, {
        'starttime': 1615932665,
        'endtime': 1615932771,
        'type': 0
      }, {
        'starttime': 1615932772,
        'endtime': 1615933311,
        'type': 1
      }, {
        'starttime': 1615933312,
        'endtime': 1615933671,
        'type': 5
      }, {
        'starttime': 1615933672,
        'endtime': 1615933851,
        'type': 1
      }, {
        'starttime': 1615933852,
        'endtime': 1615934031,
        'type': 6
      }, {
        'starttime': 1615934032,
        'endtime': 1615934055,
        'type': 1
      }, {
        'starttime': 1615934056,
        'endtime': 1615934284,
        'type': 0
      }, {
        'starttime': 1615934285,
        'endtime': 1615934386,
        'type': 3
      }, {
        'starttime': 1615934387,
        'endtime': 1615934655,
        'type': 0
      }, {
        'starttime': 1615934656,
        'endtime': 1615934777,
        'type': 6
      }, {
        'starttime': 1615934778,
        'endtime': 1615935484,
        'type': 1
      }, {
        'starttime': 1615935485,
        'endtime': 1615935624,
        'type': 6
      }, {
        'starttime': 1615935625,
        'endtime': 1615937125,
        'type': 4
      }, {
        'starttime': 1615937126,
        'endtime': 1615938546,
        'type': 5
      }, {
        'starttime': 1615938547,
        'endtime': 1615939184,
        'type': 1
      }, {
        'starttime': 1615939185,
        'endtime': 1615939725,
        'type': 0
      }, {
        'starttime': 1615939726,
        'endtime': 1615940165,
        'type': 3
      }, {
        'starttime': 1615940166,
        'endtime': 1615941148,
        'type': 0
      }]

      var my_xAxis = []
      var my_xAxis_data = []
      for (let index = 0; index < my_data.length; index++) {
        const var1 = my_data[index].starttime
        const var2 = my_data[index].endtime
        var type = my_data[index].type

        var var3
        if (type === 0) {
          var3 = 'W'
        } else if (type === 1) {
          var3 = 'N3'
        } else if (type === 5) {
          var3 = 'N1'
        } else if (type === 4) {
          var3 = 'R'
        } else if (type === 3) {
          var3 = 'O'
        } else if (type === 6) {
          var3 = 'N2'
        }

        my_xAxis.push(this.datetimeFormat(var1))
        my_xAxis.push(this.datetimeFormat(var2))

        my_xAxis_data.push({ value: [var3, this.datetimeFormat(var1), this.datetimeFormat(var2)] })
      }

      // my_xAxis=['03-17 01:04:16','']
      console.log(JSON.stringify(my_xAxis))
      console.log(JSON.stringify(my_xAxis_data))

      var myChart = echarts.init(this.$refs.echart_sm_zx, null, { renderer: 'svg' })
      // 指定图表的配置项和数据

      var option = {
        // Make gradient line here
        grid: [{
          show: false,
          borderWidth: 1
          // borderColor: '#FF0000' // 网格的边框颜色
        }],
        // 视觉映射组件，用于进行『视觉编码』
        visualMap: [{
          show: false, // 是否显示 visualMap-piecewise 组件。如果设置为 false，不会显示，但是数据映射的功能还存在。
          type: 'continuous', // 定义为连续型 visualMap
          seriesIndex: 0, // 指定取哪个系列的数据，即哪个系列的 series.data。默认取所有系列
          min: 0, // 指定 visualMapPiecewise 组件的最小值。
          max: 400
        }],
        title: [{
          left: 'center',
          text: 'Gradient along the y axis'
        }],
        // 提示框组件
        tooltip: {
          trigger: 'axis' // 触发类型。坐标轴触发，主要在柱状图，折线图等会使用类目轴的图表中使用/none什么都不触发
        },
        xAxis: [{
          data: my_xAxis,
          show: true // 是否显示x轴
        },
        {
          data: my_xAxis,
          show: true, // 是否显示x轴
          axisTick: {
            alignWithLabel: true
          },
          axisLabel: {
            showMaxLabel: true
          }
        }],
        yAxis: [{
          nameTextStyle: { // 坐标轴名称的文字样式。
            color: '#63B8FF',
            fontWeight: 'bold', // 坐标轴名称文字字体的粗细
            fontSize: 15
          },
          name: '心率次数/bmp',
          splitLine: {
            show: true, // x轴、y轴显示网格线,坐标轴在 grid 区域中的分隔线
            lineStyle: {
              // 使用深浅的间隔色--分隔线颜色，可以设置成单个颜色。也可以设置成颜色数组，分隔线会按数组中颜色的顺序依次循环设置颜色。
              color: ['#4F5258', '#30394F']
            }
          }
        }
        ],
        // 系列列表。每个系列通过 type 决定自己的图表类型
        series: [{
          type: 'line', // 线条
          showSymbol: false, // 是否显示 symbol符号, 如果 false 则只有在 tooltip hover 的时候显示。
          data: valueList,
          lineStyle: { color: '#BA3945' }, // 线条样式
          markPoint: { // 图表标注
            data: [
              {
                type: 'max',
                name: '最高心率',
                label: {
                  color: '#CCCCCC',
                  show: true
                }
              },
              {
                type: 'min',
                name: '最小心率',
                label: {
                  color: '#CCCCCC',
                  show: true
                }
              }
            ],
            // symbol: 'none', // 标记的图形。circle-圆形,rect-方形
            label: { // 标注的文本
              show: false
            },
            itemStyle: {
              color: '#081944', // 图形的颜色--设置这个是为了隐藏掉图标
              opacity: 1 // 图形透明度。支持从 0 到 1 的数字，为 0 时不绘制该图形。
            }
          }
        },

        {
          type: 'line', // 线条
          showSymbol: false, // 是否显示 symbol符号, 如果 false 则只有在 tooltip hover 的时候显示。
          data: valueList2,
          lineStyle: { color: '#BA3945' }, // 线条样式
          markPoint: { // 图表标注
            data: [
              {
                type: 'max',
                name: '最高心率',
                label: {
                  color: '#CCCCCC',
                  show: true
                }
              },
              {
                type: 'min',
                name: '最小心率',
                label: {
                  color: '#CCCCCC',
                  show: true
                }
              }
            ],
            // symbol: 'none', // 标记的图形。circle-圆形,rect-方形
            label: { // 标注的文本
              show: true
            },
            itemStyle: {
              color: '#081944', // 图形的颜色--设置这个是为了隐藏掉图标
              opacity: 1 // 图形透明度。支持从 0 到 1 的数字，为 0 时不绘制该图形。
            }
          }
        }
        ]
      }

      option = {
        // Make gradient line here

        grid: [{
          bottom: '50%'
        }, {
          top: '50%'
        }, {
          show: false,
          borderWidth: 1,
          borderColor: '#FF0000' // 网格的边框颜色
        }],
        // 视觉映射组件，用于进行『视觉编码』
        visualMap: [{
          show: false, // 是否显示 visualMap-piecewise 组件。如果设置为 false，不会显示，但是数据映射的功能还存在。
          type: 'continuous', // 定义为连续型 visualMap
          seriesIndex: 0, // 指定取哪个系列的数据，即哪个系列的 series.data。默认取所有系列
          min: 0, // 指定 visualMapPiecewise 组件的最小值。
          max: 400
        }, {
          show: false,
          type: 'continuous',
          seriesIndex: 1,
          dimension: 0,
          min: 0,
          max: dateList.length - 1
        }],
        title: [{
          left: 'center',
          text: 'Gradient along the y axis'
        }],
        // 提示框组件
        tooltip: {
          trigger: 'axis' // 触发类型。坐标轴触发，主要在柱状图，折线图等会使用类目轴的图表中使用/none什么都不触发
        },
        xAxis: [{
          data: valueList,
          show: false // 是否显示x轴
        }, {
          data: my_xAxis,

          gridIndex: 1, // x 轴所在的 grid 的索引，默认位于第一个 grid。
          axisTick: {
            alignWithLabel: true
          },
          axisLabel: {
            interval: 0,
            rotate: 30,
            formatter: function(val) {
              console.log(val)

              if (val == '2021-03-17 01:04:16') {
                return '上床'
              } else if (val == '2021-03-17 02:00:26') {
                return '入眠'
              } else if (val == '2021-03-17 07:59:44') {
                return '清醒'
              } else if (val == '2021-03-17 07:00:59') {
                return '起床'
              }
              return ''
            }

          }
        }],
        yAxis: [{
          nameTextStyle: { // 坐标轴名称的文字样式。
            color: '#63B8FF',
            fontWeight: 'bold', // 坐标轴名称文字字体的粗细
            fontSize: 15
          },
          name: '心率次数',
          splitLine: {
            show: true
          }
        }, {
          splitLine: { show: false },
          gridIndex: 1,
          data: ['N3', 'N2', 'N1', 'R', 'W', 'O']
        }],
        // 系列列表。每个系列通过 type 决定自己的图表类型
        series: [{
          type: 'line', // 线条
          showSymbol: false, // 是否显示 symbol符号, 如果 false 则只有在 tooltip hover 的时候显示。
          data: valueList
        }, {
          type: 'line', // 线条
          showSymbol: false, // 是否显示 symbol符号, 如果 false 则只有在 tooltip hover 的时候显示。
          data: valueList2
        },
        {
          xAxisIndex: 1,
          yAxisIndex: 1,
          type: 'custom',
          renderItem: function(params, api) {
            var categoryIndex = api.value(0)

            // console.log(categoryIndex + ',' + api.value(1) + ',' + api.value(2))

            var start = api.coord([api.value(1), categoryIndex])

            var end = api.coord([api.value(2), categoryIndex])

            // console.log(start + ',' + end)
            var height = 12

            return {
              type: 'rect',
              shape: echarts.graphic.clipRectByRect({
                x: start[0],
                y: start[1] - height / 2,
                width: end[0] - start[0],
                height: height
              }, {
                x: params.coordSys.x,
                y: params.coordSys.y,
                width: params.coordSys.width,
                height: params.coordSys.height
              }),
              style: api.style()
            }
          },
          encode: {
            x: [1, 2],
            y: 0
          },
          data: my_xAxis_data
        }]
      }
      // 使用刚指定的配置项和数据显示图表。
      myChart.setOption(option)
    }
  }
}
</script>

<style lang="scss" scoped>

.clz{
    font-size:11px;
            color:#333333;
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

  #main{
 width: 100%;
 }

</style>
