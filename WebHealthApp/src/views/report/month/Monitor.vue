<template>
  <div class="pg-monitor">
    <div class="user-info">

      <div class="t1"> <img class="avatar" src="@/assets/images/default_avatar.png" alt=""></div>
      <div class="t2"> <span>用户nic</span></div>
      <div class="t3"><span>2021-4</span></div>

      <div class="t4">
        <div class="lf">
          <span class="tt1"> <span class="tt2">本月得分</span><span class="tt3">60</span></span>
        </div>
        <div class="rf">
          <span class="tt1"><span class="tt2">得分超过人数</span><span class="tt3">41%</span></span>
        </div>
      </div>

    </div>
    <div class="sum-report">
      <div class="title"><span>月报告统计</span></div>
      <div class="dvit">
        <div class="dvit-head">
          <div class="t1"><span>指标(日均)</span></div>
          <div class="t2"><span>测量值</span></div>
          <div class="t3"><span>参考值</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>睡眠时长</span></div>
          <div class="t2"><span :style="{'color': rd.smSmsc.color}">{{ rd.smSmsc.value }}</span></div>
          <div class="t3"><span>{{ rd.smSmsc.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>浅睡眠时长</span></div>
          <div class="t2"><span :style="{'color': rd.smQdsmsc.color}">{{ rd.smQdsmsc.value }}</span></div>
          <div class="t3"><span>{{ rd.smQdsmsc.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>深睡眠时长</span></div>
          <div class="t2"><span :style="{'color': rd.smSdsmsc.color}">{{ rd.smSdsmsc.value }}</span></div>
          <div class="t3"><span>{{ rd.smSdsmsc.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>REM睡眠时长</span></div>
          <div class="t2"><span :style="{'color': rd.smRemsmsc.color}">{{ rd.smRemsmsc.value }}</span></div>
          <div class="t3"><span>{{ rd.smRemsmsc.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>心脏总能量</span></div>
          <div class="t2"><span :style="{'color': rd.hrvXzznl.color}">{{ rd.hrvXzznl.value }}</span></div>
          <div class="t3"><span>{{ rd.hrvXzznl.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>平均呼吸</span></div>
          <div class="t2"><span :style="{'color': rd.hxDcpjhx.color}">{{ rd.hxDcpjhx.value }}</span></div>
          <div class="t3"><span>{{ rd.hxDcpjhx.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>平均心率</span></div>
          <div class="t2"><span :style="{'color': rd.xlDcpjxl.color}">{{ rd.xlDcpjxl.value }}</span></div>
          <div class="t3"><span>{{ rd.xlDcpjxl.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>呼吸暂停</span></div>
          <div class="t2"><span :style="{'color': rd.hxZtcs.color}">{{ rd.hxZtcs.value }}</span></div>
          <div class="t3"><span>{{ rd.hxZtcs.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>体动</span></div>
          <div class="t2"><span :style="{'color': rd.smTdcs.color}">{{ rd.smTdcs.value }}</span></div>
          <div class="t3"><span>{{ rd.smTdcs.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>AHI指数</span></div>
          <div class="t2"><span :style="{'color': rd.hxZtahizs.color}">{{ rd.hxZtahizs.value }}</span></div>
          <div class="t3"><span>{{ rd.hxZtahizs.refRange }}</span></div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'Report',
  data() {
    return {
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
  }
}
</script>

<style lang="scss" scoped>

.user-info{
  background: #2b9df9;
  height: 230px;
  padding: 20px;
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
</style>
