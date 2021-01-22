<template>
  <div id="log_manage" class="app-container">
    <el-tabs v-model="activeName" type="card">
      <el-tab-pane label="操作日志" name="tabOperate"> <pane-operate-log /></el-tab-pane>
      <el-tab-pane label="库存变化日志" name="tabStock"> <pane-stock-log /></el-tab-pane>
    </el-tabs>
  </div>
</template>
<script>

import paneOperateLog from './components/PaneOperateLog'
import paneStockLog from './components/PaneStockLog'
export default {
  name: 'SettingLogList',
  components: { paneOperateLog, paneStockLog },
  data() {
    return {
      activeName: 'tabOperate'
    }
  },
  created() {
  },
  methods: {
  }
}
</script>
<style lang="scss" scoped>

</style>
