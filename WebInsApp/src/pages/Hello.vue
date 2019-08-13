<template>
  <div>
    <p>我是父级组件</p>
    <p>父级组件内容：{{ text.name }}</p>
    <!-- <p><button @click="onChange">改变内容</button></p> -->
    <hr>
    <child v-model="text.name"></child>
  </div>
</template>
<script>

export default {
  data() {
    return {
      text:{
        name:"'我是父级组件的内容'"
      }
    }
  },
  methods: {
    onChange() {
      this.text.name = '我是由父级组件触发改变了内容'
    }
  }
}
</script>
<style scoped>
</style>
