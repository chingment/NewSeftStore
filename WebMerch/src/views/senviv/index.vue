<template>
  <div class="app-container my-container1">
    <el-container>
      <el-aside class="my-aside">
        <el-menu router :default-active="navActive">
          <el-menu-item-group>
            <template slot="title">心晓</template>
            <el-menu-item index="/senviv/users">客户信息</el-menu-item>
            <el-menu-item index="/senviv/dayreport">健康报告（日）</el-menu-item>
            <el-menu-item index="/senviv/monthreport">健康报告（月）</el-menu-item>
            <el-menu-item index="/senviv/tagexplain">标签解读</el-menu-item>
            <el-menu-item index="/senviv/article">文章知识库</el-menu-item>
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
// v-permission="['Merch_Home_Shopss']"
import permission from '@/directive/permission/index.js' // 权限判断指令

export default {
  name: 'ReportIndex',
  directives: { permission },
  data() {
    return {
      navActive: '/senviv/users',
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
