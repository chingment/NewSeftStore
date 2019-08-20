<template>
  <div id="store_manage" class="app-container">
    <el-tabs v-model="activeName" type="card">
      <el-tab-pane label="基本信息" name="tabBaseInfo"> <manage-pane-base-info :store-id="id" /></el-tab-pane>
      <el-tab-pane label="商品管理" name="tabProductSkus"><manage-pane-product-skus :store-id="id" /></el-tab-pane>
      <el-tab-pane label="订单信息" name="tabOrders">角色管理</el-tab-pane>
    </el-tabs>
  </div>
</template>
<script>
import { getUrlParam } from '@/utils/commonUtil'
import managePaneBaseInfo from './components/ManagePaneBaseInfo'
import managePaneProductSkus from './components/ManagePaneProductSkus'
export default {
  components: { managePaneBaseInfo, managePaneProductSkus },
  data() {
    return {
      activeName: 'tabBaseInfo',
      id: ''
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      this.id = getUrlParam('id')
    },
    handleClick(tab, event) {
      console.log(tab, event)
    }
  }
}
</script>
