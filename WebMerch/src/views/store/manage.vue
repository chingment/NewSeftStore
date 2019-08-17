<template>
  <div id="store_container" class="app-container">
    <el-tabs v-model="activeName" type="card" @tab-click="handleClick">
      <el-tab-pane label="基本信息" name="tabEdit">  <component :is="tabEdit" /></el-tab-pane>
      <el-tab-pane label="商品管理" name="tabProductSku"><component :is="tabProductSkuList" /></el-tab-pane>
      <el-tab-pane label="订单信息" name="third">角色管理</el-tab-pane>
    </el-tabs>
  </div>
</template>
<script>
import tabEdit from './components/Edit'
import tabProductSkuList from './components/ProductSkuList'
export default {
  components: { tabEdit, tabProductSkuList },
  data() {
    return {
      activeName: 'tabEdit',
      tabEdit: 'tabEdit',
      tabProductSkuList: 'tabProductSkuList'
    }
  },
  methods: {
    handleClick(tab, event) {
      console.log(tab, event)
    }
  }
}
</script>
