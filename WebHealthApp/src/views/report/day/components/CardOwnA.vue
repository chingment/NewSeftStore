<template>
  <div class="card-own-a">
    <div class="own-info">
      <div class="tip-remark">
        本次检测结果不作为疾病的专业临床诊断依据
      </div>
      <div v-if="userInfo.careMode==25" class="wrap">
        <div class="st-1">
          <div class="field" style="width: 100%;text-align: left">
            <div class="gesweek" style="padding-bottom: 15px;">{{ rd.healthDate }}</div>
            <div class="gesweek" style="padding-bottom: 15px;">您好，{{ userInfo.signName }}</div>
            <div class="gesweek" style="padding-bottom: 15px;">孕{{ userInfo.pregnancy.gesWeek }} 周+{{ userInfo.pregnancy.gesDay }}天</div>
            <div class="birthlastdays">距离宝宝出生还有<span>{{ userInfo.pregnancy.birthLastDays }}</span>天</div>
          </div>
        </div>
        <div class="st-2">
          <vue-circle
            v-if="rd.healthScore.value>0"
            ref="health_score"
            :progress="rd.healthScore.value"
            :size="90"
            :reverse="false"
            line-cap="round"
            :fill="fill"
            empty-fill="#fff"
            :animation-start-value="0.0"
            :start-angle="30"
            insert-mode="append"
            :animation="{ duration: 1200, easing: 'easeOutBounce' }"
            :thickness="5"
            :show-percent="false"
          >
            <div class="c-health-score">
              <div class="t1" :style="'color:'+rd.healthScore.color+';'">{{ rd.healthScore.value }}</div>
              <div class="t2">健康值</div>
            </div>
          </vue-circle>
        </div>
      </div>
      <div v-else class="wrap">
        <div class="pt1">
          <div class="st-1">
            <div class="field">
              <img class="avatar" :src="userInfo.avatar">
              <div class="sign-name">{{ userInfo.signName }}</div>
              <div class="health-date">{{ rd.healthDate }}</div>
            </div>
          </div>
          <div class="st-2">
            <vue-circle
              v-if="rd.healthScore.value>0"
              ref="health_score"
              :progress="rd.healthScore.value"
              :size="90"
              :reverse="false"
              line-cap="round"
              :fill="fill"
              empty-fill="#fff"
              :animation-start-value="0.0"
              :start-angle="30"
              insert-mode="append"
              :animation="{ duration: 1200, easing: 'easeOutBounce' }"
              :thickness="5"
              :show-percent="false"
            >
              <div class="c-health-score">
                <div class="t1" :style="'color:'+rd.healthScore.color+';'">{{ rd.healthScore.value }}</div>
                <div class="t2">健康值</div>
              </div>
            </vue-circle>
            <div class="c-health-score-tip">{{ rd.healthScoreTip }}</div>
          </div>
        </div>
        <div class="pt2">

          <div class="pt2-lt">

            <div class="dv-hrvxzznl">
              <div class="t1">
                <img class="t1_bg" :src="require('@/assets/report/day/green/ic_hrvXzznl.png')">
                <span class="t1_txt" :style="'color:'+rd.hrvXzznl.color+';'"> {{ rd.hrvXzznl.value }}</span>
              </div>
              <div class="t2"><span v-if="rd.hrvXzznl.sign!='-'" :style="'color:'+rd.hrvXzznl.color+';'"> {{ rd.hrvXzznl.sign }}</span> 心脏总能量</div>
            </div>
            <div class="dv-hrvxzznl-ref">（参考值{{ rd.hrvXzznl.refRange }}）</div>

          </div>
          <div class="pt2-rt">

            <div class="dv-xldcjzxl">
              <div class="t1">
                <img class="t1_bg" :src="require('@/assets/report/day/green/ic_xlDcjzxl.png')">
                <span class="t1_txt" :style="'color:'+rd.xlDcjzxl.color+';'"> {{ rd.xlDcjzxl.value }}</span>
              </div>
              <div class="t2"> 当次基准心率</div>
            </div>
            <div class="dv-xldcjzxl-ref">（参考值{{ rd.xlDcjzxl.refRange }}）</div>

          </div>
        </div>

      </div>
    </div>
  </div>
</template>

<script>
import VueCircle from 'vue2-circle-progress'
export default {
  name: 'CardOwnA',
  components: {
    VueCircle
  },
  props: {
    userInfo: {
      type: Object,
      default: null
    },
    rd: {
      type: Object,
      default: null
    }
  },
  data() {
    return {
      fill: { gradient: ['#80a6ef', '#fff', '#80a6ef'] },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {

  }
}
</script>

<style lang="scss" scoped>

.card-own-a {
  .tip-remark {
    font-size: 12px;

    color: #572b9e;
  }

  .own-info {
    .wrap {
      display: flex;
      flex-direction: column;

      min-height: 147px;
    }

    .pt1 {
      display: flex;

      .st-1 {
        flex: 1;

        .field {
          width: 100px;

          text-align: center;

          .avatar {
            width: 90px;
            height: 90px;

            border: 3px solid #fff;
            border-radius: 50%;
          }

          .sign-name {
            padding: 5px;
          }
        }
      }

      .st-2 {
        flex: none;

        width: 100px;

        text-align: center;

        .c-health-score {
          font-weight: bold;

          margin-top: -10px;

          text-align: center;

          color: #572b9e;

          .t1 {
            font-size: 38px;

            padding: 5px;
          }

          .t2 {
            font-size: 12px;
          }
        }

        .c-health-score-tip {
          color: #fff;
        }
      }
    }

    .pt2 {
      display: flex;

      .pt2-lt {
        display: flex;
        align-items: flex-start;
        flex: 1;
        flex-direction: column;
        justify-content: flex-start;
      }

      .pt2-rt {
        display: flex;
        align-items: flex-end;
        flex: 1;
        flex-direction: column;
        justify-content: flex-end;
      }
    }

    .dv-hrvxzznl {
      display: flex;

      padding: 15px 0 5px 0;

      .t1 {
        position: relative;

        .t1_bg {
          width: 60px;
        }

        .t1_txt {
          font-size: 13px;
          font-weight: bold;

          position: absolute;
          top: 12px;
          left: 13px;

          color: blue;
        }
      }

      .t2 {
        padding-top: 10px;
      }
    }

    .dv-hrvxzznl-ref {
      font-size: 14px;
    }

    .dv-xldcjzxl {
      display: flex;

      padding: 5px 0 5px 0;

      .t1 {
        position: relative;

        .t1_bg {
          width: 50px;
        }

        .t1_txt {
          font-size: 13px;
          font-weight: bold;

          position: absolute;
          top: 16px;
          left: 18px;

          color: blue;
        }
      }

      .t2 {
        padding-top: 15px;
      }
    }

    .dv-xldcjzxl-ref {
      font-size: 14px;
    }
  }
}

</style>

