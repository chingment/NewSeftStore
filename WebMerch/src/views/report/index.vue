<template>
  <div class="app-container my-container1">
    <el-container>
      <el-aside width="200px">
        <el-menu router :default-active="navActive">
          <el-menu-item-group>
            <template slot="title">报表服务</template>
            <el-menu-item index="/report/storestockrealdata">店铺实时库存报表</el-menu-item>
            <el-menu-item index="/report/storestockdatehis">店铺历史库存报表</el-menu-item>
            <el-menu-item index="/report/storesalesdatehis">店铺销售概况表</el-menu-item>
            <el-menu-item index="/report/productskusalesdatehis">商品销售报表</el-menu-item>
            <el-menu-item index="/report/ordersalesdatehis">订单销售报表</el-menu-item>
          </el-menu-item-group>
        </el-menu>
      </el-aside>
      <el-container>
        <el-main>
          <router-view />
        </el-main>
      </el-container>
    </el-container>
  </div>
</template>

<script>
export default {
  name: 'ReportIndex',
  data() {
    return {
      navActive: '/order/list',
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    this.navActive = this.$route.path
    console.log('this.$route.params.path:' + this.$route.path)
  },
  methods: {

  }
}
</script>
