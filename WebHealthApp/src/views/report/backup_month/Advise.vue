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
        <p>{{ rd.sugByYd }}</p>
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
        <p>{{ rd.sugByYy }}</p>
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
        <p>{{ rd.sugBySm }}</p>
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
        <p>{{ rd.sugByQxyl }}</p>
      </div>
    </div>

  </div>
</template>

<script>

import { getAdvise } from '@/api/monthreport'

export default {
  name: 'Advise',
  data() {
    return {
      loading: false,
      rd: {
        sugByYy: '',
        sugByYd: '',
        sugBySm: '',
        sugByQxyl: ''
      }
    }
  },
  created() {
    this._getAdvise()
  },
  methods: {
    _getAdvise() {
      this.loading = true
      getAdvise({ rptId: this.$route.query.rptId }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.rd = d
        }
        this.loading = false
      })
    }
  }
}
</script>

<style lang="scss" scoped>

.pg-advise{
  padding: 10px;
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
    margin-bottom: 10px;
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
  color: #707070;
  line-height: 24px;
}
}

</style>
