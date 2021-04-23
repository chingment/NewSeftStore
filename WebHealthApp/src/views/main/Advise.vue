<template>
  <div class="pg-advise">
    <div class="lm-card">
      <div class="lm-card__header">
        <div class="ct">
          <img class="icon" src="@/assets/images/icon_advise_yd.png" alt="">
          <span class="text">运动</span>
        </div>
      </div>
      <div class="lm-card_body">
        <p>一般来说自主神经平衡下降的人是适合运动的，这种人对心率的调节能力是较强的，可进行一些兼顾力量与协调性的运动，如篮球，足球，网球，羽毛球等。但对于已经存在高血压的人群来说，运动强度需要谨慎。</p>
      </div>
    </div>

    <div class="lm-card">
      <div class="lm-card__header">
        <div class="ct">
          <img class="icon" src="@/assets/images/icon_advise_yy.png" alt="">
          <span class="text">营养</span>
        </div>
      </div>
      <div class="lm-card_body">
        <p>今天需多吃蛋白质量含量高的食物，如早餐补充一个鸡蛋</p>
      </div>
    </div>

    <div class="lm-card">
      <div class="lm-card__header">
        <div class="ct">
          <img class="icon" src="@/assets/images/icon_advise_sm.png" alt="">
          <span class="text">睡眠</span>
        </div>
      </div>
      <div class="lm-card_body">
        <p>睡眠时间不足，深度睡眠时长必然过少，这将加速身体的衰老，今天务必早睡</p>
      </div>
    </div>

    <div class="lm-card">
      <div class="lm-card__header">
        <div class="ct">
          <img class="icon" src="@/assets/images/icon_advise_qxyl.png" alt="">
          <span class="text">情绪压力</span>
        </div>
      </div>
      <div class="lm-card_body">
        <p>今日不适合过多劳累，需多营养</p>
      </div>
    </div>

  </div>
</template>

<script>
export default {
  name: 'Advise'
}
</script>

<style lang="scss" scoped>

.pg-advise{
  padding: 20px;
}

.lm-card{
    box-shadow: 0 2px 12px 0 rgba(0,0,0,.1);
    border: 1px solid #ebeef5;
    background-color: #fff;
    color: #303133;
    -webkit-transition: .3s;
    transition: .3s;
    border-radius: 4px;
    overflow: hidden;
    margin-bottom: 20px;
}

.lm-card__header{
    padding: 16px 0px;
    margin: 0px 16px;
    border-bottom: 1px solid #ebeef5;
    -webkit-box-sizing: border-box;
    box-sizing: border-box;

}

.lm-card__header > .ct{
    display: flex;
    justify-content: flex-start;
    align-items: center;

    .icon{
      height: 24px;
      width: 24px;
    }
    .text{
      margin-left:10px;
    }
}

.lm-card_body{
padding: 20px;

p{
  color: #5a5353;
  line-height: 24px;
}
}

</style>
