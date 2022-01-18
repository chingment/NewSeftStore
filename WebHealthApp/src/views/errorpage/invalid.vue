<template>
  <div class="wscn-container">
    <div class="wscn-invalid">
      <div class="pic-invalid" />
      <div class="bullshit">
        <div class="bullshit__oops">温馨提示</div>
        <div class="bullshit__headline">您访问的链接,无效参数</div>
        <div class="bullshit__info">如需帮助，请联系客服</div>
      </div>
    </div>
  </div>
</template>
<script>
import { removeToken } from '@/utils/auth'
export default {
  name: 'Index',
  components: {
  },
  data() {
    return {
      loading: false
    }
  },
  created() {
    removeToken()
  },
  methods: {

  }
}
</script>

<style lang="scss" scoped>
.wscn-container{

.bullshit{
  display: flex;
  flex-direction: column;
  align-items: center;
  padding-top: 60px;

 &__oops {
      font-size: 24px;
      font-weight: bold;
      line-height: 40px;
      color: #1482f0;
      margin-bottom: 10px;
    }
    &__headline {
      font-size: 20px;
      line-height: 24px;
      color: #222;
      font-weight: bold;
      margin-bottom: 10px;
    }
    &__info {
      font-size: 13px;
      line-height: 21px;
      color: grey;
      margin-bottom: 30px;
    }

}
}
</style>
