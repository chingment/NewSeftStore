<template>
  <div class="pg-monitor">
    <div class="pt1">
      <div class="cd">
        <div class="lf">
          <span class="signName">{{ userInfo.signName }}</span>
        </div>
        <div class="rf">
          <div class="t1"><img class="avatar" :src="userInfo.headImgurl" alt=""></div>
          <div class="t2"><span class="month">{{ rd.healthDate }}</span> <img class="more" src="@/assets/images/ts/arrow_right.png" alt="">  </div>
        </div>
      </div>
    </div>
    <div class="pt2">
      <div class="cd" @click="goEnery()">
        <div class="lf">
          <div class="rd">
            <div class="st">
              <div class="score">{{ rd.totalScore }}</div>
              <div class="title">本月得分</div>
            </div>
          </div>
        </div>
        <div class="rf">
          <div class="notic">全线通精准健康管理平台守护您的健康!</div>
        </div>
      </div>
    </div>
    <div class="pt3">
      <div class="cd">
        <div class="smtags">
          <template v-for="(item, index) in rd.smTags">
            <div v-if="index<=3" :key="index" :class="'item item_'+(index%2==0?'0':'1')">
              <div class="item-ct">
                <div class="tl"><span class="name">{{ item.name }} </span>  <img class="icon" style="display:none" src="@/assets/images/ts/极难入睡.png" alt=""></div>
                <div class="rd">
                  <div class="st"> <span class="count"> {{ item.count }} </span> <span class="unit">次</span> </div>
                </div>
              </div>
            </div>
          </template>
        </div>
      </div>
    </div>
    <div class="pt4">
      <div class="cd">
        <div class="summary-title">本月健康报告总结</div>
        <div class="summary-card">
          <div class="summary-card__header">
            <div class="ct">
              <div class="title">总结：</div>
              <div class="content">
                <pre style="white-space: pre-line;">{{ rd.rptSummary }}</pre>
              </div>
            </div>
          </div>
          <div class="summary-card_body">
            <div class="ct">
              <div class="title">健康建议：</div>
              <div class="content"><pre style="white-space: pre-line;">{{ rd.rptSuggest }}</pre></div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script>

import { getMonitor } from '@/api/monthreport'
import DvItem from '@/components/DvItem.vue'
import 'swiper/dist/css/swiper.min.css'
import 'swiper/dist/js/swiper.min'
export default {
  name: 'Report',
  components: { DvItem },
  data() {
    return {
      loading: false,
      userInfo: {
        signName: ''
      },
      rd: {
        scoreRatio: '',
        smSmsc: { color: '', value: '', refRange: '' },
        smQdsmsc: { color: '', value: '', refRange: '' },
        smSdsmsc: { color: '', value: '', refRange: '' },
        smRemsmsc: { color: '', value: '', refRange: '' },
        hrvXzznl: { color: '', value: '', refRange: '' },
        hxDcpjhx: { color: '', value: '', refRange: '' },
        xlDcpjxl: { color: '', value: '', refRange: '' },
        hxZtcs: { color: '', value: '', refRange: '' },
        smTdcs: { color: '', value: '', refRange: '' },
        hxZtahizs: { color: '', value: '', refRange: '' },
        rptSummary: '',
        rptSuggest: ''
      }
    }
  },
  created() {
    this._getMonitor()
  },
  methods: {
    _getMonitor() {
      this.loading = true
      getMonitor({ rptId: this.$route.query.rptId }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.userInfo = d.userInfo
          this.rd = d.reportData
        }
        this.loading = false
      })
    },
    goEnery() {
      this.$router.push('/report/month/energy?rptId=' + this.$route.query.rptId)
    }
  }
}
</script>

<style lang="scss" scoped>

.pg-monitor{
  height:100%;
}

.pt1{
    background-image: url('~@/assets/images/ts/bg_pt1.png');
    height: 240px;
    padding: 16px;
  background-size: 100% 100%;
    .cd{
      display: flex;
      padding-top: 20px;
      .lf{
        flex: 1;
      .signName{
        color:#fff;
        font-weight: 600;
      }
      }

      .rf{
        flex: 1;

        .t1{
          text-align: right;
        .avatar{
          width: 50px;
          height: 50px;
          border-radius: 50%;
          margin-right: 14px;
        }
        }

        .t2{
          color: #fff;
    display: flex;
    justify-content: flex-end;
    align-items: center;

.more{
      width: 18px;
    height: 18px
}
        }
      }
    }
}

.pt2{
    margin-top: -120px;
    padding: 16px;
    .cd{
       background-color: #fff3e8;
        border-radius: 10px;
    display: flex;
    padding: 30px 15px;
      display: flex;
      .lf{
      width: 120px;

      .rd{
            width: 100px;
    height: 100px;
    border-radius: 50%;
        border: solid 6px #fff;
    justify-content: center;
    align-items: center;
    display: flex;
    text-align: center;

    .score{
      color:#ffbd73;
      font-weight: 800;
      font-size: 21px;
    }

     .title{
      color:#ffbd73;
      font-size: 12px;
      margin-top: 8px;
      font-weight: 600;
    }

      }
      }
      .rf{
    flex: 1;
    display: flex;
    align-items: center;
    line-height: 22px;
    font-weight: 600;
    padding: 0px 20px;
.notic{
   color:#ffbd73;
}
      }
    }
}

.pt3{
   padding: 0px 16px 0px 16px;
    display: flex;
  .cd{
width: 100%;
.smtags{

  .item{
    width: 50%;
    float: left;
    margin-bottom: 20px;
  }

  .item-ct{
    background-image: url('~@/assets/images/ts/bg_smtag.png');
    background-size: 100% 100%;
    height: 180px;
    max-width: 180px;
    margin: auto;
    .tl{
      height: 50px;
      margin-left: 10px;
      font-size: 12px;
    display: flex;
    justify-content: flex-start;
    align-items: center;
 color:#606060;
.icon{
      width: 18px;
    height: 18px;
    margin-left: 5px;
}

    }
.rd{
            width: 100px;
    height: 100px;
    border-radius: 50%;
        border: solid 3px #fff;
    justify-content: center;
    align-items: center;
    display: flex;
    text-align: center;
    margin: auto;
    .count{
      color:#fff;
      font-weight: 600;
      font-size: 32px;
    }
    .unit{
       font-size: 14px;
           color:#fff;
    }

      }

  }

  .item_0{
    padding-right: 10px;
  }

   .item_1{
    padding-left: 10px;
  }
}
  }
}

.pt4{
     padding: 0px 16px 0px 16px;
}

.summary-title{
  font-weight: 600;
  text-align: center;
  height: 40px;
  line-height: 40px;
}

.summary-card{
    box-shadow: 0 2px 12px 0 rgba(0,0,0,.1);
    border: 1px solid #ebeef5;
    background-color: #fff;
    color: #303133;
    -webkit-transition: .3s;
    transition: .3s;
    border-radius: 10px;
    overflow: hidden;
    margin-bottom: 10px;

}

.summary-card__header{
    min-height: 100px;
    box-sizing: border-box;
    background: url('~@/assets/images/ts/bg_summary-card__header.png') no-repeat;;
    background-size: cover;
}

.summary-card__header {
  .ct{
padding: 20px;

.title{
    font-weight: bold;
    line-height: 32px;
    font-size: 16px
}

.content{
    color: #616161;
    font-size: 14px;
    line-height: 21px;
}

  }
}
.summary-card_body{

.ct{
padding: 20px;
.title{
    font-weight: bold;
    line-height: 32px;
    font-size: 16px
}

.content{
    color: #616161;
    font-size: 14px;
    line-height: 21px;
}
}
}

</style>
