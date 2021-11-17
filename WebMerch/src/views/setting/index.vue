<template>
  <div class="app-container my-container1">
    <el-container>
      <el-aside class="my-aside">
        <el-menu router :default-active="navActive">
          <el-menu-item-group>
            <template slot="title">基础数据</template>
            <el-menu-item index="/setting/adminuser/list">用户权限</el-menu-item>
            <el-menu-item index="/setting/merch/list">商户管理</el-menu-item>
          </el-menu-item-group>
          <el-menu-item-group>
            <template slot="title">日志服务</template>
            <el-menu-item index="/setting/log/list">日志查看</el-menu-item>
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
  name: 'SettingIndex',
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
