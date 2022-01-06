<template>
  <div id="pg_device_bind">
    <div v-show="step===1" class="step-1">
      <div class="lm-header-big">
        <div class="bg-title">设备绑定</div>
        <div class="sm-title">点击扫一扫，或输入S/N号进行绑定</div>
      </div>
      <div class="form">
        <mt-field label="" placeholder="请输入S/N号">
          <img src="@/assets/images/icon_scan_code.png" height="32px" width="32px">
        </mt-field>
      </div>

      <mt-button class="btn-scan" type="primary" @click="onSaveStep1">绑定</mt-button>
    </div>
    <div v-show="step===2" class="step-2">
      <div class="lm-header-big">
        <div class="bg-title">手机绑定</div>
        <div class="sm-title">为了更好的提供相关资讯服务</div>
      </div>

      <div class="form">
        <mt-cell title="设备号" value="SN32244DD55" />
        <mt-field label="手机号" placeholder="请输入手机号码" />
        <mt-field label="验证码" placeholder="请输入手机验证码">
          <mt-button type="primary" size="small" plain>获取</mt-button>
        </mt-field>
      </div>

      <mt-button class="btn-scan" type="primary" @click="onSaveStep2">绑定</mt-button>

    </div>

    <mt-popup
      v-model="popupVisibleByPaQrCode"
      position="bottom"
      style="width:100%"
    >
      <div class="popup-toolbar">
        <svg-icon icon-class="close" class="close" @click="popupVisibleByPaQrCode=false" />
      </div>

      <div class="pa-box">
        <img class="img-pa" src="@/assets/test/tt.jpg" alt="" @click="onPaImg">
        <div class="tip">请用手长按图片关注公众号</div>
      </div>

    </mt-popup>
  </div>

</template>
<script>
export default {
  name: 'App',
  components: {
  },
  data() {
    return {
      loading: false,
      step: 1,
      popupVisibleByPaQrCode: false
    }
  },
  methods: {
    onSaveStep1() {
      this.popupVisibleByPaQrCode = true
    },
    onPaImg() {
      this.popupVisibleByPaQrCode = false
      this.step = 2
    },
    onSaveStep2() {
      this.$router.push('/quest/fill/tp1')
    }
  }
}
</script>

<style lang="scss" scope>

#pg_device_bind{
    padding: 20px;

.form{
    padding: 50px 0px;
}

    .btn-scan{
        width: 100%;
    }
}

.popup-toolbar{
    border-bottom-width: 1px;
    border-bottom-style: solid;
    border-bottom-color: rgb(234, 234, 234);
    height: 40px;
    .close{
    width: 2em;
    height: 2rem;
    right: 0;
    position: absolute;
    top: 2px;
    }
}

.pa-box{
  text-align: center;

  .img-pa{
    height: 300px;
    width: 300px;
  }

  .tip{
    padding: 30px;
  }
}
</style>
