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

        <div ref="echart_sm_zx" style="width: 600px;height: 400px;" />
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
    getZxt() {
      // 绘制图表
      var data = [['2000-06-05', 116], ['2000-06-06', 129], ['2000-06-07', 135], ['2000-06-08', 86], ['2000-06-09', 73], ['2000-06-10', 85], ['2000-06-11', 73], ['2000-06-12', 68], ['2000-06-13', 92], ['2000-06-14', 130], ['2000-06-15', 245], ['2000-06-16', 139], ['2000-06-17', 115], ['2000-06-18', 111], ['2000-06-19', 309], ['2000-06-20', 206], ['2000-06-21', 137], ['2000-06-22', 128], ['2000-06-23', 85], ['2000-06-24', 94], ['2000-06-25', 71], ['2000-06-26', 106], ['2000-06-27', 84], ['2000-06-28', 93], ['2000-06-29', 85], ['2000-06-30', 73], ['2000-07-01', 83], ['2000-07-02', 125], ['2000-07-03', 107], ['2000-07-04', 82], ['2000-07-05', 44], ['2000-07-06', 72], ['2000-07-07', 106], ['2000-07-08', 107], ['2000-07-09', 66], ['2000-07-10', 91], ['2000-07-11', 92], ['2000-07-12', 113], ['2000-07-13', 107], ['2000-07-14', 131], ['2000-07-15', 111], ['2000-07-16', 64], ['2000-07-17', 69], ['2000-07-18', 88], ['2000-07-19', 77], ['2000-07-20', 83], ['2000-07-21', 111], ['2000-07-22', 57], ['2000-07-23', 55], ['2000-07-24', 60]]

      var data2 = [['2000-06-05', 16], ['2000-06-06', 29], ['2000-06-07', 35], ['2000-06-08', 6], ['2000-06-09', 3], ['2000-06-10', 5], ['2000-06-11', 73], ['2000-06-12', 68], ['2000-06-13', 92], ['2000-06-14', 130], ['2000-06-15', 245], ['2000-06-16', 139], ['2000-06-17', 115], ['2000-06-18', 111], ['2000-06-19', 309], ['2000-06-20', 206], ['2000-06-21', 137], ['2000-06-22', 128], ['2000-06-23', 85], ['2000-06-24', 94], ['2000-06-25', 71], ['2000-06-26', 106], ['2000-06-27', 84], ['2000-06-28', 93], ['2000-06-29', 85], ['2000-06-30', 73], ['2000-07-01', 83], ['2000-07-02', 125], ['2000-07-03', 107], ['2000-07-04', 82], ['2000-07-05', 44], ['2000-07-06', 72], ['2000-07-07', 106], ['2000-07-08', 107], ['2000-07-09', 66], ['2000-07-10', 91], ['2000-07-11', 92], ['2000-07-12', 113], ['2000-07-13', 107], ['2000-07-14', 131], ['2000-07-15', 111], ['2000-07-16', 64], ['2000-07-17', 69], ['2000-07-18', 88], ['2000-07-19', 77], ['2000-07-20', 83], ['2000-07-21', 111], ['2000-07-22', 57], ['2000-07-23', 55], ['2000-07-24', 60]]

      var dateList = data.map(function(item) {
        return item[0]
      })
      var valueList = data.map(function(item) {
        return item[1]
      })

      var valueList2 = data2.map(function(item) {
        return item[1]
      })

      var myChart = echarts.init(this.$refs.echart_sm_zx, null, { renderer: 'svg' })
      // 指定图表的配置项和数据

      var option = {
        // Make gradient line here
        grid: [{
          bottom: '60%'
        }, {
          top: '60%'
        }, {
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
          data: dateList,
          show: false // 是否显示x轴
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
        }],
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
                  show: true,
                  position: 'bottom'
                },
                symbolSize: [5, 5] // 标记的大小，可以设置成诸如 10 这样单一的数字，也可以用数组分开表示宽和高，例如 [20, 10] 表示标记宽为20，高为10。
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
                  show: true,
                  position: 'bottom'
                },
                symbolSize: [5, 5] // 标记的大小，可以设置成诸如 10 这样单一的数字，也可以用数组分开表示宽和高，例如 [20, 10] 表示标记宽为20，高为10。
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
