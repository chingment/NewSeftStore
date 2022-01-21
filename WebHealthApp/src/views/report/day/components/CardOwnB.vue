<template>
  <div class="card-own-b">
    <div class="own-info">
      <div class="wrap">
        <div class="st st-1">
          <div class="health-date">{{ rd.healthDate }}</div>
          <img class="avatar" :src="userInfo.avatar" style="width:100px;height:100px">
        </div>
        <div class="st st-2">
          <div class="t1">56</div>
          <div class="t2">健康值</div>
          <div class="t3">今天是全心监测与您相伴的</div>
          <div class="t4">第<span class="t5">99</span>天</div>
        </div>
        <div class="st st-3">
          <div class="dv-hrvxzznl">  心脏总能量</div>
          <div class="dv-hrvxzznl-ref">（参考范围1230-2460）</div>
          <div class="dv-xldcjzxl">当次基准心率</div>
          <div class="dv_xlcqjzxl">（长期基准心率）</div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'CardOwnB',
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

.card-own-b {
  .wrap {
    display: flex;

    .st-1 {
      .avatar {
        position: absolute;
        top: 100px;

        width: 120px;
        height: 120px;

        border-radius: 50%;
      }
    }

    .st-2 {
      text-align: center;

      color: #fff;

      .t1 {
        font-size: 68px;
        font-weight: bold;
      }

      .t3 {
        font-size: 12px;

        padding: 5px;
      }

      .t4 {
        font-size: 12px;

        display: flex;
        align-items: center;
        justify-content: center;
      }

      .t5 {
        font-size: 23px;
        font-weight: bold;

        padding: 0 5px;
      }
    }
  }
}

@media screen and (min-width: 300px) and (max-width: 800px) {
  .st-2 {
    flex: 1;
  }

  .st-1,
  .st-3 {
    flex: none;

    width: 100px;
  }
}

</style>

