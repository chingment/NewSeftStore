<template>
  <div :class="{'has-logo':showLogo}">
    <logo v-if="showLogo" :collapse="isCollapse" />
    <el-scrollbar wrap-class="scrollbar-wrapper">
      <el-menu
        :default-active="activeMenu"
        :collapse="isCollapse"
        :background-color="variables.menuBg"
        :text-color="variables.menuText"
        :unique-opened="false"
        :active-text-color="variables.menuActiveText"
        :collapse-transition="false"
        mode="vertical"
      >
        <sidebar-item v-for="route in routes" :key="route.path" :item="route" :base-path="route.path" />
      </el-menu>
    </el-scrollbar>
  </div>
</template>

<script>
import { mapGetters } from 'vuex'
import Logo from './Logo'
import SidebarItem from './SidebarItem'
import variables from '@/styles/variables.scss'

export default {
  components: { SidebarItem, Logo },
  computed: {
    ...mapGetters([
      'sidebar'
    ]),
    routes() {
      // var arr = [{
      //   path: '/',
      //   meta: { title: '系统设置', icon: 'table' },
      //   children: [
      //     {
      //       path: '/adminuser/list',
      //       meta: { title: '用户设置', icon: 'table' },
      //       children: [{
      //         path: '/adminuser/add',
      //         hidden: false,
      //         meta: { title: '用户新建', icon: 'table' }
      //       },
      //       {
      //         path: '/adminuser/edit',
      //         hidden: false,
      //         meta: { title: '用户修改', icon: 'table' }
      //       }]
      //     }
      //   ]
      // }]
      // console.log(JSON.stringify(this.$router.options.routes))
      // var s = [{
      //   path: '/adminuser',
      //   meta: { title: '用户管理', icon: 'example', name: 'User' },
      //   children: [{ path: 'list', meta: { title: '用户列表', icon: 'table' }
      //   }, { path: 'add', meta: { title: '用户列表', icon: 'table' }
      //   }]
      // }]
      // return arr
      // console.log( JSON.stringify(this.$router.options.routes))
      return this.$router.options.routes
    },
    activeMenu() {
      const route = this.$route
      const { meta, path } = route
      // if set path, the sidebar will highlight the path you set
      if (meta.activeMenu) {
        return meta.activeMenu
      }
      return path
    },
    showLogo() {
      return this.$store.state.settings.sidebarLogo
    },
    variables() {
      return variables
    },
    isCollapse() {
      return !this.sidebar.opened
    }
  }
}
</script>
