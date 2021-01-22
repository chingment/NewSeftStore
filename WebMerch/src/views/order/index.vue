<template>
  <div class="app-container my-container1">
    <el-container>
      <el-aside width="200px">
        <el-menu router :default-active="navActive">
          <el-menu-item-group>
            <template slot="title">订单中心</template>
            <el-menu-item index="/order/list">全部订单</el-menu-item>
            <el-menu-item index="/order/listbymt">机器自提</el-menu-item>
            <el-menu-item index="/order/listbyst">店铺自提</el-menu-item>
            <el-menu-item index="/order/listbyet">配送到手</el-menu-item>
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
  name: 'OrderCenter',
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
