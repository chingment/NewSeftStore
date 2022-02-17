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
      <mt-cell title="姓名" is-link @click.native="onEditField('fullName',deviceInfo.fullName)">
        <span>{{ deviceInfo.fullName }}</span>
      </mt-cell>
      <mt-cell title="性别" is-link @click.native="onEditField('sex',deviceInfo.sex.value)">
        <span>{{ deviceInfo.sex.text }}</span>
      </mt-cell>
      <mt-cell title="生日" is-link @click.native="onEditField('birthday',deviceInfo.birthday)">
        <span>{{ deviceInfo.birthday }}</span>
      </mt-cell>
      <mt-cell title="身高" is-link @click.native="onEditField('height',deviceInfo.height)">
        <span>{{ deviceInfo.height }}</span>
      </mt-cell>
      <mt-cell title="体重" is-link @click.native="onEditField('weight',deviceInfo.weight)">
        <span>{{ deviceInfo.weight }}</span>
      </mt-cell>
      <mt-cell title="睡眠困扰" is-link @click.native="onEditField('perplex',deviceInfo.perplex.value)">
        <span>{{ deviceInfo.perplex.text }}</span>
      </mt-cell>
      <mt-cell title="亚健康困扰" is-link @click.native="onEditField('subhealth',deviceInfo.subHealth.value)">
        <span>{{ deviceInfo.subHealth.text }}</span>
      </mt-cell>
      <mt-cell title="慢性困扰" is-link @click.native="onEditField('chronicdisease',deviceInfo.chronicdisease.value)">
        <span>{{ deviceInfo.chronicdisease.text }}</span>
      </mt-cell>
      <mt-cell title="既往病史" is-link @click.native="onEditField('medicalhis',deviceInfo.medicalhis.value)">
        <span>{{ deviceInfo.medicalhis.text }}</span>
      </mt-cell>
      <mt-cell title="服用何种药物" is-link @click.native="onEditField('medicine',deviceInfo.medicine.value)">
        <span>{{ deviceInfo.medicine.text }}</span>
      </mt-cell>
    </div>

    <mt-popup
      v-model="popupEditFieldVisible"
      position="right"
      popup-transition="popup-fade"
      style="height:100%;width:100%"
    >
      <div style="padding:20px">
        <div class="lm-header-big">
          <div class="bg-title">资料完善</div>
          <div class="sm-title">完善资料更准更好的为你服务</div>
        </div>

        <flow-form :questions="question" @on-complete="onQuestionComplete" />

      </div>
    </mt-popup>
  </div>
</template>

<script>
import { initInfo, infoEdit } from '@/api/device'
import FlowForm from '@/components/FlowForm/Index.vue'
import fromReg from '@/utils/formReg'
export default {
  name: 'OwnDeviceInfo',
  components: {
    FlowForm
  },
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
      },
      popupEditFieldVisible: false,
      question: [],
      questions: [
        {
          id: 'fullName',
          title: '您好，请输入你的真实姓名',
          type: 'input',
          value: '',
          append: '',
          rule: {
            required: true, min: 1, max: 20, message: '必填,且不能超过20个字符'
          }
        },
        {
          id: 'birthday',
          title: '生日',
          type: 'date',
          value: '2012-12-12',
          append: '',
          rule: {
            required: true, min: 1, max: 20, message: '必选,且必须是日期格式', pattern: fromReg.date
          }
        },
        {
          id: 'height',
          title: '身高',
          type: 'input',
          value: '',
          append: 'cm',
          rule: {
            required: true, min: 1, max: 20, message: '必填，必须是数字格式', pattern: fromReg.decimal
          }
        },
        {
          id: 'weight',
          title: '体重',
          type: 'input',
          value: '',
          append: 'kg',
          rule: {
            required: true, min: 1, max: 20, message: '必填，必须是数字格式', pattern: fromReg.decimal
          }
        },
        {
          id: 'sex',
          title: '性别',
          type: 'radio',
          options: [
            { label: '男', value: '1' },
            { label: '女', value: '2' }
          ],
          value: ''
        },
        {
          id: 'ladyidentity',
          title: '是否已怀孕',
          type: 'radio',
          options: [
            { label: '暂无孕产计划', value: '1' },
            { label: '备孕', value: '2' },
            { label: '孕妈', value: '3' },
            { label: '宝妈', value: '4' }
          ],
          value: ''
        },
        {
          id: 'geyweek',
          title: '孕周',
          type: 'gesweek',
          value: [7, 3],
          append: ''
        },
        {
          id: 'deliveryTime',
          title: '预产期',
          type: 'date',
          value: '',
          append: '',
          rule: {
            required: true, min: 1, max: 20, message: '必选,且必须是日期格式', pattern: fromReg.date
          }
        },
        {
          id: 'perplex',
          title: '睡眠困扰',
          type: 'checklist',
          options: [
            { label: '打鼾', value: '3' },
            { label: '失眠', value: '81' },
            { label: '易醒', value: '82' },
            { label: '难以入睡', value: '83' },
            { label: '呼吸暂停综合症', value: '2' }
          ],
          required: true,
          value: []
        },
        {
          id: 'subhealth',
          title: '亚健康困扰',
          type: 'checklist',
          options: [
            { label: '疲乏无力', value: '1' },
            { label: '情绪波动', value: '2' },
            { label: '精力不足', value: '3' },
            { label: '怕冷怕冷', value: '4' },
            { label: '头昏头痛', value: '5' },
            { label: '易于感冒', value: '6' },
            { label: '记忆力下降', value: '7' },
            { label: '胸闷', value: '8' },
            { label: '肠胃问题', value: '9' }
          ],
          required: true,
          value: []
        },
        {
          id: 'chronicdisease',
          title: '慢性困扰',
          type: 'checklist',
          options: [
            { label: '糖尿病', value: '4' },
            { label: '高血压', value: '5' },
            { label: '冠心病', value: '6' }
          ],
          required: true,
          value: []
        },
        {
          id: 'medicalhis',
          title: '既往病史',
          type: 'checklist',
          options: [
            { label: '重大手术史', value: '1' },
            { label: '输血史（非献血）', value: '2' },
            { label: '传染病史', value: '3' }
          ],
          required: true,
          value: []
        },
        {
          id: 'medicine',
          title: '服用药物状况',
          type: 'checklist',
          options: [
            { label: '高血压药物', value: '1' },
            { label: '心脏病药物', value: '2' },
            { label: '糖尿病药物', value: '3' },
            { label: '脑梗塞药物', value: '4' },
            { label: '治疗失眠药物', value: '5' }
          ],
          required: true,
          value: []
        }
      ]
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
    onEditField(field, value) {
      this.popupEditFieldVisible = true
      var questions = this.questions

      for (let i = 0; i < questions.length; i++) {
        if (questions[i].id === field) {
          questions[i].value = value
          this.question = [questions[i]]
        }
      }
    },
    onQuestionComplete() {
      var answers = {}

      answers[this.question[0].id] = this.question[0].value

      this.loading = true
      infoEdit({ deviceId: this.deviceId, svUserId: this.deviceInfo.svUserId, answers: answers }).then(res => {
        if (res.result === 1) {
          this.$toast(res.message)
          this.popupEditFieldVisible = false
          this.onInit()
        } else {
          this.$toast(res.message)
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

  .mint-cell-title{
    display: flex;
    .mint-cell-text{
      width: 100px;
    }
  }
}

</style>
