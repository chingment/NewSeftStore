import store from '@/store'

import router from '@/router'
import Layout from '@/layout'

function _generateRoutes(routers, menus, isCheckSidebar) {
  menus.forEach((item) => {
    var path = item.path
    if (item.path.indexOf('?') > -1) {
      path = item.path.split('?')[0]
    }
    const component = resolve => require([`@/views${item.component}`], resolve)
    const menu = {
      path: path,
      component: component,
      children: undefined,
      redirect: undefined,
      hidden: !item.isSidebar,
      name: item.name,
      meta: { title: item.title, icon: item.icon, id: item.id, pId: item.pId }
    }
    if (item.children) {
      if (menu.children === undefined) {
        menu.children = []
      }
      const redirect = item.redirect == null ? undefined : item.redirect

      menu.redirect = redirect
      _generateRoutes(menu.children, item.children, isCheckSidebar)
    }

    if (isCheckSidebar) {
      if (item.isSidebar) {
        routers.push(menu)
      }
    } else {
      routers.push(menu)
    }
  })
}

export function getSideBars() {
  var menus = store.getters.userInfo.menus
  var routers = []
  _generateRoutes(routers, menus, true)
  return routers
}

export function getNavbars() {
  var navbars = []
  store.getters.userInfo.menus.forEach((item) => {
    if (item.isNavbar) {
      navbars.push(item)
    }
  })
  return navbars
}

export function generateRoutes(data) {
  console.log('data:' + JSON.stringify(data))

  var routers = []

  _generateRoutes(routers, data, false)

  var _routers = [{
    path: '/',
    component: Layout,
    redirect: '/home',
    children: routers
  },
  { path: '*', redirect: '/404', hidden: true }]

  console.log('routers:' + JSON.stringify(_routers))

  router.addRoutes(_routers)
}

