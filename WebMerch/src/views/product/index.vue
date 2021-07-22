<template>
  <div class="app-container my-container1">
    <el-container>
      <el-aside class="my-aside">
        <el-menu router :default-active="navActive">
          <el-menu-item-group>
            <template slot="title">商品库</template>
            <el-menu-item index="/product/list">商品列表</el-menu-item>
            <el-menu-item index="/product/add">新建商品</el-menu-item>
            <el-menu-item index="/product/recyclebin">回收站</el-menu-item>
          </el-menu-item-group>
          <el-menu-item-group>
            <template slot="title">在售信息</template>
            <el-menu-item index="/product/listbysale">出售中的SKU</el-menu-item>
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
  name: 'ProductIndex',
  data() {
    return {
      navActive: '/product/list',
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
