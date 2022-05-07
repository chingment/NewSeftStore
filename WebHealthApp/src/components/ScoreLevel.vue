<template>
  <div class="score-level">
    <div class="collapse-item i-score" @click="onCollapse">
      <div class="t-icon" v-if="elEnableIcon"> 
         <img class="image" :src="require('@/assets/report/day/'+elTheme+'/gz_tag2_'+tagDv.id+'.png')"> 
      </div>
      <div class="t1">{{ tagDv.name }}</div>
      <div class="t2" :style="{'color': tagDv.color}"> <span v-show="!tagDv.isHidValue">{{ tagDv.valueText }}</span></div>
      <div class="t3" :style="{'color': tagDv.color}">{{ tagDv.tips }}</div>
    </div>
    <div v-show="elIsCollapse" class="collapse-item-more" style="width:100%">
      <div class="i-sign">
        <template v-for="(item, index) in tagDv.refRanges">
          <div :key="'a'+index" class="col">
            <div class="topnum" :style="(index>=0&&index<tagDv.refRanges.length-1?'text-align:left;':'text-align:right')+(index>=1&&index<tagDv.refRanges.length-1?'visibility: hidden;':'')">
              <span v-show="!tagDv.isHidValue">  {{ (index>=0&&index<(tagDv.refRanges.length-1))?item.min:item.max }}</span>
            </div>
            <div :class="'mt-range jd1 '+((tagDv.value>=item.min&&tagDv.value<item.max)?'':'no-ative') " style="padding:0px 2px">
              <div class="mt-range-content">
                <div class="mt-range-runway" :style="'border-top-width: 8px;border-top-color:'+item.color+';'" />
                <div class="mt-range-progress" style="width: 0%; height: 8px;" />
                <div class="mt-range-thumb" :style="'left: '+tagDv.value+'%;background-color:'+item.color+';'" />
              </div>
            </div>
            <div class="bottomtips">{{ item.tips }}</div>
          </div>
          <div v-if="index<tagDv.refRanges.length-1" :key="'b'+index" class="col-split">
            <div class="topnum"><span v-show="!tagDv.isHidValue"> {{ item.max }}</span></div>
            <div class="border-split" />
          </div>
        </template>
      </div>
      <div v-if="tagDv.pph!=null" class="i-pph">
        {{ tagDv.pph }}
      </div>
      <div ref="i_chart" :style="'width:100%;height:'+chatHeight+';margin:auto;'" />
    </div>
  </div>
</template>

<script>
import echarts from 'echarts'
var i_chart
export default {
  name: 'ScoreLevel',
  props: {
    tagDv: {
      type: Object,
      default: null
    },
    chatHeight: {
      type: String,
      default: '200px'
    },
    theme: {
      type: String,
      default: 'green'
    },
    isCollapse: {
      type: Boolean,
      default: false
    },
    enableIcon:{
      type: Boolean,
      default: false
    },
    enableCollapse: {
      type: Boolean,
      default: true
    }
  },
  data() {
    return {
      innerWidth: 0,
      elIsCollapse: this.isCollapse,
      elEnableIcon:this.enableIcon,
      elTheme:this.theme,
      elEnableCollapse:this.enableCollapse
    }
  },
  watch: {
    tagDv(newV, oldV) {
      var _this = this
      this.$nextTick(function() {
        _this.getChart(newV.chat)
      }, 300)
    }
  },
  beforeDestroy() {
    if (i_chart) {
      i_chart.clear()
      i_chart.dispose()
      i_chart = null
    }
  },
  created() {
    var _this = this
    this.innerWidth = window.innerWidth

    this.$nextTick(function() {
      _this.getChart(this.tagDv.chat)
    }, 300)

    window.addEventListener('resize', function() {
      var _width = document.getElementById('i_chart').offsetWidth
      console.log(_width)
      var obj = {
        width: _width
      }
      i_chart.resize(obj)
    })
  },
  methods: {
    onCollapse(){
      if(this.elEnableCollapse){
      this.elCollapse = !this.elCollapse
      }
    },
    getChart(chat) {
      var _this = this
      if (!i_chart) {
        i_chart = echarts.init(this.$refs.i_chart, null, { renderer: 'svg' })
      }
      var data = chat.data
      if (data === null || data.length === 0) { return }

      var xData = []
      var yData = []

      data.forEach(item => {
        xData.push(item.xData)
        yData.push(item.yData)
      })
      var yAxisLabel = chat.yAxisLabel
      var yAxisMin = chat.yAxisMin
      var yAxisMax = chat.yAxisMax
      var yAxisSplitNumber = chat.yAxisSplitNumber
      var yAxisLabelExt = typeof chat.yAxisLabelExt === 'undefined' ? null : chat.yAxisLabelExt
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
          min: yAxisMin,
          max: yAxisMax,
          type: 'value',
          splitNumber: yAxisSplitNumber,
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
            formatter: function(value, index) {
              console.log(value + ',' + index)

              var _val = yAxisLabel.filter(function(item) {
                return item === value
              })
              var texts = []
              if (_val == null) {
                texts.push('')
              } else {
                if (yAxisLabelExt == null) {
                  texts.push(_val)
                } else {
                  texts.push('')
                }
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
          name: '',
          symbol: 'circle',
          symbolSize: 8,
          showSymbol: true,
          itemStyle: {
            normal: {
              color: function(params) {
                var color = '#989ef6'
                var refRanges = _this.tagDv.refRanges
                for (let index = 0; index < refRanges.length; index++) {
                  var refRange = refRanges[index]
                  if (params.data >= refRange.min && params.data <= refRange.max) {
                    color = refRange.color
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

      console.log(i_chart)
      i_chart.setOption(option, null)
    }
  }
}
</script>

<style lang="scss" scoped>

.score-level{
  padding: 5px 0px;
}

.i-score {
  font-size: 18px;
  font-weight: bold;

  display: flex;

  .t-icon{
 display: flex;
    align-items: center;
    margin-right: 5px;
.image{
  widows: 30px;
  height: 30px;
}

  }
  .t1 {
    display: flex;
    flex: 1;
    justify-content: flex-start;
        align-items: center
  }

  .t2 {
    display: flex;
    flex: 1;
    justify-content: center;
        align-items: center
  }

  .t3 {
    display: flex;
    flex: 1;
    justify-content: flex-end;
        align-items: center
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

.i-pph{
    padding: 15px 0px;
    color: gray;
    text-indent: 28px;
        font-size: 12px;
        line-height: 16px;
}

</style>
