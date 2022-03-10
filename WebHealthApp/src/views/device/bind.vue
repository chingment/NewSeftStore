<template>
  <div id="pg_device_bind">
    <div v-show="step===1" class="step-1">
      <div class="lm-header-big">
        <div class="bg-title">设备绑定</div>
        <div class="sm-title">点击扫一扫，或输入S/N号进行绑定</div>
      </div>
      <div class="form">
        <mt-cell title="使用者" :value="userInfo.nickName" />
        <mt-field v-model.trim="formByBind.deviceId" label="设备号" placeholder="请输入S/N号">
          <img src="@/assets/images/icon_scan_code.png" height="32px" width="32px" @click="onScanQrCode">
        </mt-field>

      </div>
      <div class="btn-scan primary-color-font" @click="onScanQrCode">点击扫一扫</div>
      <mt-button class="btn-binddevice" type="primary" @click="onSaveStep1">绑定</mt-button>

      {{ tips }}
    </div>
    <div v-show="step===2" class="step-2">
      <div class="lm-header-big">
        <div class="bg-title">手机绑定</div>
        <div class="sm-title">为了更好的提供相关资讯服务</div>
      </div>
      <div class="form">
        <mt-cell title="设备号">
          <span style="margin-right:20px">{{ formByBind.deviceId }}</span>
          <span class="primary-color-font" @click="onChangeDeviceId">更改</span>
        </mt-cell>
        <mt-cell title="使用者" :value="userInfo.nickName" />
        <mt-field v-model.trim="formByBind.phoneNumber" type="tel" label="手机号" placeholder="请输入手机号码" />
        <mt-field v-model.trim="formByBind.validCode" type="number" label="验证码" placeholder="请输入手机验证码">
          <mt-button type="primary" size="small" plain :disabled="pvcDisabled" @click="onGetPhoneValidCode">{{ pvcText }}</mt-button>
        </mt-field>
      </div>
      <mt-button class="btn-bindphone" type="primary" @click="onSaveStep2">绑定</mt-button>
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
        <img class="img-pa" :src="appInfo.paQrCode" alt="" @click="onPaImg">
        <div class="tip">请用手长按图片关注公众号</div>
      </div>
    </mt-popup>
  </div>

</template>
<script>
import { initBind, bindSerialNo, bindPhoneNumber, getPhoneValidCode } from '@/api/device'
import wx from 'weixin-js-sdk'
import { isEmpty } from '@/utils/commonUtil'
export default {
  name: 'DeviceBind',
  components: {
  },
  data() {
    return {
      loading: false,
      userInfo: {
        nickName: ''
      },
      appInfo: {
        wxQrCode: '',
        appName: ''
      },
      formByBind: {
        deviceId: '',
        phoneNumber: '',
        validCode: '',
        tokenCode: ''
      },
      step: 1,
      pvcDisabled: false,
      pvcCount: '',
      pvcTimer: null,
      pvcText: '获取',
      popupVisibleByPaQrCode: false,
      tips: ''
    }
  },
  created() {
    console.log('created')
    this.onInit()
  },
  methods: {
    onInit() {
      this.formByBind.deviceId = this.$route.query.deviceId
      this.loading = true
      initBind({ deviceId: this.formByBind.deviceId, requestUrl: encodeURIComponent(location.href.split('#')[0]) }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.userInfo = d.userInfo
          this.appInfo = d.appInfo
          document.title = d.appInfo.appName
          if (d.step === 1 || d.step === 2) {
            this.step = d.step
          } else if (d.step === 3) {
            this.$router.push({ path: '/device/fill', query: {
              deviceId: this.formByBind.deviceId
            }})
          } else if (d.step === 4) {
            this.$router.push('/device/manage')
          }
          var openJsSdk = d.openJsSdk
          wx.config({
            debug: false, // 开启调试模式,
            appId: openJsSdk.appId, // 必填，企业号的唯一标识，此处填写企业号corpid
            timestamp: openJsSdk.timestamp, // 必填，生成签名的时间戳
            nonceStr: openJsSdk.nonceStr, // 必填，生成签名的随机串
            signature: openJsSdk.signature, // 必填，签名，见附录1
            jsApiList: ['scanQRCode']
          })
        }
        this.loading = false
      })
    },
    onSaveStep1() {
      this.loading = true

      if (isEmpty(this.formByBind.deviceId)) {
        this.$toast('请输入设备号')
        return
      }

      bindSerialNo({ deviceId: this.formByBind.deviceId }).then(res => {
        if (res.result === 1) {
          this.step = 2
        } else {
          if (res.code === 2801) {
            this.popupVisibleByPaQrCode = true
          } else {
            this.$toast(res.message)
          }
        }
        this.loading = false
      })
    },
    onPaImg() {
      this.popupVisibleByPaQrCode = false
      this.step = 2
    },
    onSaveStep2() {
      this.loading = true

      var re1 = /^1\d{10}$/
      if (isEmpty(this.formByBind.phoneNumber)) {
        this.$toast('请输入手机号码')
        return
      } else {
        if (!re1.test(this.formByBind.phoneNumber)) {
          this.$toast('手机格式错误，请输入11位数字')
          return
        }
      }

      if (isEmpty(this.formByBind.validCode)) {
        this.$toast('请输入验证码')
        return
      }

      bindPhoneNumber(this.formByBind).then(res => {
        if (res.result === 1) {
          this.$router.push({ path: '/device/fill', query: {
            deviceId: this.formByBind.deviceId
          }})
        } else {
          this.$toast(res.message)
        }
        this.loading = false
      })
    },
    onScanQrCode() {
      var _this = this
      wx.scanQRCode({
        needResult: 1,
        scanType: ['qrCode', 'barCode'],
        desc: 'scanQRCode desc',
        success: function(res) {
          if (res.errMsg === 'scanQRCode:ok') {
            var result = res.resultStr
            if (result.indexOf('http://') > -1 || result.indexOf('https://') > -1) {
              window.location.href = result
            } else {
              var code = result.split(',')
              // _this.tips = JSON.stringify(code)
              _this.formByBind.deviceId = code[1]
            }
          }
        }
      })
    },
    onChangeDeviceId() {
      this.step = 1
    },
    onGetPhoneValidCode() {
      // const TIME_COUNT = 120
      // if (!this.pvcTimer) {
      //   this.pvcCount = TIME_COUNT
      //   this.pvcDisabled = true
      //   this.pvcTimer = setInterval(() => {
      //     if (this.pvcCount > 0 && this.pvcCount <= TIME_COUNT) {
      //       this.pvcCount--
      //       this.pvcText = this.pvcCount + 'S'
      //     } else {
      //       this.pvcDisabled = false
      //       this.pvcText = '获取'
      //       clearInterval(this.pvcTimer)
      //       this.pvcTimer = null
      //     }
      //   }, 1000)
      // }

      var re1 = /^1\d{10}$/
      if (isEmpty(this.formByBind.phoneNumber)) {
        this.$toast('请输入手机号码')
        return
      } else {
        if (!re1.test(this.formByBind.phoneNumber)) {
          this.$toast('手机格式错误，请输入11位数字')
          return
        }
      }

      this.loading = true
      getPhoneValidCode({ deviceId: this.formByBind.deviceId, phoneNumber: this.formByBind.phoneNumber }).then(res => {
        this.$toast(res.message)
        if (res.result === 1) {
          var d = res.data
          this.formByBind.tokenCode = d.tokenCode
          const TIME_COUNT = 120
          if (!this.pvcTimer) {
            this.pvcCount = TIME_COUNT
            this.pvcDisabled = true
            this.pvcTimer = setInterval(() => {
              if (this.pvcCount > 0 && this.pvcCount <= TIME_COUNT) {
                this.pvcCount--
                this.pvcText = this.pvcCount + 'S'
              } else {
                this.pvcDisabled = false
                this.pvcText = '获取'
                clearInterval(this.pvcTimer)
                this.pvcTimer = null
              }
            }, 1000)
          }
        }
        this.loading = false
      })
    }
  }
}
</script>

<style lang="scss" scope>

#pg_device_bind {
  padding: 20px;

  .form {
    padding: 50px 0 10px 0;
  }

  .btn-scan {
    padding: 20px;

    text-align: center;
  }

  .btn-binddevice,
  .btn-bindphone {
    width: 100%;
    margin-top: 50px;
  }
}

.popup-toolbar {
  height: 40px;

  border-bottom-width: 1px;
  border-bottom-style: solid;
  border-bottom-color: rgb(234, 234, 234);

  .close {
    position: absolute;
    top: 2px;
    right: 0;

    width: 2em;
    height: 2rem;
  }
}

.pa-box {
  text-align: center;

  .img-pa {
    width: 300px;
    height: 300px;
  }

  .tip {
    padding: 30px;
  }
}

</style>
