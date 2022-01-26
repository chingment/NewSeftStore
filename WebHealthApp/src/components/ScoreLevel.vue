<template>

  <div>
    <div class="i-score">
      <div class="t1">{{ tagDv.name }}</div>
      <div class="t2" :style="{'color': tagDv.color}">{{ tagDv.value }}</div>
      <div class="t3" :style="{'color': tagDv.color}">{{ tagDv.tips }}</div>
    </div>
    <div class="i-sign">
      <template v-for="(item, index) in refRangeDv">
        <div :key="'a'+index" class="col">
          <div class="topnum" :style="(index>=0&&index<refRangeDv.length-1?'text-align:left;':'text-align:right')+(index>=1&&index<refRangeDv.length-1?'visibility: hidden;':'')">
            {{ (index>=0&&index<(refRangeDv.length-1))?item.min:item.max }}
          </div>
          <div :class="'mt-range jd1 '+((tagDv.value>=item.min&&tagDv.value<item.max)?'':'no-ative') ">
            <div class="mt-range-content">
              <div class="mt-range-runway" :style="'border-top-width: 8px;border-top-color:'+item.color+';'" />
              <div class="mt-range-progress" style="width: 0%; height: 8px;" />
              <div class="mt-range-thumb" :style="'left: '+tagDv.value+'%;background-color:'+item.color+';'" />
            </div>
          </div>
          <div class="bottomtips">{{ item.tips }}</div>
        </div>
        <div v-if="index<refRangeDv.length-1" :key="'b'+index" class="col-split">
          <div class="topnum">{{ item.max }}</div>
          <div class="border-split" />
        </div>
      </template>
    </div>
    <div ref="i_chart" :style="'width:100%;height:300px;margin:auto;'" />
  </div>
</template>

<script>
import echarts from 'echarts'

export default {
  name: 'ScoreLevel',
  props: {
    tagDv: {
      type: Object,
      default: null
    },
    chartDv: {
      type: Array,
      default() {
        return []
      }
    },
    refRangeDv: {
      type: Array,
      default() {
        return []
      }
    }
  },
  data() {
    return {
      i_chart: null,
      isDesktop: this.$store.getters.isDesktop
    }
  },
  mounted() {
    this.i_chart = echarts.init(this.$refs.i_chart, null, { renderer: 'svg' })
    this.getChart(this.chartDv)
  },
  created() {
    // this.getChart(this.chartData)
  },
  methods: {
    getChart(data) {
      var _this = this
      if (data === null || data.length === 0) { return }

      var xData = [] // = ['12-12', '12-13', '12-14', '12-15', '12-16', '12-17']
      var yData = []// = [68, 32, 65, 71, 82, 93]

      data.forEach(item => {
        xData.push(item.xData)
        yData.push(item.yData)
      })
      var yAxisLabel = [0, 30, 50, 70, 90, 100]
      var option = {
        grid: [{
          x: 30,
          y: 30,
          x2: 30,
          y2: 30
        }],
        legend: {
          padding: 10
        },
        title: {
          text: '', // 主标题
          x: 'center',
          y: 'top'
        },
        tooltip: {
          trigger: 'axis'
        },
        xAxis: {
          data: xData,
          show: true,
          axisLabel:
          {
            interval: 0,
            show: true,
            textStyle: {
              color: '#000'
            }
          },
          axisTick: {
            show: true,
            alignWithLabel: true
          },
          boundaryGap: false,
          axisLine: {
            lineStyle: {
              color: '#c7c7c7',
              width: 1
            }
          }
        },
        yAxis: {
          min: 0,
          max: 100,
          type: 'value',
          splitNumber: 10,
          splitLine: {
            show: false
          },
          axisTick: {
            show: false
          },

          axisLabel: {
            interval: 0,
            show: true,
            textStyle: {
              color: '#000'
            },
            formatter: function(value) {
              var _val = yAxisLabel.filter(function(item) {
                return item === value
              })
              var texts = []
              if (_val == null) {
                texts.push('')
              } else {
                texts.push(_val)
              }

              // console.log(value)
              // var texts = []
              // if (value === 0) {
              //   texts.push('0')
              // } else if (value === 10) {
              //   texts.push('')
              // } else if (value === 20) {
              //   texts.push('')
              // } else if (value === 30) {
              //   texts.push('30')
              // } else if (value === 40) {
              //   texts.push('')
              // } else if (value === 50) {
              //   texts.push('50')
              // } else if (value === 60) {
              //   texts.push('')
              // } else if (value === 70) {
              //   texts.push('70')
              // } else if (value === 80) {
              //   texts.push('')
              // } else if (value === 90) {
              //   texts.push('90')
              // } else if (value === 100) {
              //   texts.push('100')
              // }
              return texts
            }
          },
          axisLine: {
            lineStyle: {
              color: '#c7c7c7',
              width: 1
            }
          }
        },
        series: {
          type: 'line',
          name: '分数',
          symbol: 'circle',
          symbolSize: 8,
          showSymbol: true,
          itemStyle: {
            normal: {
              color: function(params) {
                var color = '#989ef6'
                for (let index = 0; index < _this.refRangeDv.length; index++) {
                  var refRangeDv = _this.refRangeDv[index]
                  if (params.data >= refRangeDv.min && params.data <= refRangeDv.max) {
                    color = refRangeDv.color
                    break
                  }
                }

                return color
              },
              lineStyle: {
                color: '#bbbdb7'
              }
            }
          },
          data: yData,
          markLine: {
            symbol: 'none',
            silent: false,
            lineStyle: {
              normal: {
                color: '#bac1c6' // 这儿设置安全基线颜色
              }
            },
            data: [{
              yAxis: [70]
            }],
            label: {
              normal: {
                color: '#000',
                formatter: '70' // 这儿设置安全基线
              }
            }
          }

        }
      }

      console.log(this.i_chart)
      this.i_chart.setOption(option, null)
    }
  }
}
</script>

<style lang="scss" scoped>

.i-score {
  font-size: 18px;
  font-weight: bold;

  display: flex;

  .t1 {
    display: flex;
    flex: 1;
    justify-content: flex-start;
  }

  .t2 {
    display: flex;
    flex: 1;
    justify-content: center;
  }

  .t3 {
    display: flex;
    flex: 1;
    justify-content: flex-end;
  }
}

.i-sign {
  display: flex;

  padding: 10px 0;

  .col {
    flex: 1;

    .topnum {
      font-size: 12px;

      padding: 5px 0;

      text-align: left;

      color: gray;
    }

    .bottomtips {
      font-size: 12px;

      padding: 5px 0;

      text-align: center;

      color: gray;
    }

    /deep/ .mt-range-runway {
      border-radius: 20px;
    }

    /deep/ .mt-range-thumb {
      top: 4px;

      width: 20px;
      height: 20px;
    }

    /deep/ .no-ative {
      .mt-range-thumb {
        display: none  !important;
      }
    }
  }

  .col-split {
    display: flex;
    align-items: center;
    flex-direction: column;
    justify-content: flex-start;

    .topnum {
      font-size: 12px;

      padding: 5px 0;

      color: gray;
    }

    .border-split {
      display: inline-block;

      width: 1px;
      height: 44px;

      border-left: 1px dashed #c5c5c5;
    }
  }
}

</style>
