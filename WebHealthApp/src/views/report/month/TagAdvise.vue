<template>
  <div class="pg-energy" style="height:500px">
    <div class="pt1">
      <div class="at1">
        <div class="at_a1"><span class="at_title">{{ rd.tagName }}</span><span class="at_score">{{ rd.tagCount }}</span> <span class="at_unit">次</span> </div>
      </div>
      <div class="at2">

        <div class="explain-card">
          <div class="explain-card__header">
            <div class="ct">
              <div class="title">解释</div>
              <div class="content">
                <pre style="white-space: pre-line;">{{ rd.proExplain }}</pre>
              </div>
            </div>
          </div>
          <div class="explain-card_body">
            <div class="ct">
              <div class="title">建议：</div>
              <div class="content"><pre style="white-space: pre-line;">{{ rd.suggest }}</pre></div>
            </div>
          </div>

        </div>

      </div>

    </div>

  </div>
</template>

<script>

import { getTagAdvise } from '@/api/monthreport'

export default {
  name: 'TagAdvise',
  data() {
    return {
      loading: false,
      rd: {
        tagName: '',
        tagCount: '',
        proExplain: '',
        tcmExplain: '',
        suggest: ''
      }
    }
  },
  mounted() {

  },
  created() {
    this._getTagAdvise()
  },
  beforeDestroy() {
  },
  methods: {
    _getTagAdvise() {
      this.loading = true
      getTagAdvise({ tagId: this.$route.query.tagId }).then(res => {
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

.pg-energy{
  padding: 16px;
  background: linear-gradient(#ffb24f, #fff)
}

.pt1{
    box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
    background-color: #fff;
    color: #303133;
    -webkit-transition: .3s;
    transition: .3s;
    border-radius: 10px;
    overflow: hidden;
    margin-bottom: 10px;
}

.pt1_t1{
width: 100px;
height: 100px;
position: absolute;
right: 100px;
top: 32px;
}

.pt1_t2{
width: 50px;
    height: 50px;
    position: absolute;
    left: 100px;
    top: 62px;
}

.pt1_t3{
    width: 60px;
    height: 60px;
    position: absolute;
    right: 20px;
    top: 22px;
}

.at1{
  height: 150px;
  background: url('~@/assets/images/ts/bg_energy_pt1_at1.png') no-repeat;
  background-size: 100% 100%;

  .at_a1{
        padding-top: 100px;
    padding-left: 50px;
    color:#ffbd73;

  .at_title{
      font-size: 21px;
    }
    .at_score{
      font-size: 32px;
      margin-left: 10px;
    }

 .at_unit{
      font-size: 21px;
      margin-left: 10px;
    }

  }
}

.explain-card{
    background-color: #fff;
    color: #303133;
    -webkit-transition: .3s;
    transition: .3s;
    overflow: hidden;
    margin-bottom: 10px;
    padding: 0px 20px;
}

.explain-card__header{
    min-height: 100px;
    box-sizing: border-box;
    background: url('~@/assets/images/ts/bg_explain-card__header.png') no-repeat;
    background-size: 100% 100%;
}

.explain-card__header {
  .ct{
      padding: 20px 10px 20px 45px;

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
.explain-card_body{

.ct{
    padding: 20px 10px 20px 45px;
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
