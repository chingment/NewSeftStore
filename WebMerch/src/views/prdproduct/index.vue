<template>
  <div class="app-container my-container1">
    <el-container>
      <el-aside width="200px">
        <el-menu router :default-active="navActive">
          <el-menu-item-group>
            <template slot="title">商品</template>
            <el-menu-item index="/prdproduct/list">商品列表</el-menu-item>
            <el-menu-item index="/prdproduct/add">新建商品</el-menu-item>
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
  name: 'PrdProductIndex',
  data() {
    return {
      navActive: '/prdproduct/list',
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