<template>
  <div class="card-own-a">
    <div class="own-info">
      <div class="wrap">
        <div class="st-1">
          <img class="avatar" :src="userInfo.avatar">
          <div class="sign-name">{{ userInfo.signName }}</div>
          <div class="health-date">{{ rd.healthDate }}</div>
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
    </div>
    <div class="own-remark">
      本次检测结果不作为疾病的专业临床诊断依据
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
  .own-info {
    display: flex;

    .wrap {
      width: 100%;
    }

    .st-1 {
      float: left;

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

    .st-2 {
      float: right;

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

  .own-remark {
    font-size: 12px;

    color: #572b9e;
  }
}

</style>

