<template>
  <div id="pg_device_info">
    <div class="step-1">
      <div class="lm-header-big">
        <div class="bg-title">设备解绑</div>
        <div class="sm-title">以下是您已绑定的设备</div>
      </div>

      <div v-for="(item, index) in devices" :key="index" class="device">
        <div class="form">
          <mt-cell title="设备号">
            <span>{{ item.id }}</span>
          </mt-cell>
          <mt-cell title="使用者">
            <span>{{ item.userSignName }}</span>
          </mt-cell>
          <mt-cell title="绑定状态">
            <span>{{ item.bindStatus.text }}</span>
          </mt-cell>
          <mt-cell v-if="item.bindStatus.value==1" title="绑定时间">
            <span>{{ item.bindTime }}</span>
          </mt-cell>
          <mt-cell v-if="item.bindStatus.value==2" title="解绑时间">
            <span>{{ item.unBindTime }}</span>
          </mt-cell>
        </div>
        <mt-button v-if="item.bindStatus.value==1" class="btn-unbind" type="primary" @click="onUnBind(item)">解绑</mt-button>
      </div>
    </div>

  </div>

</template>
<script>
import { initInfo, unBind } from '@/api/device'
export default {
  name: 'App',
  components: {
  },
  data() {
    return {
      loading: false,
      appInfo: {},
      devices: []
    }
  },
  created() {
    this.onInit()
  },
  methods: {
    onInit() {
      this.loading = true
      initInfo({ }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.appInfo = d.appInfo
          this.devices = d.devices
        }
        this.loading = false
      })
    },
    onUnBind(item) {
      this.$messagebox.confirm('确定要解绑设备?').then(action => {
        this.loading = true
        unBind({ deviceId: item.id }).then(res => {
          this.$toast(res.message)
          if (res.result === 1) {
            this.onInit()
          }
          this.loading = false
        })
      })
    }
  }
}
</script>

<style lang="scss" scope>

#pg_device_info{
    padding: 20px;

.form{
    padding: 50px 0px;
}

    .btn-unbind{
        width: 100%;
    }
}

</style>
