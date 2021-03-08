<template>
  <div id="product_manage">
    <page-header />
    <el-tabs v-model="activeName" type="card">
      <el-tab-pane label="基本信息" name="tabBaseInfo"> <manage-pane-base-info :spu-id="id" /></el-tab-pane>
      <el-tab-pane label="在售店铺" name="tabStoreSale"><manage-pane-store-sale :spu-id="id" /></el-tab-pane>
    </el-tabs>
  </div>
</template>
<script>
import { getUrlParam } from '@/utils/commonUtil'
import managePaneBaseInfo from './components/ManagePaneBaseInfo'
import managePaneStoreSale from './components/ManagePaneStoreSale'
import PageHeader from '@/components/PageHeader/index.vue'
export default {
  name: 'ProductManage',
  components: { managePaneBaseInfo, managePaneStoreSale, PageHeader },
  data() {
    return {
      activeName: 'tabBaseInfo'
    }
  },
  watch: {
    '$route'(to, from) {
      this.id = to.query.id
      this.init()
    }
  },
  created() {
    this.id = getUrlParam('id')
    this.init()
  },
  methods: {
    init() {
      this.activeName = getUrlParam('tab')
    }
  }
}
</script>
<style lang="scss" scoped>

</style>
