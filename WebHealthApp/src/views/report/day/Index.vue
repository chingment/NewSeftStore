<template>
  <div :class="'page-tabbar theme-' +theme">
    <div class="page-wrap">

      <mt-tab-container v-model="selected" class="page-tabbar-container">
        <mt-tab-container-item v-if="selected==='tab1'" id="tab1">
          <div class="a-part-1">

            <card-own-a
              v-if="theme==='pink'"
              :user-info="userInfo"
              :rd="rd"
            />

            <card-own-b
              v-if="theme==='green'"
              :user-info="userInfo"
              :rd="rd"
            />

          </div>
          <div class="a-part-2">
            <div style="padding:10px">
              <carousel-3d :space="carousel.space" :width="640" :height="640" :on-main-slide-click="onGzTag" :display="carousel.display" class="carousel-gz-tags">
                <slide v-for="(item, index) in rd.gzTags" :key="index" :index="index">

                  <div ref="gz_tag" class="gz-tag" :data-index="index">
                    <img class="image_item" :src="require('@/assets/report/day/'+theme+'/gz_tag_bg.png')">
                    <div class="t1">
                      <div class="name">{{ item.name }}</div>
                      <div class="icon">
                        <img class="image_item" :src="require('@/assets/report/day/'+theme+'/gz_tag_'+item.id+'.png')">
                      </div>
                    </div>
                    <div class="t2">
                      <div class="value">{{ item.value }}</div>
                      <div class="tips">{{ item.tips }}</div>
                    </div>

                  </div>

                </slide>
              </carousel-3d>

              <mt-popup
                v-model="popupVisibleGzTag"
                class="mint-popup-1"
                :style="'top:'+popupTopGzTag+'px'"
              >

                <score-level :tag-dv="activeGzTag" />

              </mt-popup>
            </div>
          </div>
          <div class="a-part-3">

            <div class="lm-tabs smTags">
              <div class="lm-tabs-title">
                <div v-for="(item, index) in rd.smTags" :key="index" :class="'lm-tabs-title-item '+ (activeTabBySmTag == item.id?'active':'') " @click="activeTabBySmTag = item.id"><span>{{ item.name }}</span></div>
              </div>
              <div class="lm-tabs-content">
                <div v-for="(item, index) in rd.smTags" v-show="activeTabBySmTag == item.id" :key="index" :class="'lm-tabs-content-item '+ (activeTabBySmTag == item.id?'active':'') ">
                  <div class="tag-name">{{ item.name }}</div>
                  <div class="tag-proexplain">
                    <div class="t">解释：</div>
                    <div class="c"> {{ item.proExplain }}</div>
                  </div>
                  <div class="tag-suggest">
                    <div class="t">建议：</div>
                    <div class="c"><pre style="white-space: pre-line;">{{ item.suggest }}</pre></div>
                  </div>
                </div>
              </div>
            </div>
          </div>

        </mt-tab-container-item>
        <mt-tab-container-item v-if="selected==='tab2'" id="tab2">
          <div class="b-part-1">
            <div class="sm-score-box">
              <div class="process-bg" />
              <div class="process-ct">
                <div class="process-score">

                  <vue-circle
                    ref="sm_score"
                    :progress="rd.smScore.value"
                    :size="100"
                    :reverse="false"
                    line-cap="round"
                    :fill="smScoreCircleFill"
                    empty-fill="#fff"
                    :animation-start-value="0.0"
                    :start-angle="30"
                    insert-mode="append"
                    :animation="{ duration: 1200, easing: 'easeOutBounce' }"
                    :thickness="8"
                    :show-percent="false"
                  >
                    <div class="c-health-score">
                      <div class="t1">{{ parseInt(rd.smScore.value) }}</div>
                      <div class="t2">睡眠值</div>
                    </div>
                  </vue-circle>

                </div>
                <div class="process-text">
                  <div class="t1">{{ userInfo.signName }}</div>
                  <div class="t2">{{ rd.smScoreTip }}</div>
                </div>
              </div>
            </div>

            <div class="sm-score-chart" style="width:100%">

              <score-level :tag-dv="rd.smScore" />

              <!-- <div class="i-score">
                <div class="t1">睡眠值</div>
                <div class="t2" :style="{'color': rd.smScore.color}">{{ rd.smScore.value }}</div>
                <div class="t3" :style="{'color': rd.smScore.color}">{{ rd.smScore.tips }}</div>
              </div>

              <div class="i-sign">
                <div class="col">
                  <div class="topnum" style="text-align: left;">0</div>
                  <div :class="'mt-range jd1 '+((rd.smScore.value>=0&&rd.smScore.value<30)?'':'no-ative') ">
                    <div class="mt-range-content">
                      <div class="mt-range-runway" style="border-top-width: 8px;border-top-color: #e68a8b;" />
                      <div class="mt-range-progress" style="width: 0%; height: 8px;" />
                      <div class="mt-range-thumb" :style="'left: '+rd.smScore.value+'%;background-color:#e68a8b;'" />
                    </div>
                  </div>
                  <div class="bottomtips">差</div>
                </div>
                <div class="col-split">
                  <div class="topnum">30</div>
                  <div class="border-split" />
                </div>
                <div class="col">
                  <div class="topnum" style="visibility: hidden;">0</div>
                  <div :class="'mt-range jd2 '+((rd.smScore.value>=30&&rd.smScore.value<50)?'':'no-ative') ">
                    <div class="mt-range-content">
                      <div class="mt-range-runway" style="border-top-width: 8px;border-top-color: #f1b46d;" />
                      <div class="mt-range-progress" style="width: 0%; height: 8px;" />
                      <div class="mt-range-thumb" :style="'left: '+rd.smScore.value+'%;background-color:#f1b46d;'" />
                    </div>
                  </div>
                  <div class="bottomtips">较差</div>
                </div>
                <div class="col-split">
                  <div class="topnum">50</div>
                  <div class="border-split" />
                </div>
                <div class="col">
                  <div class="topnum" style="visibility: hidden;">0</div>
                  <div :class="'mt-range jd3 '+((rd.smScore.value>=50&&rd.smScore.value<70)?'':'no-ative') ">
                    <div class="mt-range-content">
                      <div class="mt-range-runway" style="border-top-width: 8px;border-top-color: #e16d6d;" />
                      <div class="mt-range-progress" style="width: 0%; height: 8px;" />
                      <div class="mt-range-thumb" :style="'left: '+rd.smScore.value+'%;background-color:#e16d6d;'" />
                    </div>
                  </div>
                  <div class="bottomtips">中等</div>
                </div>
                <div class="col-split">
                  <div class="topnum">70</div>
                  <div class="border-split" />
                </div>
                <div class="col">
                  <div class="topnum" style="visibility: hidden;">0</div>
                  <div :class="'mt-range jd4 '+((rd.smScore.value>=70&&rd.smScore.value<90)?'':'no-ative') ">
                    <div class="mt-range-content">
                      <div class="mt-range-runway" style="border-top-width: 8px;border-top-color: #96a2dc;" />
                      <div class="mt-range-progress" style="width: 0%; height: 8px;" />
                      <div class="mt-range-thumb" :style="'left: '+rd.smScore.value+'%;background-color:#96a2dc;'" />
                    </div>
                  </div>
                  <div class="bottomtips">较好</div>
                </div>
                <div class="col-split">
                  <div class="topnum">90</div>
                  <div class="border-split" />
                </div>
                <div class="col">
                  <div class="topnum" style="text-align: right;">100</div>
                  <div :class="'mt-range jd5 '+((rd.smScore.value>=90&&rd.smScore.value<=100)?'':'no-ative') ">
                    <div class="mt-range-content">
                      <div class="mt-range-runway" style="border-top-width: 8px;border-top-color: #628DF2;" />
                      <div class="mt-range-progress" style="width: 0%; height: 8px;" />
                      <div class="mt-range-thumb" :style="'left: '+rd.smScore.value+'%;background-color:#628DF2;'" />
                    </div>
                  </div>
                  <div class="bottomtips">好</div>
                </div>
              </div>

              <div id="chart_BySmScore" ref="chart_BySmScore" :style="'width:'+(screenWidth-50)+'px;height:300px;margin:auto;'" />
               -->
            </div>
          </div>

          <div class="b-part-2">
            <div class="mi-title">睡眠评价</div>
            <div class="mi-content">
              <div style="text-indent: 20px;"> 您本次睡眠的在床时间为 {{ rd.smZcsjfw.value }}，共{{ rd.smZcsc.value }}，睡眠总时长为{{ rd.smSmsc.value }}。</div>
              <div style="text-indent: 20px;">本次睡眠效率 {{ rd.smSmxl.value }}%（<span :style="'color:'+rd.smSmxl.color+';'"> {{ rd.smSmxl.tips }}</span>）,睡眠连续性 {{ rd.smSmlxx.value }}%（<span :style="'color:'+rd.smSmlxx.color+';'"> {{ rd.smSmlxx.tips }}</span>），深睡眠比例 {{ rd.smSdsmbl.value }}%（<span :style="'color:'+rd.smSdsmbl.color+';'"> {{ rd.smSdsmbl.tips }}</span>）。</div>
              <div style="text-indent: 20px;">呼吸紊乱指数 {{ rd.hxZtahizs.value }}（<span :style="'color:'+rd.hxZtahizs.color+';'"> {{ rd.hxZtahizs.tips }}</span>）；基准心率 {{ rd.xlDcjzxl.value }}次/分钟（<span :style="'color:'+rd.xlDcjzxl.color+';'"> {{ rd.xlDcjzxl.tips }}</span>）；基准呼吸 {{ rd.hxDcjzhx.value }} 次/分钟（<span :style="'color:'+rd.hxDcjzhx.color+';'"> {{ rd.hxDcjzhx.tips }}</span>） 。</div>
            </div>
          </div>

          <div class="b-part-3">
            <div class="mi-title">监测结果</div>
            <div class="mi-content">
              <div v-for="(item, index) in rd.smDvs" :key="index" :class="'mi-item mi-item_'+(index%2==0?'0':'1')">
                <div class="wrap">
                  <div class="item-n">{{ item.name }}</div>
                  <div class="item-v"><dv-item :value="item" sign /></div>
                </div>
              </div>
            </div>
          </div>

          <div v-if="rd.rptSuggest!=null" class="b-part-4">
            <div class="mi-title">今日建议</div>
            <div class="mi-content">
              <div class="mi-icon" />
              <div class="mi-suggest">
                <pre style="white-space: pre-line;">{{ rd.rptSuggest }}</pre>
              </div>
            </div>
          </div>

        </mt-tab-container-item>
        <mt-tab-container-item v-if="selected==='tab3'" id="tab3">
          <div class="c-part-1">
            <div v-if="!consult.isOpen" class="open-off">
              <div class="t1"> 敬请期待</div>
              <div class="t2"> 谢谢关注，即将开启</div>
            </div>
            <div v-else class="open-on">
              <img :src="consult.tmpImg" alt="" style="width:100%">
            </div>
          </div>
        </mt-tab-container-item>
      </mt-tab-container>

      <mt-tabbar v-model="selected" fixed>
        <mt-tab-item v-for="(item, index) in tabs" :id="item.id" :key="index">
          <img slot="icon" :src="selected==item.id?item.iconOn:item.icon "> {{ item.text }}
        </mt-tab-item>
      </mt-tabbar>
    </div>
  </div>
</template>

<script>
import { Carousel3d, Slide } from 'vue-carousel-3d'
import { getDetails, updateVisitCount } from '@/api/dayreport'
import DvItem from '@/components/DvItem.vue'
import CardOwnA from './components/CardOwnA.vue'
import CardOwnB from './components/CardOwnB.vue'
import VueCircle from 'vue2-circle-progress'
import echarts from 'echarts'
import ScoreLevel from '@/components/ScoreLevel.vue'
var chartBySmScore
export default {
  name: 'ReportDayIndex',
  components: {
    Carousel3d,
    Slide,
    DvItem,
    CardOwnA,
    CardOwnB,
    VueCircle,
    ScoreLevel
  },
  data() {
    return {
      loading: false,
      rptId: '',
      screenWidth: document.body.clientWidth,
      selected: 'tab1',
      tabs: [
        {
          id: 'tab1',
          text: '首页',
          icon: require('@/assets/report/day/pink/tabbar_home.png'),
          iconOn: require('@/assets/report/day/pink/tabbar_home_on.png')
        },
        {
          id: 'tab2',
          text: '健康监测',
          icon: require('@/assets/report/day/pink/tabbar_monitor.png'),
          iconOn: require('@/assets/report/day/pink/tabbar_monitor_on.png')
        },
        {
          id: 'tab3',
          text: '健康管家',
          icon: require('@/assets/report/day/pink/tabbar_health.png'),
          iconOn: require('@/assets/report/day/pink/tabbar_health_on.png')
        }
      ],
      userInfo: {
        avatar: '',
        signName: '',
        careMode: 0,
        pregnancy: {
          birthLastDays: 0,
          gesWeek: 0,
          gesDay: 0
        }
      },
      rd: {
        healthScore: { value: 0 },
        healthDate: '',
        smScore: 40,
        smScoreTip: '您的睡眠值已经打败77%的人',
        smTags: [],
        gzTags: [
          {
            id: '1',
            name: ''
          },
          {
            id: '2',
            name: ''
          },
          {
            id: '3',
            name: ''
          },
          {
            id: '4',
            name: ''
          },
          {
            id: '5',
            name: ''
          },
          {
            id: '6',
            name: ''
          },
          {
            id: '7',
            name: ''
          }
        ],
        smDvs: [],
        hrvXzznl: {
          value: '',
          refRange: ''
        },
        xlDcjzxl: {
          value: '',
          refRange: ''
        },
        xlCqjzxl: {
          value: '',
          refRange: ''
        },
        rptSuggest: ''
      },
      carousel: {
        space: 80,
        display: 7
      },
      smScoreCircleFill: { gradient: ['#8316bd', '#fff', '#ad1da3'] },
      activeTabBySmTag: 0,
      activeGzTag: {},
      popupVisibleGzTag: false,
      popupTopGzTag: 160,
      consult: {
        isOpen: false,
        tmpImg: ''
      },
      theme: 'green'
    }
  },
  watch: {
    selected: function(val) {
      if (val === 'tab2') {
        this.$nextTick(function() {
          // this.getChartBySmScore()
        }, 2000)
      }
    }
  },
  mounted() {
    if (window.performance.navigation.type === 1) {
      console.log('页面被刷新！')
    } else {
      updateVisitCount({ rptId: this.$route.query.rptId }).then(res => {})
    }
  },
  created() {
    this.rptId = this.$route.query.rptId
    this.theme = typeof this.$route.query.theme === 'undefined' ? 'green' : this.$route.query.theme
    this.onGetDetails()
  },
  methods: {
    onGetDetails() {
      this.loading = true
      if (this.screenWidth > 300 && this.screenWidth < 400) {
        this.carousel.space = 60
      } else {
        this.carousel.space = 80
      }
      getDetails({ rptId: this.rptId }).then(res => {
        if (res.result === 1) {
          var d = res.data

          if (this.theme === 'pink') {
            this.smScoreCircleFill.gradient = ['#8316bd', '#fff', '#ad1da3']
          } else if (this.theme === 'green') {
            this.smScoreCircleFill.gradient = ['#0ca7d4', '#fff', '#076e8b']
          }

          this.rd = d.rd
          this.userInfo = d.userInfo
          this.consult = d.consult
          this.activeTabBySmTag = d.rd.smTags[0].id
        }
        this.loading = false
      })
    },
    getChartBySmScore() {
      chartBySmScore = echarts.init(this.$refs.chart_BySmScore, null, { renderer: 'svg' })

      var datePt = [] // = ['12-12', '12-13', '12-14', '12-15', '12-16', '12-17']
      var valuePt = []// = [68, 32, 65, 71, 82, 93]

      this.rd.smScoreByLast.forEach(item => {
        datePt.push(item.date)
        valuePt.push(item.score)
      })

      console.log(datePt)

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
          data: datePt,
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
              var texts = []
              if (value === 0) {
                texts.push('0')
              } else if (value === 10) {
                texts.push('')
              } else if (value === 20) {
                texts.push('')
              } else if (value === 30) {
                texts.push('30')
              } else if (value === 40) {
                texts.push('')
              } else if (value === 50) {
                texts.push('50')
              } else if (value === 60) {
                texts.push('')
              } else if (value === 70) {
                texts.push('70')
              } else if (value === 80) {
                texts.push('')
              } else if (value === 90) {
                texts.push('90')
              } else if (value === 100) {
                texts.push('100')
              }
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
              color: '#989ef6',
              lineStyle: {
                color: '#bbbdb7'
              }
            }
          },
          data: valuePt,
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

      chartBySmScore.setOption(option, null)
    },
    clearChart() {
      this.$destroy()
    },
    onGzTag(item) {
      console.log(this.$refs.gz_tag[item.index].offsetHeight)
      this.activeGzTag = this.rd.gzTags[item.index]
      this.popupTopGzTag = this.$refs.gz_tag[item.index].offsetHeight + 320
      this.popupVisibleGzTag = true
    }
  }
}
</script>

<style lang="scss" scoped>

.page-tabbar {
  overflow: hidden;

  height: 100vh;

  background-color: #f5f5f5;
}

.page-wrap {
  overflow: auto;

  height: 100%;
  padding-bottom: 100px;
}

.mint-tabbar {
  background-color: #fff;
  background-size: 100% 0;
}

.mint-tabbar > .mint-tab-item.is-selected {
  color: #5478ff;
  background-color: #fff;
}

.lm-tabs {
  padding: 0 20px 20px 20px;
}

.lm-tabs-title {
  display: flex;
}

.lm-tabs-title-item {
  font-size: 12px;
  font-weight: bold;

  display: flex;
  align-items: center;
  flex: 1;
  justify-content: center;

  height: 42px;
  padding: 0 5px;
}

.lm-tabs-title .active {
  border-top-left-radius: 10px;
  border-top-right-radius: 10px;
  background-size: 100% 100%;
}

.lm-tabs-content {
  margin-top: -1px;
}

.lm-tabs-content-item {
  min-height: 300px;
  padding: 18px;

  border-radius: 10px;
  background-size: cover;
  box-shadow: 0 2px 12px 0 rgba(0,0,0,.1);
}

.lm-tabs-content-item:first-child {
  border-top-left-radius: 0;
}

.lm-tabs-content-item:last-child {
  border-top-right-radius: 0;
}

.smTags {
  .tag-name {
    font-weight: bold;
  }

  .tag-proexplain {
    color: #53535b;

    .t {
      padding: 8px 0;
    }

    .c {
      font-size: 14px;
      line-height: 21px;
    }
  }

  .tag-suggest {
    color: #53535b;

    .t {
      padding: 8px 0;
    }

    .c {
      font-size: 14px;
      line-height: 21px;
    }
  }
}

.a-part-1 {
  padding: 20px;
}

.b-part-1 {
  padding: 20px;

  background-repeat: no-repeat;
  background-size: cover;

  .sm-score-box {
    position: relative;

    .process-bg {
      display: flex;

      height: 120px;

      opacity: .2;
      border-radius: 10px;
      background: #fff;
    }

    .process-ct {
      position: absolute;
      top: 0;
      left: 0;

      display: flex;

      width: 100%;
      height: 100%;
    }

    .process-score {
      display: flex;
      align-content: center;
      align-items: center;
      justify-content: center;

      width: 120px;

      color: #fff;

      .t1 {
        font-size: 32px;
        font-weight: bold;
      }

      .t2 {
        font-size: 12px;

        padding: 8px 0;
      }
    }

    .process-text {
      display: flex;
      flex-direction: column;
      justify-content: center;

      color: #fff;

      .t1 {
        font-size: 24px;
      }

      .t2 {
        padding: 5px 0;
      }
    }
  }

  .sm-score-chart {
    margin-top: 10px;
    padding: 10px;

    border-radius: 5px;
    background: #fff;

  }
}

.b-part-2 {
  display: flex;
  flex-direction: column;

  padding: 0 20px;

  .mi-title {
    font-size: 16px;
    font-weight: bold;
    line-height: 38px;

    height: 38px;
  }

  .mi-content {
    font-size: 12px;
    line-height: 21px;

    padding: 10px;

    color: #4c4b5e;
    border-radius: 12px;
    background-color: #fff;
  }
}

.b-part-3 {
  display: flex;
  flex-direction: column;

  padding: 0 20px;

  .mi-title {
    font-size: 16px;
    font-weight: bold;
    line-height: 38px;

    height: 38px;
  }

  .mi-item {
    float: left;

    width: 50%;
    margin-bottom: 8px;

    .wrap {
      padding: 10px;

      border-radius: 12px;
      background-color: #fff;

      .item-n {
        font-size: 16px;
        font-weight: 600;

        padding: 6px 0;

        color: #2d3142;
      }

      .item-v {
        font-size: 14px;
        font-weight: 600;

        padding: 4px 0;
      }
    }
  }

  .mi-item_0 {
    padding-right: 4px;
  }

  .mi-item_1 {
    padding-left: 4px;
  }
}

.b-part-4 {
  display: flex;
  flex-direction: column;

  padding: 0 20px 20px 20px;

  .mi-title {
    font-size: 24px;
    font-weight: bold;

    padding: 12px 0;
  }

  .mi-content {
    display: flex;

    min-height: 100px;
    padding: 10px 6px;

    border-radius: 5px;
    background-color: #fff;

    .mi-icon {
      width: 50px;

      background-size: 100%;
    }

    .mi-suggest {
      font-size: 12px;
      line-height: 21px;

      flex: 1;

      padding: 0 6px;

      letter-spacing: 3px;

      color: #53535b;
    }
  }
}

.c-part-1 {
  display: flex;
  align-items: center;
  flex-direction: column;
  justify-content: center;
  justify-content: center;
  background: #fff;
  height: 100vh;
  padding: 10px;
.open-off{
  text-align: center;
  .t1 {
    font-size: 24px;
    padding: 10px;
  }

  .t2 {
    font-size: 32px;
    padding: 20px;
  }
}
}

.theme-pink {
  .a-part-1 {
    background: url('~@/assets/report/day/pink/a_part1_bg.png');
    background-repeat: no-repeat;
    background-size: 100% 100%;
    background-size: cover;
  }

  .a-part-2 {
    .carousel-gz-tags {
      height: 168px !important;

      /deep/ .carousel-3d-slider {
        width: 168px !important;
        height: 168px !important;

        background: unset;
      }

      /deep/ .carousel-3d-slide {
        width: 168px !important;
        height: 168px !important;

        border: none;
        background: unset;
      }

      .gz-tag {
        position: absolute;

        width: 168px;
        height: 168px;

        cursor: default;
        cursor: default;
        cursor: default;
        cursor: default;
        -webkit-transition: all .5s;
           -moz-transition: all .5s;
             -o-transition: all .5s;
                transition: all .5s;

        color: #fff;
        border-radius: 10px;

        .t1 {
          position: absolute;
          top: 10px;
          left: 0;

          display: flex;

          width: 100%;
          padding: 14px 30px 0 30px;
        }

        .t2 {
          position: absolute;
          bottom: 34px;
          left: 0;

          display: flex;

          width: 100%;
          padding: 0 30px 0 30px;
        }

        .name {
          font-size: 12px;
          font-size: 22px;

          flex: 1;
        }

        .icon {
          flex: 1;
        }

        .value {
          font-size: 28px;
          font-weight: bold;
          font-style: italic;
        }

        .tips {
          display: flex;
          align-items: center;
          flex: 1;
          justify-content: center;
          ;
        }
      }
    }
  }

  .a-part-3 {
    .lm-tabs-content-item {
      background: url('~@/assets/report/day/pink/sm_tag_content.png') no-repeat top center;
      background-color: #fff;
      background-size: contain;
    }

    .lm-tabs-title .active {
      background: #c3c1fb;
    }
  }

  .b-part-1 {
    background: url('~@/assets/report/day/pink/b_part1_bg.png') no-repeat top center;
    background-size: cover;
  }

  .b-part-4 {
    .mi-icon {
      background: url('~@/assets/report/day/pink/b_part4_icon.jpg') no-repeat;
      background-size: 100%;
    }
  }
}

.theme-green {
  .a-part-1 {
    background: url('~@/assets/report/day/green/a_part1_bg.png');
    background-repeat: no-repeat;
    background-size: cover;
      padding: 20px 10px;
  }

  .a-part-2 {
    .carousel-gz-tags {
      height: 180px !important;

      /deep/ .carousel-3d-slider {
        width: 135px !important;
        height: 180px !important;

        background: unset;
      }

      /deep/ .carousel-3d-slide {
        width: 135px !important;
        height: 180px !important;

        border: none;
        background: unset;
      }

      .gz-tag {
        position: absolute;

        width: 135px;
        height: 180px;

        cursor: default;
        cursor: default;
        cursor: default;
        cursor: default;
        -webkit-transition: all .5s;
           -moz-transition: all .5s;
             -o-transition: all .5s;
                transition: all .5s;

        color: #fff;
        border-radius: 10px;

        .t1 {
          position: absolute;
          top: 10px;
          left: 0;

          display: flex;

          width: 100%;
          padding: 10px 20px;
        }

        .t2 {
          position: absolute;
          bottom: 10px;
          left: 0;

          display: flex;

          width: 100%;
          padding: 10px 14px;
        }

        .name {
          font-size: 12px;
          font-size: 22px;

          flex: 1;
        }

        .icon {
          flex: 1;
        }

        .value {
          font-size: 28px;
          font-weight: bold;
          font-style: italic;
        }

        .tips {
          display: flex;
          align-items: center;
          flex: 1;
          justify-content: center;
          ;
        }
      }
    }
  }

  .a-part-3 {
    .lm-tabs-content-item {
      color: #fff;
      background: url('~@/assets/report/day/green/sm_tag_content.png') no-repeat top center;
      background-color: #06bdc1;
      background-size: contain;
    }

    .lm-tabs-title .active {
      color: #fff;
      background: #09747d;
    }

    .tag-name,
    .tag-proexplain,
    .tag-suggest {
      color: #fff;
    }
  }

  .b-part-1 {
    background: url('~@/assets/report/day/green/b_part1_bg.png') no-repeat top center;
  }

  .b-part-4 {
    .mi-icon {
      background: url('~@/assets/report/day/green/b_part4_icon.jpg') no-repeat;
      background-size: 100%;
    }
  }
}

</style>
