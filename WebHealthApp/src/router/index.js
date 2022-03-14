import Vue from 'vue'
import VueRouter from 'vue-router'

// import Main_Index from '../views/report/month/Index.vue'
// import Main_Monitor from '../views/report/month/Monitor.vue'
// import Main_Energy from '../views/report/month/Energy.vue'
// import Main_TagAdvise from '../views/report/month/TagAdvise.vue'

Vue.use(VueRouter)

const router = new VueRouter({
  mode: 'history',
  routes: [
    {
      path: '/own/info',
      component: () => import('@/views/own/info'),
      meta: { title: '我的信息', isAuth: true }
    },
    {
      path: '/device/bind',
      component: () => import('@/views/device/bind'),
      meta: { title: '设备绑定', isAuth: true }
    },
    {
      path: '/device/info',
      component: () => import('@/views/device/info'),
      meta: { title: '设备信息', isAuth: true }
    },
    {
      path: '/device/infofield',
      component: () => import('@/views/device/infofield'),
      meta: { title: '设备资料', isAuth: true }
    },
    {
      path: '/device/manage',
      component: () => import('@/views/device/manage'),
      meta: { title: '设备管理', isAuth: true }
    },
    {
      path: '/device/fill',
      component: () => import('@/views/device/fill'),
      meta: { title: '设备信息', isAuth: true }
    },
    {
      path: '/article/details',
      component: () => import('@/views/article/details'),
      meta: { title: '文章知识', isAuth: false }
    },
    {
      path: '/imitate/lyingin',
      component: () => import('@/views/imitate/lyingin'),
      meta: { title: '', isAuth: true }
    },
    {
      path: '/errorpage/invalid',
      component: () => import('@/views/errorpage/invalid'),
      meta: { title: '错误提示', isAuth: false }
    },
    {
      path: '/errorpage/service',
      component: () => import('@/views/errorpage/service'),
      meta: { title: '错误提示', isAuth: false }
    },
    {
      path: '/ws/test',
      component: () => import('@/views/ws/test'),
      meta: { title: 'ws测送', isAuth: false }
    },
    {
      path: '/report/day',
      component: () => import('@/views/report/day/Index'),
      meta: { title: '健康报告', isAuth: false }
    },
    {
      path: '/report/month/monitor',
      component: () => import('@/views/report/month/Index'),
      redirec: '/report/month/monitor',
      meta: { title: '健康报告', isAuth: false },
      children: [
        {
          path: '/report/month/monitor',
          meta: { title: '健康报告', isAuth: false },
          component: () => import('@/views/report/month/Monitor')
        },
        {
          path: '/report/month/energy',
          meta: { title: '健康报告', isAuth: false },
          component: () => import('@/views/report/month/Energy')
        },
        {
          path: '/report/month/tagadvise',
          meta: { title: '健康报告', isAuth: false },
          component: () => import('@/views/report/month/TagAdvise')
        }
      ]
    }
  ]
})

export default router
