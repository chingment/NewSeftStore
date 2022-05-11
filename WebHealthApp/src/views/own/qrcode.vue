<template>
  <div id="pg_egycontactedit">
    <div class="step-1">
      <div class="lm-header-big">
        <div class="bg-title">我的二维码</div>
        <div class="sm-title">长按下面的二维码发送给好友，邀请对方关注，关注后将同时接收到您的报告</div>
      </div>

      <div style="margin:20px 0px">

        <div id="QRCodeNone" style="display:none" />
        <div id="qrcode" style="text-align: center;" />
        <span />
      </div>

      <mt-button style="width: 100%;" type="default" @click="onGoBack">返回</mt-button>

    </div>

  </div>

</template>
<script>

import QRCode from 'qrcodejs2'

export default {
  name: 'EgyContacts',
  components: {
  },
  data() {
    return {
      loading: false,
      appInfo: {}
    }
  },
  created() {
    this.onInit()
  },
  methods: {
    onInit() {
      this.$nextTick(() => {
        var qrcode = new QRCode(document.getElementById('QRCodeNone'), {
          width: 150,
          height: 150,
          text: 'dsdadadadads', // 二维码地址
          colorDark: '#000',
          colorLight: '#fff'
        })

        const myCanvas = document.getElementsByTagName('canvas')[0]
        const img = this.convertCanvasToImage(myCanvas)
        const code = document.getElementById('qrcode')
        code.appendChild(img)
      })
    },
    convertCanvasToImage(canvas) {
      var image = new Image()
      image.src = canvas.toDataURL('image/png')
      return image
    },
    onGoBack() {
      this.$router.go(-1)
    }
  }
}
</script>

<style lang="scss" scope>

#pg_egycontactedit {
  padding: 20px;

  .list-field {
    padding: 50px 0;
  }

}

</style>
