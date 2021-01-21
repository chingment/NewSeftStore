import store from '@/store'
import { constantRoutes } from '@/router'
import router from '@/router'
import Layout from '@/layout'
import Main from '@/views/order/index.vue'
import Tour from '@/views/order/listbymt.vue'
export function getSideBars() {
  // meta: { title: '用户设置', icon: 'table' },
  function toTree(data) {
    // 删除 所有 children,以防止多次调用
    data.forEach(function(item) {
      delete item.children
    })

    // 将数据存储为 以 id 为 KEY 的 map 索引数据列
    var map = {}
    data.forEach(function(item) {
      map[item.id] = item
    })
    //        console.log(map);
    var val = []
    data.forEach(function(item) {
      // 以当前遍历项，的pid,去map对象中找到索引的id
      var parent = map[item.pId]
      // 好绕啊，如果找到索引，那么说明此项不在顶级当中,那么需要把此项添加到，他对应的父级中
      if (parent) {
        (parent.children || (parent.children = [])).push(item)
      } else {
        // 如果没有在map中找到对应的索引ID,那么直接把 当前的item添加到 val结果集中，作为顶级
        val.push(item)
      }
    })
    return val
  }

  var sildeBar = []
  var menus = store.getters.userInfo.menus
  menus.filter(item => {
    if (item.isSidebar) {
      sildeBar.push(item)
    }
  })

  // console.log('sildeBar:' + JSON.stringify(sildeBar))
  var data = toTree(sildeBar)

  // console.log('arr:' + JSON.stringify(data))

  var routers = []
  function _generateRoutes(routers, menus) {
    menus.forEach((item) => {
      const menu = {
        path: item.path,
        children: undefined,
        meta: { title: item.title, icon: item.icon }
      }
      if (item.children) {
        if (menu.children === undefined||menu.children==null) {
          menu.children = []
        }
        _generateRoutes(menu.children, item.children)
      }
      routers.push(menu)
    })
  }
  _generateRoutes(routers, data)

  return routers
}

export function getBreadcrumb(route) {
  var matc = []
  var parentList = store.getters.userInfo.menus

  function findParent(idx) {
    parentList.forEach(g => {
      if (g.id === idx) {
        matc.push({
          path: g.path,
          meta: { title: g.title, icon: g.icon }
        })
        findParent(g.pId)
      }
    })
  }

  findParent(route.meta.id)

  matc = matc.reverse()

  // console.log(JSON.stringify(matc))

  return matc
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
  console.log('data:'+JSON.stringify(data))
 
 
  var _layoutRouter = {
      path: '/',
      component: Layout,
      redirect: '/home',
      children: [
      ]
    }

  var routers = []
  function _generateRoutes(routers, menus) {
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
            redirect:undefined,
            hidden: item.hidden,
            name: item.name,
            meta: { title: item.title, icon: item.icon, id: item.id, pId: item.pId }
      }
      if (item.children) {
        if (menu.children === undefined) {
          menu.children = []
        }
        const redirect=item.redirect==null?undefined:item.redirect

         menu.redirect=redirect
        _generateRoutes(menu.children, item.children)
      }
      routers.push(menu)
    })
  }

  _generateRoutes(routers, data)


  //_layoutRouter.children.push(routers)

  //routers.push(_layoutRouter)
  // var s=[]
  // function _generateRoutes(router, menus) {
  //   var _layoutRouter = {
  //     path: '/',
  //     component: Layout,
  //     redirect: '/home',
  //     children: [
  //     ]
  //   }

  //   menus.filter(item => {
  //     if (item.isRouter) {
  //       if (item.component !== null) {
  //         var path = item.path
  //         if (item.path.indexOf('?') > -1) {
  //           path = item.path.split('?')[0]
  //         }

  //         const component = resolve => require([`@/views${item.component}`], resolve)

  //         var _router = {
  //           path: path,
  //           component: component,
  //           children: undefined,
  //           hidden: item.hidden,
  //           name: item.name,
  //           meta: { title: item.title, icon: item.icon, id: item.id, pId: item.pId }
  //         }
  //         _layoutRouter.children.push(_router)
  //       }
  //     }
  //   })

  //   s.push(_layoutRouter)
  //   router.push(_layoutRouter)
  // }

  // _generateRoutes(constantRoutes, data)

  // console.log(JSON.stringify(s))

  //constantRoutes.push({ path: '*', redirect: '/404', hidden: true })

  //_layoutRouter.children.push(routers)
//{"path":"/home","name":"MerchHome","meta":{"title":"主页","icon":"dashboard","id":"25f3d80ce69a48b683f6aa2d6c899fca","pId":"10000000000000000000000000000025"}}


// [{
//   path:'/order',
//   component:Main,
//   children:[{
//    path:'list',
//    component:Tour,
//   }]
// }]

  var _routers=[{
    path: '/',
    component: Layout,
    redirect: '/home',
    children: routers
},
{ path: '*', redirect: '/404', hidden: true }]

console.log('routers:'+JSON.stringify(_routers))

  router.addRoutes(_routers)
}

