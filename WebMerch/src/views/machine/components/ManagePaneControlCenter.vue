<template>
  <div id="machine_controlcenter" v-loading="loading" class="app-container">
    <div class="pane-ctl">
      <div class="title"><span>系统相关</span>
      </div>
      <div class="content">

        <el-button type="primary">重启系统</el-button>
        <el-button type="primary">设置状态</el-button>
      </div>
    </div>
  </div>
</template>
<script>

export default {
  name: 'ManagePaneBaseInfo',
  data() {
    return {
    }
  },
  watch: {
    '$route'(to, from) {
      this.init()
    }
  },
  mounted() {

  },
  created() {
    this.init()
  },
  methods: {
    init() {

    }
  }
}
</script>
<style lang="scss" scoped>

#machine_controlcenter{

  .pane-ctl {

    .title{
    padding: 10px 8px;
    background-color: #ecf8ff;
    border-radius: 4px;
    border-left: 5px solid #50bfff;
   }

   .content{
    padding: 20px 8px;
   }

  }

}
</style>
