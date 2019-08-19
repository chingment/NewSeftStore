<template>
  <div id="store_container" class="app-container">
    <el-tabs v-model="activeName" type="card">
      <el-tab-pane label="基本信息" name="tabBaseInfo"> <manage-pane-base-info /></el-tab-pane>
      <el-tab-pane label="商品管理" name="tabProductSkus"><manage-pane-product-skus :store-id="storeId" /></el-tab-pane>
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
      storeId: ''
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      this.storeId = getUrlParam('storeId')
    },
    handleClick(tab, event) {
      console.log(tab, event)
    }
  }
}
</script>
