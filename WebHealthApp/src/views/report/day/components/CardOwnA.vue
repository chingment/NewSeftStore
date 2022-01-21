<template>
  <div class="card-own-a">
    <div class="own-info">
      <div class="wrap">
        <div class="st-1">
          <img class="avatar" :src="userInfo.avatar" style="width:100px;height:100px">
          <div class="sign-name">{{ userInfo.signName }}</div>
          <div class="health-date">{{ rd.healthDate }}</div>
        </div>
        <div class="st-2">
          <vue-circle
            ref="myprogress"
            :progress="rd.healthScore"
            :size="100"
            :reverse="false"
            line-cap="round"
            :fill="fill"
            empty-fill="#fff"
            :animation-start-value="0.0"
            :start-angle="0"
            insert-mode="append"
            :animation="{ duration: 1200, easing: 'easeOutBounce' }"
            :thickness="5"
            :show-percent="false"
          >
            <div class="c-health-score">
              <div class="t1">{{ rd.healthScore }}</div>
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

.card-own-a{

  .own-info{
    display: flex;
    .wrap{
      width: 100%;
    }
.st-1{
    width: 100px;
    float: left;
    text-align: center;
.avatar{
      width: 100px;
    height: 100px;
    border-radius: 50%;
    border: 5px solid #fff;
}
}

.st-2{
    width: 100px;
    float: right;
    text-align: center;
   .c-health-score{
        text-align: center;
    color: #572b9e;
    margin-top: -10px;
    font-weight: bold;
    .t1{
    font-size: 38px;
    padding: 5px
    }

     .t2{
    font-size: 12px;
    }
   }
.c-health-score-tip{
    color: #fff;
}
}
  }

  .own-remark{
   color: #572b9e;
     font-size: 12px;
  }
}

</style>

