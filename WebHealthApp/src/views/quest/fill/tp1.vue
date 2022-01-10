<template>
  <div class="quest-fill-tp1">
    <div class="lm-header-big">
      <div class="bg-title">资料完善</div>
      <div class="sm-title">完善资料更准更好的为你服务</div>
    </div>
    <flow-form :questions="questions" />
  </div>
</template>
<script>

import FlowForm from '@/components/FlowForm/Index.vue'
export default {
  name: 'Example',
  components: {
    FlowForm
  },
  data() {
    return {
      questions: [
        {
          id: 'fullName',
          title: '您好，请输入你的真实姓名',
          type: 'input',
          value: ''
        },
        {
          id: 'birthday',
          title: '生日',
          type: 'date',
          value: ''
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
            '1': 'isTest',
            '2': 'isPregnant'
          },
          value: ''
        },
        {
          id: 'isTest',
          title: '近期困扰',
          type: 'radio',
          options: [
            { label: '睡眠', value: '1' },
            { label: '睡眠1', value: '2' },
            { label: '睡眠2', value: '3' },
            { label: '睡眠3', value: '4' }
          ],
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
          id: 'absds',
          title: '新得下一个问题',
          type: 'radio',
          options: [
            { label: 'A', value: '1' },
            { label: 'B', value: '2' },
            { label: 'C', value: '3' },
            { label: '宝D妈', value: '4' }
          ],
          value: ''
        }
      ]
    }
  },

  mounted() {

  },

  methods: {

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
