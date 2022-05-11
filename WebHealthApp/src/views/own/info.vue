<template>
  <div id="own_info">
    <div class="info-card">
      <div class="lf">
        <img class="avatar" :src="userInfo.avatar" alt="">
      </div>
      <div class="md">
        <div class="sign-name">
          {{ userInfo.signName }}
        </div>
      </div>
      <div class="rf">

        <img class="qrcode" src="@/assets/images/my_qrcode.png" alt="" style="display:none" @click="onMyQrcode">

      </div>
    </div>

    <div class="info-nav">
      <mt-cell v-for="(item, index) in devices" :key="index" :title="'设备（'+item.id+'）'" is-link @click.native="onDeviceInfo(item)">
        <span style="color: green">{{ item.signName }}</span>
      </mt-cell>
      <mt-cell title="紧急联系人" is-link @click.native="onEgyContacts()" />
    </div>
  </div>

</template>

<script>
import { info } from '@/api/own'
export default {
  name: 'OwnInfo',
  data() {
    return {
      loading: false,
      userInfo: {
        avatar: '',
        signName: ''
      },
      devices: [
      ],
      appInfo: {}
    }
  },
  created() {
    this.onInit()
  },
  methods: {
    onInit() {
      this.loading = true
      info({ }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.userInfo = d.userInfo
          this.devices = d.devices
          this.appInfo = d.appInfo
        }
        this.loading = false
      })
    },
    onDeviceInfo(item) {
      this.$router.push({ path: '/device/info', query: {
        deviceId: item.id
      }})
    },
    onEgyContacts() {
      this.$router.push('/own/egycontacts')
    },
    onMyQrcode() {
      this.$router.push('/own/qrcode')
    }
  }
}
</script>

<style lang="scss" scope>
@import '@/styles/variables.scss';

.info-card {
  display: flex;

  height: 140px;
  padding: 20px;

  color: #fff;
  background: $primaryColor;

  .avatar {
    width: 60px;
    height: 60px;

    border-radius: 50%;
  }
  .lf{
    width: 70px;
  }
  .md {
    flex: 1;
    padding: 10px;

    .sign-name {
      font-size: 16px;
      font-weight: bold;
    }
  }

  .rf{
     width: 30px;
     .qrcode{
       width: 30px;
       height: 30px;
     }
  }
}

.info-nav {
  padding: 10px;
}

</style>
