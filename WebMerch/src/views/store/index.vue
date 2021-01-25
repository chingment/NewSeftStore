<template>
  <div class="app-container my-container1">
    <el-container>
      <el-aside class="my-aside">
        <el-menu router :default-active="navActive">
          <el-menu-item-group>
            <template slot="title">店铺</template>
            <el-menu-item index="/store/list">店铺列表</el-menu-item>
          </el-menu-item-group>
          <el-menu-item-group>
            <template slot="title">门店</template>
            <el-menu-item index="/shop/list">门店列表</el-menu-item>
            <el-menu-item index="/shop/add">新建门店</el-menu-item>
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
  name: 'StoreIndex',
  data() {
    return {
      navActive: '/store/list',
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
