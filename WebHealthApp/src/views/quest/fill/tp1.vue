<template>
  <div class="quest-fill-tp1">
    <div class="lm-header-big">
      <div class="bg-title">资料完善</div>
      <div class="sm-title">完善资料更准更好的为你服务</div>
    </div>

    <div v-show="step===1" class="form-devices">
      <div v-for="(item, index) in devices" :key="index" class="device">
        <div class="form">
          <mt-cell title="设备号">
            <span>{{ item.id }}</span>
          </mt-cell>
          <mt-cell title="使用者">
            <span>{{ item.userName }}</span>
          </mt-cell>
          <mt-cell title="绑定状态">
            <span>{{ item.bindStatus.text }}</span>
          </mt-cell>
        </div>
        <mt-button class="btn-go-fill" type="primary" @click="onGoFill(item)">进入</mt-button>
      </div>
    </div>

    <div v-show="step===2">
      <flow-form :questions="questions" @on-complete="onQuestionsComplete" />
    </div>

    <div v-show="step===3" class="form-welcomeout">
      <div class="t1">绑定成功</div>
      <div class="t2">您好，欢迎使用！</div>
    </div>
  </div>
</template>
<script>

import { initFill, fill } from '@/api/quest'

import FlowForm from '@/components/FlowForm/Index.vue'
export default {
  name: 'Example',
  components: {
    FlowForm
  },
  data() {
    return {
      deviceId: null,
      devices: null,
      appInfo: {},
      step: 0,
      questions: [
        {
          id: 'fullName',
          title: '您好，请输入你的真实姓名',
          type: 'input',
          value: '',
          append: ''
        },
        {
          id: 'birthday',
          title: '生日',
          type: 'date',
          value: '',
          append: ''

        },
        {
          id: 'height',
          title: '身高',
          type: 'input',
          value: '',
          append: 'cm'
        },
        {
          id: 'weight',
          title: '体重',
          type: 'input',
          value: '',
          append: 'kg'
        },
        {
          id: 'sex',
          title: '性别',
          type: 'radio',
          options: [
            { label: '男', value: '1' },
            { label: '女', value: '2' }
          ],
          jump: {
            '1': 'perplexs',
            '2': 'isPregnant'
          },
          value: ''
        },
        {
          id: 'isPregnant',
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
          id: 'perplexs',
          title: '睡眠困扰',
          type: 'checklist',
          options: [
            { label: '打鼾', value: '1' },
            { label: '失眠', value: '2' },
            { label: '易醒', value: '3' },
            { label: '难以入睡', value: '4' },
            { label: '呼吸暂停综合症', value: '5' }
          ],
          required: false,
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
            { label: '易于干嘛', value: '6' },
            { label: '记忆力下降', value: '7' },
            { label: '胸闷', value: '8' },
            { label: '肠胃问题', value: '9' }
          ],
          required: false,
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
          required: false,
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
          required: false,
          value: []
        },
        {
          id: 'medicine',
          title: '服用何种药物',
          type: 'checklist',
          options: [
            { label: '高血压药物', value: '1' },
            { label: '心脏病药物', value: '2' },
            { label: '糖尿病药物', value: '3' },
            { label: '脑梗塞药物', value: '4' },
            { label: '治疗失眠药物', value: '5' }
          ],
          required: false,
          value: []
        }
      ]
    }
  },
  created() {
    this.deviceId = typeof this.$route.query.deviceId === 'undefined' ? null : this.$route.query.deviceId
    console.log(this.deviceId)
    // this.onInit()
    this.step = 2
  },
  methods: {
    onInit() {
      this.loading = true
      initFill({}).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.devices = d.devices
          var _deviceId = this.deviceId
          if (_deviceId == null) {
            if (this.devices.length === 1) {
              var device = this.devices[0]
              this.deviceId = device.id
              if (device.bindStatus.value === 1) {
                this.step = 2
              } else {
                this.step = 3
              }
            } else {
              this.step = 1
            }
          } else {
            var device2 = this.devices.filter(function(item) {
              return item.id === _deviceId
            })[0]

            if (device2.bindStatus.value === 1) {
              this.step = 2
            } else {
              this.step = 3
            }
          }
        }
        this.loading = false
      })
    },
    onGoFill(item) {
      this.deviceId = item.id
      this.step = 2
    },
    onQuestionsComplete() {
      console.log('onQuestionsComplete')
      this.loading = true
      var answers = {}

      this.questions.forEach(item => {
        answers[item.id] = item.value
      })

      fill({ deviceId: this.deviceId, answers: answers }).then(res => {
        if (res.result === 1) {
          this.step = 3
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

.quest-fill-tp1 {
  height: 100%;
  padding: 20px;

  .btn-go-fill {
    width: 100%;
    margin: 30px 0;
  }

  .form-welcomeout {
    margin-top: 60px;

    text-align: center;

    .t1 {
      margin: 20px;

      color: #4caf50;
    }

    .t2 {
      font-size: 28px;

      color: #409eff;
    }
  }
}

</style>
