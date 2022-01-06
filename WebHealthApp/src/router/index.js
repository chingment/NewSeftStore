import Vue from 'vue'
import VueRouter from 'vue-router'

// import Main_Index from '../views/report/month/Index.vue'
// import Main_Monitor from '../views/report/month/Monitor.vue'
// import Main_Energy from '../views/report/month/Energy.vue'
// import Main_TagAdvise from '../views/report/month/TagAdvise.vue'

Vue.use(VueRouter)

const router = new VueRouter({
  mode: 'hash',
  routes: [
    {
      path: '/own/info',
      name: 'OwnInfo',
      component: () => import('@/views/own/info')
    },
    {
      path: '/device/bind',
      name: 'DeviceBind',
      component: () => import('@/views/device/bind')
    },
    {
      path: '/device/info',
      name: 'DeviceInfo',
      component: () => import('@/views/device/info')
    },
    {
      path: '/quest/fill/tp1',
      name: 'QuestFillTp1',
      component: () => import('@/views/quest/fill/tp1')
    },
    {
      path: '/report/month/monitor',
      name: 'MainIndex',
      component: () => import('@/views/report/month/Index'),
      redirec: '/report/month/monitor',
      children: [
        {
          path: '/report/month/monitor',
          name: 'MainMonitor',
          component: () => import('@/views/report/month/Monitor')
        },
        {
          path: '/report/month/energy',
          name: 'MainEnergy',
          component: () => import('@/views/report/month/Energy')
        },
        {
          path: '/report/month/tagadvise',
          name: 'MainTagAdvise',
          component: () => import('@/views/report/month/TagAdvise')
        }
      ]
    }
  ]
})

export default router
