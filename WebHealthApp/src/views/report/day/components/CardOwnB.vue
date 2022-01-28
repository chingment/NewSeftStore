<template>
  <div class="card-own-b">
    <div v-if="rd!=null" class="own-info">
      <div class="tip-remark">
        本次检测结果不作为疾病的专业临床诊断依据
      </div>
      <div class="wrap">
        <div class="st st-1">
          <div class="health-date">{{ rd.healthDate }}</div>
          <img class="avatar" :src="userInfo.avatar" style="width:100px;height:100px">
        </div>
        <div class="st st-2">
          <div class="t1" :style="'color:'+rd.healthScore.color+';'">{{ rd.healthScore.value }}</div>
          <div class="t2">健康值</div>
          <div class="t3">今天是全心监测与您相伴的</div>
          <div class="t4">第<span class="t5">{{ userInfo.togetherDays }}</span>天</div>
        </div>
        <div class="st st-3">
          <div class="dv-hrvxzznl">
            <div class="t1">
              <img class="t1_bg" :src="require('@/assets/report/day/green/ic_hrvXzznl.png')">
              <span class="t1_txt" :style="'color:'+rd.hrvXzznl.color+';'"> {{ rd.hrvXzznl.value }}</span>
            </div>
            <div class="t2"> 心脏总能量</div>
          </div>
          <div class="dv-hrvxzznl-ref">（参考值{{ rd.hrvXzznl.refRange }}）</div>
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
  .tip-remark{
    color: #fff;
    font-size: 12px;
  }
  .wrap {
    display: flex;

    color: #fff;

    .st-1 {
      .health-date{
        padding-top: 10px;
      }
      .avatar {
        position: absolute;
        top: 120px;

        width: 90px;
        height: 90px;

        border-radius: 50%;
      }
    }

    .st-2 {
      text-align: center;

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

  .st-1 {
    flex: none;

    width: 90px;
  }

  .st-3 {
    flex: none;

    width: 150px;

    .dv-hrvxzznl {
      display: flex;

      padding: 12px 0 5px 0;

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
      font-size: 12px;
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
    position: absolute;
    top: 16px;
    left: 18px;
    color: blue;
    font-weight: bold;
        }
      }

      .t2 {
        padding-top: 15px;
      }
    }

    .dv-xldcjzxl-ref {
      font-size: 12px;
    }
  }
}

</style>

