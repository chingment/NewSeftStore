<template>
  <div
    id="device_info"
    class="own-info"
    style="display: flex;
    flex-direction: column;
    height: 100%;"
  >
    <div class="lm-header-big">
      <div class="bg-title">设备信息</div>
      <div class="sm-title">使用者的个人信息</div>
    </div>
    <div class="page-bodyer" style="flex:1;overflow: auto;">

      <mt-cell title="设备号">
        <span>{{ deviceInfo.deviceId }}</span>
      </mt-cell>

      <mt-cell title="姓名" is-link>
        <span>{{ deviceInfo.fullName }}</span>
      </mt-cell>
      <mt-cell title="性别" is-link>
        <span>{{ deviceInfo.sex.text }}</span>
      </mt-cell>
      <mt-cell title="生日" is-link>
        <span>{{ deviceInfo.birthday }}</span>
      </mt-cell>
      <mt-cell title="身高" is-link>
        <span>{{ deviceInfo.height }}</span>
      </mt-cell>
      <mt-cell title="体重" is-link>
        <span>{{ deviceInfo.weight }}</span>
      </mt-cell>
      <mt-cell title="睡眠困扰" is-link>
        <span>{{ deviceInfo.perplex.text }}</span>
      </mt-cell>
      <mt-cell title="亚健康困扰" is-link>
        <span>{{ deviceInfo.subHealth.text }}</span>
      </mt-cell>
      <mt-cell title="慢性困扰" is-link>
        <span>{{ deviceInfo.chronicdisease.text }}</span>
      </mt-cell>
      <mt-cell title="既往病史" is-link>
        <span>{{ deviceInfo.medicalhis.text }}</span>
      </mt-cell>
      <mt-cell title="服用何种药物" is-link>
        <span>{{ deviceInfo.medicine.text }}</span>
      </mt-cell>
    </div>
  </div>
</template>

<script>
import { deviceInfo } from '@/api/own'
export default {
  name: 'OwnDeviceInfo',
  data() {
    return {
      loading: false,
      deviceId: '',
      deviceInfo: {
        deviceId: '',
        fullName: '',
        age: 0,
        sex: { value: '', text: '' },
        birthday: '',
        height: '',
        weight: '',
        perplex: { value: '', text: '' },
        chronicdisease: { value: '', text: '' },
        medicalhis: { value: '', text: '' },
        medicine: { value: '', text: '' },
        subHealth: { value: '', text: '' }
      }
    }
  },
  created() {
    this.deviceId = this.$route.query.deviceId
    this.onInit()
  },
  methods: {
    onInit() {
      this.loading = true
      deviceInfo({ deviceId: this.deviceId }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.deviceInfo = d.deviceInfo
          this.appInfo = d.appInfo
        }
        this.loading = false
      })
    }
  }
}
</script>

<style lang="scss" scope>

#device_info {
  padding: 20px;
}

</style>
