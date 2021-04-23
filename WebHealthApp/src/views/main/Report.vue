<template>
  <div>
    <div class="sum-report">
      <div class="title"><span>月报告统计</span></div>
      <div class="dvit">
        <div class="dvit-head">
          <div class="t1"><span>指标</span></div>
          <div class="t2"><span>测量值</span></div>
          <div class="t3"><span>参考值</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>日均睡眠时长</span></div>
          <div class="t2"><span :style="{'color': rd.smSmsc.color}">{{ rd.smSmsc.value }}</span></div>
          <div class="t3"><span>{{ rd.smSmsc.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>日均浅睡眠时长</span></div>
          <div class="t2"><span :style="{'color': rd.smQdsmsc.color}">{{ rd.smQdsmsc.value }}</span></div>
          <div class="t3"><span>{{ rd.smQdsmsc.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>日均深睡眠时长</span></div>
          <div class="t2"><span :style="{'color': rd.smSdsmsc.color}">{{ rd.smSdsmsc.value }}</span></div>
          <div class="t3"><span>{{ rd.smSdsmsc.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>日均REM睡眠时长</span></div>
          <div class="t2"><span :style="{'color': rd.smRemsmsc.color}">{{ rd.smRemsmsc.value }}</span></div>
          <div class="t3"><span>{{ rd.smRemsmsc.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>日均心脏总能量</span></div>
          <div class="t2"><span :style="{'color': rd.hrvXzznl.color}">{{ rd.hrvXzznl.value }}</span></div>
          <div class="t3"><span>{{ rd.hrvXzznl.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>日均心脏总能量</span></div>
          <div class="t2"><span :style="{'color': rd.hrvXzznl.color}">{{ rd.hrvXzznl.value }}</span></div>
          <div class="t3"><span>{{ rd.hrvXzznl.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>日均呼吸</span></div>
          <div class="t2"><span :style="{'color': rd.hxDcpjhx.color}">{{ rd.hxDcpjhx.value }}</span></div>
          <div class="t3"><span>{{ rd.hxDcpjhx.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>日均心率</span></div>
          <div class="t2"><span :style="{'color': rd.xlDcpjxl.color}">{{ rd.xlDcpjxl.value }}</span></div>
          <div class="t3"><span>{{ rd.xlDcpjxl.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>日均呼吸暂停</span></div>
          <div class="t2"><span :style="{'color': rd.hxZtcs.color}">{{ rd.hxZtcs.value }}</span></div>
          <div class="t3"><span>{{ rd.hxZtcs.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>日均体动</span></div>
          <div class="t2"><span :style="{'color': rd.smTdcs.color}">{{ rd.smTdcs.value }}</span></div>
          <div class="t3"><span>{{ rd.smTdcs.refRange }}</span></div>
        </div>
        <div class="dvit-item">
          <div class="t1"><span>日均AHI指数</span></div>
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
