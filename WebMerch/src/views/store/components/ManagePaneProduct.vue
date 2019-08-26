<template>
  <div v-loading="loading">

    <el-tabs v-model="activeName" tab-position="left">

      <el-tab-pane v-for="item in sellChannels" :key="item.name" :label="item.name" :name="item.name">
        <keep-alive>
          <manage-pane-product-list v-if="activeName==item.name" :store-id="storeId" :sellchannel="item" />
        </keep-alive>
      </el-tab-pane>

      <!-- <el-tab-pane label="所有">全部</el-tab-pane>
      <el-tab-pane label="快递商品">快递商品</el-tab-pane>
      <el-tab-pane label="机器商品">机器商品</el-tab-pane>
      <el-tab-pane label="机器商品">机器商品</el-tab-pane> -->

    </el-tabs>
  </div>
</template>
<script>
import { initManageProduct } from '@/api/store'
import managePaneProductList from './ManagePaneProductList'
import { getUrlParam } from '@/utils/commonUtil'

export default {
  name: 'ManagePaneProductList',
  components: { managePaneProductList },
  data() {
    return {
      storeId: '',
      loading: false,
      activeName: '全部',
      sellChannels: []
    }
  },
  watch: {
    '$route'(to, from) {
      this.init()
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      this.loading = true
      var id = getUrlParam('id')
      this.storeId = id
      initManageProduct({ id: id }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.sellChannels = d.sellChannels
        }
        this.loading = false
      })
    }
  }
}
</script>
