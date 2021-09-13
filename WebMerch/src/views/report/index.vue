<template>
  <div class="app-container my-container1">
    <el-container>
      <el-aside class="my-aside">
        <el-menu router :default-active="navActive">
          <el-menu-item-group>
            <template slot="title">报表服务</template>
            <el-menu-item index="/report/storesummary">店铺概况报表</el-menu-item>
            <el-menu-item index="/report/devicesummary">设备概况报表</el-menu-item>
            <el-menu-item index="/report/devicestocksummary">设备实时总表</el-menu-item>
            <el-menu-item index="/report/devicestockrealdata">设备实时库存报表</el-menu-item>
            <el-menu-item index="/report/devicereplenishplan">设备补货计划报表</el-menu-item>
            <el-menu-item index="/report/skusaleshis">商品销售报表</el-menu-item>
            <el-menu-item index="/report/ordersaleshis">订单销售报表</el-menu-item>
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
  },
  methods: {

  }
}
</script>
