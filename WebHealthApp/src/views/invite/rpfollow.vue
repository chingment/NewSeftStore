<template>
  <div id="pg_invite_follow">
    <div class="lm-header-big">
      <div class="bg-title">邀请关注</div>
      <div class="sm-title">同意关注后，可同步接收邀请人的健康报告</div>
    </div>
    <div class="lm-body">
      <div v-if="step==1">
        <div class="inviter">
          <img class="avatar" :src="inviter.avatar" alt="">
          <span class="nickname">{{ inviter.nickName }}</span>
        </div>
        <mt-button class="btn-agree" type="primary" @click="onAgree">同意</mt-button>
      </div>
      <div v-else-if="step==2">
        <div class="tip-box">
          <img class="icon" :src="require('@/assets/images/ic_success.png')" alt="">
          <span class="tip">关注成功</span>
        </div>
      </div>
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
import { initRpFollow, agreeRpFollow } from '@/api/invite'

export default {
  name: 'InviteRpFollow',
  components: {
  },
  data() {
    return {
      loading: false,
      step: 1,
      inviter: {
        avatar: '',
        nickName: ''
      },
      appInfo: {
        wxQrCode: '',
        appName: ''
      },
      popupVisibleByPaQrCode: false,
      form: {
        merchId: '',
        ts: '',
        iv_uid: ''
      }
    }
  },
  created() {
    this.form.merchId = this.$route.query.merchId
    this.form.ts = this.$route.query.ts
    this.form.iv_uid = this.$route.query.iv_uid
    this.onInit()
  },
  methods: {
    onInit() {
      this.loading = true
      initRpFollow(this.form).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.step = d.step
          this.inviter = d.inviter
          this.appInfo = d.appInfo
        }
        this.loading = false
      })
    },
    onAgree() {
      this.$messagebox.confirm('确定要关注?').then(action => {
        this.loading = true
        agreeRpFollow(this.form).then(res => {
          this.$toast(res.message)
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
      })
    },
    onPaImg() {
      this.popupVisibleByPaQrCode = false
    }
  }
}
</script>

<style lang="scss" scope>

#pg_invite_follow {
  padding: 20px;

  .data-list {
    padding: 20px 0;
  }

  .inviter {
    display: flex;
    align-items: center;
    flex-direction: column;

    padding: 50px;

    .avatar {
      width: 100px;
      height: 100px;
    }

    .nickname {
      color: #000;
    }
  }

  .btn-agree {
    width: 100%;
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
}

</style>
