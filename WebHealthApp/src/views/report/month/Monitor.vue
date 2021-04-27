<template>
  <div class="pg-monitor">

    <div class="user-info">

      <div class="t1"> <img class="avatar" :src="userInfo.headImgurl" alt=""></div>
      <div class="t2"> <span>{{ userInfo.signName }}</span></div>
      <div class="t3"><span>{{ rd.healthDate }}</span></div>

      <div class="t4">
        <div class="lf">
          <span class="tt1"> <span class="tt2">本月得分</span><span class="tt3">{{ rd.totalScore }}</span></span>
        </div>
        <div class="rf">
          <span class="tt1"><span class="tt2">得分超过人数</span><span class="tt3">41%</span></span>
        </div>
      </div>

    </div>

    <div class="swiper-container sm-tags">
      <div class="swiper-wrapper">
        <div v-for="(item, index) in rd.smTags" :key="index" class="swiper-slide">
          <div :class="'item item_'+(index<=2?index:2)">
            <div> {{ item.name }} <br>{{ item.count }}</div>
          </div>
        </div>

        <!-- <div class="swiper-slide">标签1</div>
        <div class="swiper-slide">标签2</div>
        <div class="swiper-slide">标签3</div>
        <div class="swiper-slide">标签4</div>
        <div class="swiper-slide">标签5</div>
        <div class="swiper-slide">标签6</div> -->
      </div>
      <!-- 如果需要分页器 -->
      <!-- <div class="swiper-pagination" /> -->

      <!-- 如果需要导航按钮 -->
      <!-- <div class="swiper-button-prev" />
      <div class="swiper-button-next" /> -->

    <!-- 如果需要滚动条 -->
      <!--    <div class="swiper-scrollbar"></div>-->
    </div>

    <div
      class="sum-report"
    >
      <div class="title"><span>月报告统计</span></div>
      <div class="dvit">
        <div class="dvit-head">
          <div class="t1"><span>指标(日均)</span></div>
          <div class="t2"><span>测量值</span></div>
          <div class="t3"><span>参考值</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>睡眠时长</span></div>
          <div class="t2"><dv-item :value="rd.smSmsc" sign /></div>
          <div class="t3"><span>{{ rd.smSmsc.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>浅睡眠时长</span></div>
          <div class="t2"><dv-item :value="rd.smQdsmsc" sign /></div>
          <div class="t3"><span>{{ rd.smQdsmsc.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>深睡眠时长</span></div>
          <div class="t2"><dv-item :value="rd.smSdsmsc" sign /></div>
          <div class="t3"><span>{{ rd.smSdsmsc.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>REM睡眠时长</span></div>
          <div class="t2"><dv-item :value="rd.smRemsmsc" sign /></div>
          <div class="t3"><span>{{ rd.smRemsmsc.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>心脏总能量</span></div>
          <div class="t2"><dv-item :value="rd.hrvXzznl" sign /></div>
          <div class="t3"><span>{{ rd.hrvXzznl.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>平均呼吸</span></div>
          <div class="t2"><dv-item :value="rd.hxDcpjhx" sign /></div>
          <div class="t3"><span>{{ rd.hxDcpjhx.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>平均心率</span></div>
          <div class="t2"><dv-item :value="rd.xlDcpjxl" sign /></div>
          <div class="t3"><span>{{ rd.xlDcpjxl.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>呼吸暂停</span></div>
          <div class="t2"><dv-item :value="rd.hxZtcs" sign /></div>
          <div class="t3"><span>{{ rd.hxZtcs.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>体动</span></div>
          <div class="t2"><dv-item :value="rd.smTdcs" sign /></div>
          <div class="t3"><span>{{ rd.smTdcs.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>AHI指数</span></div>
          <div class="t2"><dv-item :value="rd.hxZtahizs" sign /></div>
          <div class="t3"><span>{{ rd.hxZtahizs.refRange }}</span></div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>

import { getMonitor } from '@/api/monthreport'
import DvItem from '@/components/DvItem.vue'
import Swiper from 'swiper'
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
        smSmsc: { color: '1', value: '0', refRange: '3' },
        smQdsmsc: { color: '', value: '', refRange: '' },
        smSdsmsc: { color: '', value: '', refRange: '' },
        smRemsmsc: { color: '', value: '', refRange: '' },
        hrvXzznl: { color: '', value: '', refRange: '' },
        hxDcpjhx: { color: '', value: '', refRange: '' },
        xlDcpjxl: { color: '', value: '', refRange: '' },
        hxZtcs: { color: '', value: '', refRange: '' },
        smTdcs: { color: '', value: '', refRange: '' },
        hxZtahizs: { color: '', value: '', refRange: '' }
      }

    }
  },
  mounted() {

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
          this.$nextTick(function() {
            new Swiper('.swiper-container', {
              slidesPerView: 3,
              loop: true,
              // 如果需要分页器
              pagination: '.swiper-pagination',
              // 如果需要前进后退按钮
              nextButton: null,
              prevButton: null
            // 如果需要滚动条
            // scrollbar: '.swiper-scrollbar',
            // 如果需要自动切换海报
            // autoplay: {
            //   delay: 1000,//时间 毫秒
            //   disableOnInteraction: false,//用户操作之后是否停止自动轮播默认true
            // },
            })
          }, 2000)
        }
        this.loading = false
      })
    }
  }
}
</script>

<style lang="scss" scoped>

.user-info{
 // background: #2b9df9;
  background-image: url('~@/assets/images/bg_1.jpg');
  height: 230px;
  padding: 20px;
  background-size: 100% 100%;
.t1,.t2,.t3{
  display: flex;
  align-items: center;
  justify-content: center;
}

.t4{
  display: flex;
   padding-top: 50px;
     color: #fff;
  .lf{
    flex: 1;
    align-items: center;
    display: flex;
    justify-content: flex-start;
  }

  .rf{
    flex: 1;
    align-items: center;
    display: flex;
    justify-content: flex-end;
  }

   .tt1 { }
    .tt2 {   float: left;font-size: 16px; }
        .tt3 {  float: left; font-weight: 600;font-size: 30px;      margin-top: -12px;     margin-left: 8px;}
}

.t1{
}

.t2{
  padding-top: 10px;
  color: #fff;
}

.t3{
  padding-top: 10px;
    color: #fff;
}

  .avatar{
    border: 5px solid hsla(0,0%,90%,.5);
    background: #bbb;
    background-clip: padding-box;
    border-radius: 50%;
    height: 80px;
    width: 80px;
  }
}

.sum-report{
background: #fff;
.title{
      height: 60px;
    line-height: 60px;
    margin: 0px 20px;
    border-bottom: 1px solid gainsboro;
}

.dvit{

.dvit-head{
  display: flex;
  color: #4491ed;
  text-align: center;
  padding: 10px 20px;
  font-size: 14px;
}

.dvit-item{
  display: flex;
   padding: 10px 20px;
    font-size: 14px;
    color: #707070;
}

.dvit-item:nth-child(even){
   background: #ebf3fd;
}

.dvit-item:nth-child(odd){

}

.t1{
  flex:1;
  text-align: left;
}
.t2{
  flex:1;
  text-align: center;
}
.t3{
  flex:1;
  text-align: center;
}

}

}

.sm-tags{
  width: 100%;
  margin-bottom: 10px;
  padding: 20px;
  background: #fff;
  .swiper-wrapper{
    .swiper-slide{
      width: 100%;
      height: 100%;
      text-align: center;
      padding: 0px 10px;
      color:#fff;
      font-weight: 600;
    }
  }

  .item{
    background-size: 100% 100%;
    width: 12Y0px;
    height: 80px;
    display: flex;
    justify-content: center;
    align-items: center;
    line-height: 24px;
  }

  .item_0{
    background-image: url('~@/assets/images/sm_tag_0.jpg');
  }
    .item_1{
    background-image: url('~@/assets/images/sm_tag_1.jpg');
  }
    .item_2{
    background-image: url('~@/assets/images/sm_tag_2.jpg');
  }
}

</style>
