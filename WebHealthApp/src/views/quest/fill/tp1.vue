<template>
  <div class="quest-fill-tp1">
    <div class="lm-header-big">
      <div class="bg-title">资料完善</div>
      <div class="sm-title">完善资料更准更好的为你服务</div>
    </div>
    <flow-form :questions="questions" />

    <mt-button class="btn-scan" type="primary" @click="onSave">保存</mt-button>
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
      appInfo: {},
      questions: [
        {
          id: 'fullName',
          title: '您好，请输入你的真实姓名',
          type: 'input',
          value: ''
        },
        // {
        //   id: 'birthday',
        //   title: '生日',
        //   type: 'date',
        //   value: ''
        // },
        // {
        //   id: 'height',
        //   title: '身高',
        //   type: 'input',
        //   value: ''
        // },
        // {
        //   id: 'weight',
        //   title: '体重',
        //   type: 'input',
        //   value: ''
        // },
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
          value: []
        },
        {
          id: 'chronicdisease',
          title: '慢性困扰',
          type: 'checklist',
          options: [
            { label: '高血压', value: '1' },
            { label: '糖尿病', value: '2' },
            { label: '冠心病', value: '3' }
          ],
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
          value: []
        }
      ]
    }
  },
  created() {
    this.onInit()
  },
  methods: {
    onInit() {
      this.loading = true
      initFill({}).then(res => {
        if (res.result === 1) {
          var d = res.data
        }
        this.loading = false
      })
    },
    onSave() {
      this.loading = true
      fill({}).then(res => {
        if (res.result === 1) {

        }
        this.loading = false
      })
    }
  }
}
</script>
<style lang="scss" scope>

.quest-fill-tp1{
  background-color: #f6f6f8;
  height: 100%;
   padding: 20px;
}

</style>
