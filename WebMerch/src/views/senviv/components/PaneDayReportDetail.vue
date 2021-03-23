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

      var myChart = echarts.init(this.$refs.echart_sm_zx, null, { renderer: 'svg' })
      // 指定图表的配置项和数据

      var option = {
        // 标题
        title: {
          text: '多条折线图测试用例' // 标题名称
        },
        // x轴
        xAxis: {
          data: ['1-1', '1-2', '1-3', '2-1', '3-1', '6-1', '7-1'] // x轴坐标名称
        },
        // y轴
        yAxis: {},
        // 提示框，鼠标悬浮交互时的信息提示
        tooltip: {
          show: true, // 是否显示
          trigger: 'axis' // 触发类型，默认数据触发，见下图，可选为：'item' | 'axis'
        },
        legend: {
          data: ['用例A', '用例B']
        },
        // 指定图标的类型
        series: [
        // 第一条折线图
          {
            name: '用例A', // 系列名称
            type: 'line', // 类型：线
            data: [11, 22, 33, 44, 55, 66, 77, 33, 11, 22, 33, 44, 55, 66, 77, 33], // 数据
            markPoint: {
              // 标注图形数据
              data: [{
                type: 'max', // 类型
                symbol: 'pin', // 标志图形类型，默认自动选择（8种类型循环使用，不显示标志图形可设为'none'），默认循环选择类型有：'circle' | 'rectangle' | 'triangle' | 'diamond' |'emptyCircle' | 'emptyRectangle' | 'emptyTriangle' | 'emptyDiamond' 另外，还支持五种更特别的标志图形'heart'（心形）、'droplet'（水滴）、'pin'（标注）、'arrow'（箭头）和'star'（五角星），这并不出现在常规的8类图形中，但无论是在系列级还是数据级上你都可以指定使用，同时，'star' + n（n>=3)可变化出N角星，如指定为'star6'则可以显示6角星
                name: '最大值'
              },
              {
                type: 'min', // 类型
                symbol: 'pin',
                name: '最小值'
              }
              ],
              // 特殊标注文字
              label: {
                normal: {
                  show: true,
                  // position: 'top', // 文字位置
                  // 显示的文字
                  formatter: '{b}：{c}'
                }
              },
              // 触发操作
              tooltip: {
                show: true, // 是否显示
                formatter: '{b}：{c}', // 内容格式器 a（系列名称），b（类目值），c（数值）, d（无）
                trigger: 'item' // 触发类型，默认数据触发，见下图，可选为：'item' | 'axis'
              }
            },
            markLine: {
              data: [{
                type: 'average',
                name: '平均值',
                itemStyle: {
                  normal: {
                    color: 'orange'
                  }
                }
              }]
            },
            // 折线图圆点
            label: {
              normal: {
                show: true,
                // position: 'bottom', // 文字位置
                // 显示的文字
                formatter: '{c}',
                textStyle: {
                  color: '#000' // 文字颜色
                }
              }
            }
          },
          // 第二条折线图
          {
            name: '用例B', // 系列名称
            type: 'line', // 类型：线
            data: [1, 2, 3, 4, 5, 6, 7, 1], // 数据
            markPoint: {
              // 标注图形数据
              data: [{
                type: 'max', // 类型
                symbol: 'circle', // 标志图形类型，默认自动选择（8种类型循环使用，不显示标志图形可设为'none'），默认循环选择类型有：'circle' | 'rectangle' | 'triangle' | 'diamond' |'emptyCircle' | 'emptyRectangle' | 'emptyTriangle' | 'emptyDiamond' 另外，还支持五种更特别的标志图形'heart'（心形）、'droplet'（水滴）、'pin'（标注）、'arrow'（箭头）和'star'（五角星），这并不出现在常规的8类图形中，但无论是在系列级还是数据级上你都可以指定使用，同时，'star' + n（n>=3)可变化出N角星，如指定为'star6'则可以显示6角星
                name: '最大值'
              },
              {
                type: 'min', // 类型
                symbol: 'circle',
                name: '最小值'
              }
              ],
              // 特殊标注文字
              label: {
                normal: {
                  show: true,
                  // position: 'top', // 文字位置
                  // 显示的文字
                  formatter: '{b}：{c}'
                }
              },
              // 触发操作
              tooltip: {
                show: true, // 是否显示
                formatter: '{b}：{c}', // 内容格式器 a（系列名称），b（类目值），c（数值）, d（无）
                trigger: 'item' // 触发类型，默认数据触发，见下图，可选为：'item' | 'axis'
              }
            },
            markLine: {
              data: [{
                type: 'average',
                name: '平均值',
                itemStyle: {
                  normal: {
                    color: 'green'
                  }
                }
              }]
            },
            // 折线图圆点
            label: {
              normal: {
                show: true,
                // position: 'bottom', // 文字位置
                // 显示的文字
                formatter: '{c}',
                textStyle: {
                  color: '#000' // 文字颜色
                }
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
