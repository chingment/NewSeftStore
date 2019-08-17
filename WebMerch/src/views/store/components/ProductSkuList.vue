<template>
  <div v-loading="loading">
    <el-tabs tab-position="left">

      <el-tab-pane v-for="item in sellChannels" :key="item.id" :label="item.name" :name="item.id" />

      <!-- <el-tab-pane label="所有">全部</el-tab-pane>
      <el-tab-pane label="快递商品">快递商品</el-tab-pane>
      <el-tab-pane label="机器商品">机器商品</el-tab-pane>
      <el-tab-pane label="机器商品">机器商品</el-tab-pane> -->

    </el-tabs>
  </div>
</template>
<script>
import { fetchList, initGetProductSkuList } from '@/api/store'
export default {
  props: {
    storeId: {
      type: String,
      default: 'CN'
    }
  },
  data() {
    return {
      loading: false,
      sellChannels: [],
      listQuery: {
        page: 1,
        limit: 10,
        name: undefined
      }
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      this.loading = true
      initGetProductSkuList({ storeId: this.storeId }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.sellChannels = d.sellChannels
        }
        this.loading = false
      })

      this.getListData()
    },
    getListData() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      fetchList(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d
        }
        this.loading = false
      })
    }
  }
}
</script>
