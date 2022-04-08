<template>
  <div class="card-own-b">
    <div v-if="rd!=null" class="own-info">
      <div class="tip-remark">
        本次检测结果不作为疾病的专业临床诊断依据
      </div>
      <div class="wrap">
        <div class="pt1">
          <div class="pt1-lt">
            <img class="avatar" :src="userInfo.avatar" alt="">
          </div>
          <div class="pt1-md">
            <div class="t1">{{ rd.healthScore.value }}</div>
            <div class="t2">健康值</div>
          </div>
          <div class="pt1-rt">
            <div style="max-width: 200px;">
              <div class="health-date">{{ rd.healthDate }}</div>
              <div class="health-tword">今天是全心监测与您相伴的</div>
              <div class="health-tday">第<span class="t5">{{ userInfo.togetherDays }}</span>天</div>
            </div>
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
  .tip-remark {
    font-size: 12px;

    color: #fff;
  }

  .wrap {
    display: flex;
    flex-direction: column;

    color: #fff;

    .pt1 {
      display: flex;

      .pt1-lt {
        display: flex;
        align-items: center;
        flex: 1;

        .avatar {
          width: 85px;
          height: 85px;

          border-radius: 50%;
        }
      }

      .pt1-md {
        display: flex;
        align-items: center;
        flex-direction: column;
        justify-content: center;

        width: 100px;
        width: 100px;

        .t1 {
          font-size: 68px;
          font-weight: bold;
        }

        .t2 {
          font-size: 16px;

          padding: 5px;
        }
      }

      .pt1-rt {
        display: flex;
        flex: 1;
        flex-direction: column;
        justify-content: center;
        align-items: flex-end;
        text-align: center;

        .t5 {
          font-size: 23px;
          font-weight: bold;

          padding: 0 5px;
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

    .health-date {
      padding-bottom: 5px;
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

@media screen and (min-width: 300px) and (max-width: 420px) {
  .st-1 {
    flex: none;

    width: 90px;
  }

  .st-2 {
    flex: 1;
  }

  .st-3 {
    flex: none;

    width: 150px;
  }
}

@media screen and (min-width: 420px) {
  .st-1 {
    flex: none;

    width: 150px;
  }

  .st-2 {
    flex: 1;
  }

  .st-3 {
    flex: none;

    width: 150px;
  }
}

</style>

