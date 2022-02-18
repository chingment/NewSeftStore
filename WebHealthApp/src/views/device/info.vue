<template>
  <div
    id="pg_device_info"
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
      <mt-cell title="姓名" is-link @click.native="onFieldEdit('fullName',deviceInfo.fullName)">
        <span>{{ deviceInfo.fullName }}</span>
      </mt-cell>
      <mt-cell title="性别" is-link @click.native="onFieldEdit('sex',deviceInfo.sex.value)">
        <span>{{ deviceInfo.sex.text }}</span>
      </mt-cell>
      <mt-cell title="生日" is-link @click.native="onFieldEdit('birthday',deviceInfo.birthday)">
        <span>{{ deviceInfo.birthday }}</span>
      </mt-cell>
      <mt-cell title="身高" is-link @click.native="onFieldEdit('height',deviceInfo.height)">
        <span style="margin-right:5px">{{ deviceInfo.height }}</span><span>cm</span>
      </mt-cell>
      <mt-cell title="体重" is-link @click.native="onFieldEdit('weight',deviceInfo.weight)">
        <span style="margin-right:5px">{{ deviceInfo.weight }}</span><span>kg</span>
      </mt-cell>
      <mt-cell title="睡眠困扰" is-link @click.native="onFieldEdit('perplex',deviceInfo.perplex.value)">
        <span>{{ deviceInfo.perplex.text }}</span>
      </mt-cell>
      <mt-cell title="亚健康困扰" is-link @click.native="onFieldEdit('subhealth',deviceInfo.subHealth.value)">
        <span>{{ deviceInfo.subHealth.text }}</span>
      </mt-cell>
      <mt-cell title="慢性困扰" is-link @click.native="onFieldEdit('chronicdisease',deviceInfo.chronicdisease.value)">
        <span>{{ deviceInfo.chronicdisease.text }}</span>
      </mt-cell>
      <mt-cell title="既往病史" is-link @click.native="onFieldEdit('medicalhis',deviceInfo.medicalhis.value)">
        <span>{{ deviceInfo.medicalhis.text }}</span>
      </mt-cell>
      <mt-cell title="服用何种药物" is-link @click.native="onFieldEdit('medicine',deviceInfo.medicine.value)">
        <span>{{ deviceInfo.medicine.text }}</span>
      </mt-cell>
    </div>
  </div>
</template>

<script>
import { initInfo } from '@/api/device'
export default {
  name: 'DeviceInfo',
  data() {
    return {
      loading: false,
      deviceId: '',
      deviceInfo: {
        deviceId: '',
        svUserId: '',
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
      initInfo({ deviceId: this.deviceId }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.deviceInfo = d.deviceInfo
          this.appInfo = d.appInfo
        }
        this.loading = false
      })
    },
    onFieldEdit(field, value) {
      sessionStorage.setItem('question_field', field)
      var question_value_type = typeof value
      if (question_value_type === 'object') {
        value = JSON.stringify(value)
      }
      sessionStorage.setItem('question_value_type', question_value_type)
      sessionStorage.setItem('question_value', value)

      this.$router.push({ path: '/device/infofield', query: {
        deviceId: this.deviceId,
        svUserId: this.deviceInfo.svUserId
      }})
    }
  }
}
</script>

<style lang="scss" scope>

#pg_device_info {
  padding: 20px;

  .mint-cell-title{
    display: flex;
    .mint-cell-text{
      width: 100px;
    }
  }
}

</style>
