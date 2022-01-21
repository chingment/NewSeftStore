<template>
  <div :class="'page-tabbar theme-' +theme">
    <div class="page-wrap">

      <mt-tab-container v-model="selected" class="page-tabbar-container">
        <mt-tab-container-item id="tab1">
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
              <carousel-3d :space="carousel.space" :display="carousel.display" class="carousel-gz-tags">
                <slide v-for="(item, index) in rd.gzTags" :key="index" :index="index">

                  <div class="gz-tag" :data-index="index">
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
        <mt-tab-container-item id="tab2">
          <div class="b-part-1">
            <div class="sm-score-box">
              <div class="process-bg" />
              <div class="process-ct">
                <div class="process-score">

                  <vue-circle
                    ref="myprogress"
                    :progress="rd.smScore"
                    :size="100"
                    :reverse="false"
                    line-cap="round"
                    :fill="smCircleFill"
                    empty-fill="#fff"
                    :animation-start-value="0.0"
                    :start-angle="30"
                    insert-mode="append"
                    :animation="{ duration: 1200, easing: 'easeOutBounce' }"
                    :thickness="8"
                    :show-percent="false"
                  >
                    <div class="c-health-score">
                      <div class="t1">{{ rd.smScore }}</div>
                      <div class="t2">健康值</div>
                    </div>
                  </vue-circle>

                </div>
                <div class="process-text">
                  <div class="t1">{{ userInfo.signName }}</div>
                  <div class="t2">{{ rd.smScoreTip }}</div>
                </div>
              </div>
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

          <div class="b-part-4">
            <div class="mi-title">今日建议</div>
            <div class="mi-content">
              <div class="mi-icon" />
              <div class="mi-suggest">
                <pre style="white-space: pre-line;">{{ rd.rptSuggest }}</pre>
              </div>
            </div>
          </div>

        </mt-tab-container-item>
        <mt-tab-container-item id="tab3">
          <div class="c-part-1">
            <div class="t1"> 敬请期待</div>
            <div class="t2"> 谢谢关注，即将开启</div>
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
import { getDetails } from '@/api/dayreport'
import DvItem from '@/components/DvItem.vue'
import CardOwnA from './components/CardOwnA.vue'
import CardOwnB from './components/CardOwnB.vue'
import VueCircle from 'vue2-circle-progress'
export default {
  components: {
    Carousel3d,
    Slide,
    DvItem,
    CardOwnA,
    CardOwnB,
    VueCircle
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
        signName: ''
      },
      rd: {
        healthScore: 60,
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
        rptSuggest: ''
      },
      carousel: {
        space: 80,
        display: 7
      },
      smCircleFill: { gradient: ['#8316bd', '#fff', '#ad1da3'] },
      activeTabBySmTag: 0,
      theme: 'pink'
    }
  },
  created() {
    this.rptId = this.$route.query.rptId
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
          this.rd = d.rd
          this.userInfo = d.userInfo
          this.activeTabBySmTag = d.rd.smTags[0].id
        }
        this.loading = false
      })
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

  background-repeat: no-repeat;
  background-size: 100% 100%;
  background-size: cover;
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

  height: 100vh;

  .t1 {
    font-size: 24px;

    padding: 10px;
  }

  .t2 {
    font-size: 32px;

    padding: 20px;
  }
}

.theme-pink {
  .a-part-1 {
    background: url('~@/assets/report/day/pink/a_part1_bg.png');
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
    }

    .lm-tabs-title .active {
      background: #c3c1fb;
    }
  }

  .b-part-1 {
    background: url('~@/assets/report/day/pink/b_part1_bg.png') no-repeat top center;
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
