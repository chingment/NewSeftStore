<template>
  <div class="app-container my-container1">
    <el-container>
      <el-aside class="my-aside">
        <el-menu router :default-active="navActive">
          <el-menu-item-group>
            <template slot="title">支付中心</template>
            <el-menu-item index="/paytrans/pay/query">支付查询</el-menu-item>
          </el-menu-item-group>
          <el-menu-item-group>
            <template slot="title">退款中心</template>
            <el-menu-item index="/paytrans/refund/query">退款查询</el-menu-item>
            <el-menu-item index="/paytrans/refund/apply">申请退款</el-menu-item>
            <el-menu-item index="/paytrans/refund/handle">退款处理</el-menu-item>
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
  name: 'PayTransIndex',
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
