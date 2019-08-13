<template>
  <div>
    <p>我是子组件</p>
    <p>子组件内容：{{ myValue }}</p>
    <p><button @click="onChange">改变内容</button></p>
  </div>
</template>
<script>
export default {
  name:"child",
  props: {
    //此处一定要用value
    value: {
      type: String
    }
  },
  data() {
    return {
      myValue: this.value
    }
  },
  methods: {
    onChange() {
      this.myValue = '我是由子组件触发改变了内容'
    }
  },
  watch: {
    //监听prop传的value，如果父级有变化了，将子组件的myValue也跟着变，达到父变子变的效果
    value(newVal) {
      this.myValue = newVal
    },
    //监听myValue，如果子组件中的内容变化了，通知父级组件，将新的值告诉父级组件，我更新了，父级组件接受到值后页就跟着变了
    //参考官网：https://cn.vuejs.org/v2/guide/components-custom-events.html#%E8%87%AA%E5%AE%9A%E4%B9%89%E7%BB%84%E4%BB%B6%E7%9A%84-v-model
    myValue(newVal) {
      this.$emit('input', newVal)
    }
  }
}
</script>
<style scoped>
</style>
