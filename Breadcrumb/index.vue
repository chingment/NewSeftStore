<template>
  <el-breadcrumb class="app-breadcrumb" separator="/">
    <transition-group name="breadcrumb">
      <el-breadcrumb-item v-for="(item,index) in levelList" :key="item.path">
        <span v-if="item.redirect==='noRedirect'||index==levelList.length-1" class="no-redirect">{{ item.meta.title }}</span>
        <a v-else @click.prevent="handleLink(item)">{{ item.meta.title }}</a>
      </el-breadcrumb-item>
    </transition-group>
  </el-breadcrumb>
</template>

<script>
import pathToRegexp from 'path-to-regexp'
import { Tree } from 'element-ui'
import { close } from 'fs'

export default {
  data() {
    return {
      levelList: null,
      arouters: [
        {
          orgId: 1,
          parentId: 0,
          path: '/',
          meta: { title: '系统设置', icon: 'table' },
          breadcrumb: true
        },
        {
          orgId: 3,
          parentId: 1,
          path: '/adminuser/list',
          meta: { title: '用户设置', icon: 'table' },
          breadcrumb: true, children: [{
            orgId: 4,
            parentId: 3,
            path: '/adminuser/add',
            meta: { title: '用户新建', icon: 'table' },
            breadcrumb: true
          },
          {
            orgId: 5,
            parentId: 3,
            path: '/adminuser/edit',
            meta: { title: '用户编辑', icon: 'table' },
            breadcrumb: true
          }]
        }
      ]
    }
  },
  watch: {
    $route() {
      this.getBreadcrumb()
    }
  },
  created() {
    this.getBreadcrumb()
  },
  methods: {
    getBreadcrumb() {
      var parentList = []
      var matc = []
      function buildParentList(arr) {
        arr.forEach(g => {
          if (g.parentId !== undefined) {
            const pid = g['parentId']
           	const oid = g['orgId']
            // /parentList[oid] = pid
            parentList.push(g)
          }
          if (g.children !== undefined) { buildParentList(g['children']) }
        })
      }
      function findParent(idx) {
  	parentList.forEach(g => {
          if (g.orgId === idx) {
            matc.push(g)
            console.log(g.parentId)
            findParent(g.parentId)
          }
        })

        //   if (parentList[idx] != undefined){
        //       let pid = parentList[idx]
        //       matc.push()
        //       console.log(pid)
        //       findParent(pid)
        // }
      }

      buildParentList(this.arouters)

      findParent(4) // 0 1 2

      matc = matc.reverse()

      let matched = matc
      const first = matched[0]

      if (!this.isDashboard(first)) {
        matched = [{ path: '/home', meta: { title: '主页' }}].concat(matched)
      }

      // this.levelList = matched.filter(item => item.meta && item.meta.title && item.meta.breadcrumb !== false)
      // console.log('levelList:' + JSON.stringify(this.levelList))
      // only show routes with meta.title
      // console.log('matched:'+ JSON.stringify(this.$route.matched))
    //   let matched = this.arouters.filter(item => item.meta && item.meta.title)
    //    console.log('matched:'+JSON.stringify(matched))
    //   const first = matched[0]
    //   if (!this.isDashboard(first)) {
    //     matched = [{ path: '/home', meta: { title: '主页' }}].concat(matched)
    //   }
    // console.log('matched2:'+JSON.stringify(matched))
    //   this.levelList = matched.filter(item => item.meta && item.meta.title && item.meta.breadcrumb !== false)
    //    console.log('levelList:'+JSON.stringify(this.levelList))
    },
    isDashboard(route) {
      const name = route && route.name
      if (!name) {
        return false
      }
      return name.trim().toLocaleLowerCase() === 'Home'.toLocaleLowerCase()
    },
    pathCompile(path) {
      // To solve this problem https://github.com/PanJiaChen/vue-element-admin/issues/561
      const { params } = this.$route
      var toPath = pathToRegexp.compile(path)
      return toPath(params)
    },
    handleLink(item) {
      const { redirect, path } = item
      if (redirect) {
        this.$router.push(redirect)
        return
      }
      this.$router.push(this.pathCompile(path))
    }
  }
}
</script>

<style lang="scss" scoped>
.app-breadcrumb.el-breadcrumb {
  display: inline-block;
  font-size: 14px;
  line-height: 50px;
  margin-left: 8px;

  .no-redirect {
    color: #97a8be;
    cursor: text;
  }
}
</style>
