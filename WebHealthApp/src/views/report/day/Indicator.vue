<template>
  <div class="pg-indicator">

    <div class="pt1" style="padding:14px">
      <div ref="chartByMv" style="width: 100%;height: 320px;margin:auto" />

      <div ref="chartByTd" style="width: 100%;height: 150px;margin:auto" />

      <div class="chartByMv-Hxzt"><span class="item">{{ rd.hxZtcs.value }}次呼吸暂停</span></div>

      <div class="chartByMv-Legend">
        <div class="ld"> <span class="n-c" :style="' background-color:'+ n3_clr+';'" /> <span class="n-n">深睡</span></div>
        <div class="ld"> <span class="n-c" :style="' background-color:'+ n2_clr+';'" /> <span class="n-n">浅睡</span></div>
        <div class="ld"> <span class="n-c" :style="' background-color:'+ r_clr+';'" /> <span class="n-n">REM</span></div>
        <div class="ld"> <span class="n-c" :style="' background-color:'+ n1_clr+';'" /> <span class="n-n">清醒</span></div>
        <div class="ld"> <span class="n-c" :style="' background-color:'+ o_clr+';'" /> <span class="n-n">离枕</span></div>
      </div>
    </div>

    <div class="line" />
    <div class="pt2" style="padding:0 14px 14px 14px">

      <mt-navbar v-model="active_tag_point" class="tag-point">
        <mt-tab-item id="tag_point_1">睡眠</mt-tab-item>
        <mt-tab-item id="tag_point_2">心率</mt-tab-item>
        <mt-tab-item id="tag_point_3">呼吸</mt-tab-item>
        <mt-tab-item id="tag_point_4">HRV</mt-tab-item>
      </mt-navbar>
      <mt-tab-container v-model="active_tag_point">
        <mt-tab-container-item id="tag_point_1">
          <score-level :tag-dv="rd.smSmsc" :is-collapse="false" :enable-collapse="true" />
          <score-level :tag-dv="rd.smSmxl" :is-collapse="false" :enable-collapse="true" />
          <score-level :tag-dv="rd.smRsxs" :is-collapse="false" :enable-collapse="true" />
          <score-level :tag-dv="rd.smSmlxx" :is-collapse="false" :enable-collapse="true" />
          <score-level :tag-dv="rd.smSmzq" :is-collapse="false" :enable-collapse="true" />
          <score-level :tag-dv="rd.smQdsmbl" :is-collapse="false" :enable-collapse="true" />
          <score-level :tag-dv="rd.smSdsmbl" :is-collapse="false" :enable-collapse="true" />
          <score-level :tag-dv="rd.smRemsmbl" :is-collapse="false" :enable-collapse="true" />
          <score-level :tag-dv="rd.smLzcs" :is-collapse="false" :enable-collapse="true" />
          <score-level :tag-dv="rd.smTdcs" :is-collapse="false" :enable-collapse="true" />
        </mt-tab-container-item>
        <mt-tab-container-item id="tag_point_2">
          <score-level :tag-dv="rd.xlDcjzxl" :is-collapse="false" :enable-collapse="true" />
          <score-level :tag-dv="rd.xlCqjzxl" :is-collapse="false" :enable-collapse="true" />
          <score-level :tag-dv="rd.xlDcpjxl" :is-collapse="false" :enable-collapse="true" />
        </mt-tab-container-item>
        <mt-tab-container-item id="tag_point_3">
          <score-level :tag-dv="rd.hxDcjzhx" :is-collapse="false" :enable-collapse="true" />
          <score-level :tag-dv="rd.hxCqjzhx" :is-collapse="false" :enable-collapse="true" />
          <score-level :tag-dv="rd.hxDcpjhx" :is-collapse="false" :enable-collapse="true" />
          <score-level :tag-dv="rd.hxZtahizs" :is-collapse="false" :enable-collapse="true" />
          <score-level :tag-dv="rd.hxZtcs" :is-collapse="false" :enable-collapse="true" />
        </mt-tab-container-item>
        <mt-tab-container-item id="tag_point_4">
          <score-level :tag-dv="rd.hrvXzznl" :is-collapse="false" :enable-collapse="true" />
          <score-level :tag-dv="rd.hrvJgsjzlzs" :is-collapse="false" :enable-collapse="true" />
          <score-level :tag-dv="rd.hrvMzsjzlzs" :is-collapse="false" :enable-collapse="true" />
          <score-level :tag-dv="rd.hrvZzsjzlzs" :is-collapse="false" :enable-collapse="true" />
          <score-level :tag-dv="rd.jbfxXlscfx" :is-collapse="false" :enable-collapse="true" />
          <score-level :tag-dv="rd.jbfxXljsl" :is-collapse="false" :enable-collapse="true" />
        </mt-tab-container-item>
      </mt-tab-container>
    </div>

  </div>
</template>

<script>
import echarts from 'echarts'
import { getIndicator } from '@/api/dayreport'
import ScoreLevel from '@/components/ScoreLevel.vue'
import DvCollapse from '@/components/DvCollapse/Index.vue'
var chartByMv
var chartByTd
export default {
  name: '',
  components: {
    DvCollapse,
    ScoreLevel
  },
  data() {
    return {
      loading: false,
      rptId: '',
      rd: {
        hxPoint: [],
        xlPoint: [],
        smPoint: []
      },
      w_clr: '#d8edff',
      n1_clr: '#a9ddfd',
      n3_clr: '#4c6ce5',
      r_clr: '#e6affd',
      o_clr: '#ffdf24',
      n2_clr: '#a9ddfd',
      active_tag_point: 'tag_point_1'
    }
  },
  created() {
    this.rptId = this.$route.query.rptId

    this.loading = true
    getIndicator({ rptId: this.rptId }).then(res => {
      if (res.result === 1) {
        var d = res.data

        this.rd = d.rd

        this.$nextTick(function() {
          this.onGetChartByMv()
          this.onGetChartByTd()
        }, 500)
      }

      this.loading = false
    })
  },
  beforeDestroy() {
    if (chartByMv) {
      chartByMv.clear()
      chartByMv.dispose()
      chartByMv = null
    }

    if (chartByTd) {
      chartByTd.clear()
      chartByTd.dispose()
      chartByTd = null
    }
  },
  methods: {
    onGetChartByMv() {
      if (!chartByMv) {
        chartByMv = echarts.init(this.$refs.chartByMv, null, { renderer: 'svg' })
      }
      if (this.rd.hxPoint === null) { return }

      var hx_point_d = this.rd.hxPoint.dataValue

      if (hx_point_d === null) { return }

      var xl_point_d = this.rd.xlPoint.dataValue

      var sm_point_d = this.rd.smPoint.dataValue

      var sm_point_x_min_t = this.datetimeFormat2(this.rd.smPoint.startTime, -1000 * 60)
      var sm_point_x_max_t = this.datetimeFormat2(this.rd.smPoint.endTime, +1000 * 60)
      var sc_time = this.rd.smScsj
      var rs_time = this.rd.smRssj
      var qx_time = this.rd.smQxsj
      var lc_time = this.rd.smLcsj

      var sm_point_xAxis = []
      var sm_point_xAxis_data = []
      for (let index = 0; index < sm_point_d.length; index++) {
        const var1 = sm_point_d[index].startTime
        const var2 = sm_point_d[index].endTime
        var type = sm_point_d[index].type

        var var3
        var color = ''

        if (type === 0) {
          var3 = 'W'
          color = this.w_clr
        } else if (type === 1) {
          var3 = 'N1'
          color = this.n1_clr
        } else if (type === 5) {
          var3 = 'N3'
          color = this.n3_clr
        } else if (type === 4) {
          var3 = 'R'
          color = this.r_clr
        } else if (type === 3) {
          var3 = 'O'
          color = this.o_clr
        } else if (type === 6) {
          var3 = 'N2'
          color = this.n2_clr
        }

        sm_point_xAxis.push(this.datetimeFormat(var1))
        sm_point_xAxis.push(this.datetimeFormat(var2))

        sm_point_xAxis_data.push({ itemStyle: { normal: { color: color }}, value: [var3, this.datetimeFormat(var1), this.datetimeFormat(var2)] })
      }

      var option = {
        grid: [{
          bottom: '50%',
          x: 28,
          y: 50,
          x2: 28,
          y2: 50
        }, {
          top: '50%',
          x: 28,
          y: 50,
          x2: 28,
          y2: 50

        }, {
          show: false,
          borderWidth: 1,
          borderColor: '#FF0000',
          x: 50,
          y: 50,
          x2: 50,
          y2: 50
        }],
        title: [{
          left: 'center',
          text: ''
        }],
        legend: {
          show: false,
          data: ['呼吸', '心率', 'N3', 'N2', 'N1', 'R', 'W', 'O']
        },
        tooltip: {
          trigger: 'axis',
          formatter: function(params) {
            if (params[0].componentSubType === 'line') {
              return ' 心率：' + params[1].data + '<br/>' + ' 呼吸：' + params[0].data
            } else if (params[0].componentSubType === 'custom') {
              var html = ''
              for (let index = 0; index < params.length; index++) {
                html += params[index].value[1] + '-' + params[index].value[2] + ',' + params[index].value[0] + '<BR/>'
              }
              // var param = params[params.length - 1]
              // console.log(JSON.stringify(param))
              return html
            }
          }
        },
        xAxis: [{
          data: hx_point_d,
          show: false
        }, {
          gridIndex: 1,
          axisTick: {
            show: false,
            height: 0
          },
          type: 'time',
          rotate: 0,
          splitLine: { show: false },
          interval: 60 * 1000,
          min: sm_point_x_min_t,
          max: sm_point_x_max_t,
          axisLabel: {
            textStyle: {
              align: 'center'
              // verticalAlign: 'bottom'
            },
            interval: 0,
            padding: [-8, 0, 0, 0],
            formatter: function(value) {
              function datetimeFormat(longTypeDate) {
                const date = new Date(longTypeDate)
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
                return y + '/' + MM + '/' + d + ' ' + h + ':' + m
              }

              function getMin(longTypeDate) {
                const date = new Date(longTypeDate)
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
                return h + ':' + m
              }

              if (datetimeFormat(value) === sc_time) {
                // console.log('上床')
                return '|\n|\n|\n上床 ' + getMin(value)
              } else if (datetimeFormat(value) === rs_time) {
                // console.log('入睡')
                return '|\n入睡 ' + getMin(value)
              } else if (datetimeFormat(value) === qx_time) {
                // console.log('清醒')
                return '|\n清醒 ' + getMin(value)
              } else if (datetimeFormat(value) === lc_time) {
                // console.log('离床')
                return '|\n|\n|\n离床 ' + getMin(value)
              }
              return ''
            },
            rich: {
              // 这里的rich，下面有解释
              'SC': {
                width: 1,
                height: 100,
                borderColor: '#000',
                verticalAlign: 'bottom',
                borderWidth: 0.5
              },
              'RS': {
                verticalAlign: 'bottom',
                width: 0,
                height: 30,
                borderColor: '#000',
                borderWidth: 0.5
              },
              'QX': {
                align: 'left',
                width: 0,
                height: 30,
                borderColor: '#000',
                borderWidth: 0.5
              },
              'QC': {
                align: 'right',
                verticalAlign: 'right',
                width: 100,
                height: 100,
                borderColor: '#000',
                borderWidth: 0.5
              }
            }

          }
        }],
        yAxis: [{
          nameTextStyle: {
            color: '#63B8FF',
            fontWeight: 'bold',
            fontSize: 15
          },
          name: '',
          splitLine: {
            show: true
          }
        }, {
          splitLine: { show: false },
          gridIndex: 1,
          data: ['N3', 'N2', 'N1', 'R', 'W', 'O']
        }],
        series: [
          { name: 'N3', type: 'bar', data: [], itemStyle: {
            normal: {
              color: '#4c6ce5',
              lineStyle: {
                color: '#4c6ce5'
              }
            }
          }},
          { name: 'N2', type: 'bar', data: [], itemStyle: {
            normal: {
              color: '#a9ddfd',
              lineStyle: {
                color: '#a9ddfd'
              }
            }
          }},
          { name: 'N1', type: 'bar', data: [], itemStyle: {
            normal: {
              color: '#a9ddfd',
              lineStyle: {
                color: '#a9ddfd'
              }
            }
          }},
          { name: 'R', type: 'bar', data: [], itemStyle: {
            normal: {
              color: '#e6affd',
              lineStyle: {
                color: '#e6affd'
              }
            }
          }},
          { name: 'W', type: 'bar', data: [], itemStyle: {
            normal: {
              color: '#d8edff',
              lineStyle: {
                color: '#d8edff'
              }
            }
          }},
          { name: 'O', type: 'bar', data: [], itemStyle: {
            normal: {
              color: '#ffdf24',
              lineStyle: {
                color: '#ffdf24'
              }
            }
          }},
          {
            type: 'line',
            name: '呼吸',
            showSymbol: false,
            data: hx_point_d,
            itemStyle: {
              normal: {
                color: '#2f2ffa',
                lineStyle: {
                  color: '#2f2ffa'
                }
              }
            },
            markPoint: {
              data: [
                {
                  type: 'max',
                  name: '最高呼吸',
                  label: {
                    color: '#2f2ffa',
                    show: true
                  }
                },
                {
                  type: 'min',
                  name: '最低呼吸',
                  label: {
                    color: '#2f2ffa',
                    show: true
                  }
                }
              ],
              label: {
                show: false
              },
              itemStyle: {
                color: '#6a6afc',
                opacity: 1
              }
            }
          }, {
            name: '心率',
            type: 'line',
            showSymbol: false,
            data: xl_point_d,
            itemStyle: {
              normal: {
                color: '#ffa500',
                lineStyle: {
                  color: '#ffa500'
                }
              }
            },
            markPoint: {
              data: [
                {
                  type: 'max',
                  name: '最大心率',
                  label: {
                    color: '#ffa500',
                    show: true
                  }
                },
                {
                  type: 'min',
                  name: '最小心率',
                  label: {
                    color: '#ffa500',
                    show: true
                  }
                }
              ],
              label: {
                show: false
              },
              itemStyle: {
                color: '#081944',
                opacity: 1
              }
            }
          },
          {
            xAxisIndex: 1,
            yAxisIndex: 1,
            type: 'custom',
            renderItem: function(params, api) {
              var categoryIndex = api.value(0)
              var start = api.coord([api.value(1), categoryIndex])
              var end = api.coord([api.value(2), categoryIndex])
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
            data: sm_point_xAxis_data
          }]
      }

      chartByMv.setOption(option, null)
    },
    onGetChartByTd() {
      if (!chartByTd) {
        chartByTd = echarts.init(this.$refs.chartByTd, null, { renderer: 'svg' })
      }

      var sm_point_d = this.rd.smTdcsPoint

      var sm_point_x_min_t = this.datetimeFormat2(this.rd.smPoint.startTime, -1000 * 60)
      var sm_point_x_max_t = this.datetimeFormat2(this.rd.smPoint.endTime, +1000 * 60)
      var sc_time = this.rd.smScsj
      var rs_time = this.rd.smRssj
      var qx_time = this.rd.smQxsj
      var lc_time = this.rd.smLcsj

      var sm_point_xAxis = []
      var sm_point_xAxis_data = []
      for (let index = 0; index < sm_point_d.length; index++) {
        const var1 = sm_point_d[index].startTime
        const var2 = sm_point_d[index].endTime
        var var3 = 'N3'
        var color = this.n3_clr
        sm_point_xAxis_data.push({ itemStyle: { normal: { color: color }}, value: [var3, this.datetimeFormat(var1), this.datetimeFormat(var2)] })
        // consoleconsole.log(sm_point_xAxis_data)
      }

      // console.log(sm_point_xAxis_data.l)

      var option = {
        grid: [{
          x: 28,
          y: 50,
          x2: 28,
          y2: 50
        }],
        title: [{
          left: 'center',
          text: ''
        }],
        xAxis: [{
          gridIndex: 0,
          axisTick: {
            show: false,
            height: 0
          },
          type: 'time',
          rotate: 0,
          splitLine: { show: false },
          interval: 60 * 1000,
          min: sm_point_x_min_t,
          max: sm_point_x_max_t,
          axisLabel: {
            textStyle: {
              align: 'center'
              // verticalAlign: 'bottom'
            },
            interval: 0,
            padding: [-8, 0, 0, 0],
            formatter: function(value) {
              function datetimeFormat(longTypeDate) {
                const date = new Date(longTypeDate)
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
                return y + '/' + MM + '/' + d + ' ' + h + ':' + m
              }

              function getMin(longTypeDate) {
                const date = new Date(longTypeDate)
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
                return h + ':' + m
              }

              if (datetimeFormat(value) === sc_time) {
                // console.log('上床')
                return '|\n|\n|\n上床 ' + getMin(value)
              } else if (datetimeFormat(value) === rs_time) {
                // console.log('入睡')
                return '|\n入睡 ' + getMin(value)
              } else if (datetimeFormat(value) === qx_time) {
                // console.log('清醒')
                return '|\n清醒 ' + getMin(value)
              } else if (datetimeFormat(value) === lc_time) {
                // console.log('离床')
                return '|\n|\n|\n离床 ' + getMin(value)
              }
              return ''
            }
          }
        }],
        yAxis: [{
          splitLine: { show: false },
          gridIndex: 0,
          data: ['N3']
        }],
        series: [
          {
            type: 'custom',
            renderItem: function(params, api) {
              var categoryIndex = api.value(0)
              var start = api.coord([api.value(1), categoryIndex])
              var end = api.coord([api.value(2), categoryIndex])
              var height = 30
              return {
                type: 'rect',
                shape: echarts.graphic.clipRectByRect({
                  x: start[0],
                  y: start[1] - height / 2,
                  width: (end[0] - start[0]) * 4,
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
            data: sm_point_xAxis_data
          }]
      }

      chartByTd.setOption(option, null)
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
      return y + '/' + MM + '/' + d + ' ' + h + ':' + m
    },
    datetimeFormat2(longTypeDate, ts) {
      const date = new Date(longTypeDate * 1000 + ts)
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
      return y + '/' + MM + '/' + d + ' ' + h + ':' + m
    }
  }
}
</script>

<style lang="scss" scoped>

.line{
  background: #f3f3f3;
  height: 10px;
}

.chartByMv-Hxzt{
  margin:30px 0px;
  text-align: center;
  .item{
    background: #e3f6ff;
    color: #006beb;
    font-size: 16px;
    padding: 10px;
    padding-left: 45px;
    padding-right: 45px;
    border-radius: 5px;
    border: 1px solid #0e83ff;
}
}

.chartByMv-Legend{
 display: flex;
 margin-top: 12px;
.ld{
  flex: 1;
  display: flex;
  align-items: center;
  .n-c{
    width: 20px;
    height: 20px;
    display: block;
    border-radius: 50%;
    margin-right: 5px;
  }
}

}

.tag-point{
  padding-bottom: 10px;
}

</style>
