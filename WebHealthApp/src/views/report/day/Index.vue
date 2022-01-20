<template>
  <div :class="'page-tabbar ' +theme">
    <div class="page-wrap">
      <mt-tab-container v-model="selected" class="page-tabbar-container">
        <mt-tab-container-item id="tab1">
          <div class="a-part-1">
            <div class="part-wrap">
              <div class="st-1">
                1
              </div>
              <div class="st-2">
                <div>65</div>
                <div>健康值</div>
              </div>
              <div class="st-3">
                3
              </div>
            </div>
          </div>
          <div class="a-part-2">
            <div style="padding:10px">
              <carousel-3d :space="carousel.space" :display="carousel.display">
                <slide v-for="(item, index) in rd.gzTags" :key="index" :index="index">

                  <div
                    class="poster-item"
                    :data-index="index"
                  >
                    <img class="image_item" :src="require('@/assets/report/day/pink/icm.png')">
                    <span class="name">{{ item.name }}</span>
                    <span class="value">{{ item.value }}</span>
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
                  <process-circle :stroke-width="8" stroke-color="#b71e9d" :trail-width="6" trail-color="#fff" :percent="rd.smScore">
                    <div class="t1">{{ rd.smScore }}</div>
                    <div class="t2">睡眠值</div>
                  </process-circle>
                </div>
                <div class="process-text">
                  <div class="t1">{{ userInfo.signName }}</div>
                  <div class="t2">{{ rd.smScoreTip }}</div>
                </div>
              </div>
            </div>
          </div>
          <div class="b-part-3">
            <div class="mi-title">检测结果</div>
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
                <pre data-v-616ccf7e="" style="white-space: pre-line;">1.睡前要保持平和心态，不能太过兴奋，否则容易导致浅睡和噩梦连连。
2.晚上不要吃太饱，睡前避免食用咖啡、巧克力、可乐、茶等食品或饮料。</pre>
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
import ProcessCircle from '@/components/ProcessCircle/Index.vue'
import DvItem from '@/components/DvItem.vue'
export default {
  components: {
    Carousel3d,
    Slide,
    ProcessCircle,
    DvItem
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
        healthScore: '',
        healthDate: '',
        smScore: 40,
        smScoreTip: '您的睡眠值已经打败77%的人',
        smTags: [],
        gzTags: [
          {
            id: '1',
            image: require('@/assets/report/day/pink/icm.png'),
            yanse: '#fff',
            name: 'sfsff'
          },
          {
            id: '2',
            image: require('@/assets/report/day/pink/icm.png'),
            yanse: '#fff',
            name: 'sfsff'
          },
          {
            id: '3',
            image: require('@/assets/report/day/pink/icm.png'),
            yanse: '#fff',
            name: 'sfsff'
          },
          {
            id: '4',
            image: require('@/assets/report/day/pink/icm.png'),
            yanse: '#fff',
            name: 'sfsff'
          },
          {
            id: '5',
            image: require('@/assets/report/day/pink/icm.png'),
            yanse: '#fff',
            name: 'sfsff'
          },
          {
            id: '6',
            image: require('@/assets/report/day/pink/icm.png'),
            yanse: '#fff',
            name: 'sfsff'
          },
          {
            id: '7',
            image: require('@/assets/report/day/pink/icm.png'),
            yanse: '#fff',
            name: 'sfsff'
          }
        ],
        smDvs: []
      },
      carousel: {
        space: 80,
        display: 7
      },
      activeTabBySmTag: 0,
      theme: 'theme-pink'
    }
  },
  created() {
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

.a-part-1{
    background-repeat:no-repeat;
    background-size: cover;
    height: 240px;

    .part-wrap{
        display: flex;
    }
}

.mint-tabbar{
    background-color: #fff;
    background-size: 100% 0px;
}

.mint-tabbar > .mint-tab-item.is-selected {
    background-color: #fff;
    color: #5478ff;
}

/deep/ .carousel-3d-container {
  height: 160px !important;
  // background-color: #fff;
}
/deep/ .carousel-3d-slider {
  width: 168px !important;
  height: 168px !important;
  background: unset;
}
/deep/ .carousel-3d-slide {
  width: 168px !important;
  height: 168px !important;
  background: unset;
  border: none;
}
.poster-item {
  // background: #fff;
 width: 168px;
  height: 168px;
  border-radius: 10px;
  transition: all 0.5s;
  cursor: default;
  -moz-transition: all 0.5s;
  cursor: default;
  -webkit-transition: all 0.5s;
  cursor: default;
  -o-transition: all 0.5s;
  cursor: default;
  position: absolute;

  .name{
    position: absolute;
    top: 30px;
    left: 35px;
    font-size: 12px;
  }
  .value{
    position: absolute;
    top: 26px;
    right: 35px;
    font-size: 14px;
    font-weight: bold;
  }
}

.lm-tabs{
    padding: 0px 20px 20px 20px;
}
.lm-tabs-title{
 display: flex;
}
.lm-tabs-title-item{
    flex: 1;
    height: 42px;
    justify-content: center;
    align-items: center;
    display: flex;
    font-size: 12px;
    font-weight: bold;
    padding: 0px 5px;
}
.lm-tabs-title .active {
   border-top-right-radius: 10px;
   border-top-left-radius: 10px;
   background-size: 100% 100%;
 }

.lm-tabs-content{
    margin-top: -1px;

}
.lm-tabs-content-item{
    background-size: cover;
    min-height: 300px;
    border-radius: 10px;
    padding: 18px;
    box-shadow: 0 2px 12px 0 rgba(0,0,0,.1);
 }
.lm-tabs-content-item:first-child{
   border-top-left-radius: 0px;
 }
 .lm-tabs-content-item:last-child{
   border-top-right-radius: 0px;
 }

.smTags{
.tag-name{
    font-weight: bold;
}

.tag-proexplain{
    color:#53535b;

    .t{
padding: 8px 0px;
    }

    .c{
        font-size: 14px;
         line-height: 21px;
    }
}
.tag-suggest{
    color:#53535b;

    .t{
padding: 8px 0px;
    }
    .c{
        font-size: 14px;
        line-height: 21px;
    }
}
}

.b-part-1{
    background-repeat:no-repeat;
    background-size: cover;
    padding: 20px;
    .sm-score-box{
      position: relative;
    .process-bg{
    display: flex;
    background: #fff;
    opacity: 0.2;
    height: 120px;
    border-radius: 10px;

    }
    .process-ct{
    position: absolute;
    top: 0;
    left: 0;
    display: flex;
    height: 100%;
    width: 100%;
    }

    .process-score{
     display: flex;
    justify-content: center;
    align-items: center;
    align-content: center;
    width: 120px;
    color:#fff;
        .t1{
    font-size: 32px;
    font-weight: bold;
    }
 .t2{
    font-size: 12px;
    padding: 8px 0px;
    }
    }

    .process-text{
      display: flex;
    flex-direction: column;
    justify-content: center;
    color:#fff;
    .t1{
font-size: 24px;
    }
 .t2{
padding: 5px 0px;
    }

    }
  }
}

.b-part-3{
    padding: 0px 20px;
display: flex;
flex-direction: column;
.mi-title{
    font-weight: bold;
    font-size: 16px;
    line-height: 38px;
    height: 38px;
}

  .mi-item{
    width: 50%;
    float: left;
    margin-bottom: 8px;

    .wrap{
      padding: 10px;
      background-color: #fff;
      border-radius: 12px;

      .item-n{
 padding: 6px 0px;
    font-size: 16px;
    font-weight: 600;
    color: #2d3142;
      }

      .item-v{
 padding: 4px 0px;
 font-size: 14px;
 font-weight: 600;
      }
    }
  }

    .mi-item_0{
    padding-right: 4px;
  }

   .mi-item_1{
    padding-left: 4px;
  }

}

.b-part-4{
    padding: 0px 20px 20px 20px;
display: flex;
flex-direction: column;
.mi-title{
    font-weight: bold;
    font-size: 24px;
    padding: 12px 0px;
}
.mi-content{
      padding: 10px 6px;
      background-color: #fff;
      border-radius: 5px;
      min-height: 200px;
      display: flex;
      .mi-icon{
width: 50px;
  background: url('~@/assets/report/day/pink/b_part4_icon.jpg') no-repeat;
background-size:100%;
      }
       .mi-suggest{
    flex: 1;
    color: #53535b;
    font-size: 12px;
    padding: 0px 6px;
    line-height: 21px;
    letter-spacing: 3px;
      }
}
}

.c-part-1{
      display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    justify-content: center;
    height: 100vh;

    .t1{
      padding: 10px;
      font-size: 24px;
    }
    .t2{
      padding: 20px;
      font-size: 32px;
    }
}

.theme-pink{
    .a-part-1{
    background: url('~@/assets/report/day/pink/a_part1_bg.png');
    }

   .a-part-3{
    .lm-tabs-content-item{
    background: url('~@/assets/report/day/pink/sm_tag_content.png') no-repeat top center;
    }

    .lm-tabs-title .active {
   background: #c3c1fb;
    }
   }

    .b-part-1{
   background: url('~@/assets/report/day/pink/b_part1_bg.png') no-repeat top center;
    }
}

.theme-green{
    .a-part-1{
 background: url('~@/assets/report/day/green/a_part1_bg.png');
    }

.a-part-3{
      .lm-tabs-content-item{
    background: url('~@/assets/report/day/green/sm_tag_content.png') no-repeat top center;
    color: #fff;
    }

     .lm-tabs-title .active {
   background: #09747d;
    color: #fff;
    }

    .tag-name,.tag-proexplain,.tag-suggest{
          color: #fff;
    }
}

  .b-part-1{
   background: url('~@/assets/report/day/green/b_part1_bg.png') no-repeat top center;
    }

}

</style>
