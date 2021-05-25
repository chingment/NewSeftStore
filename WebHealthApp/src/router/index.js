import Vue from 'vue'
import VueRouter from 'vue-router'

import Main_Index from '../views/report/month/Index.vue'
import Main_Monitor from '../views/report/month/Monitor.vue'
import Main_Energy from '../views/report/month/Energy.vue'
import Main_TagAdvise from '../views/report/month/TagAdvise.vue'

Vue.use(VueRouter)

const routes = [
  {
    path: '/',
    name: 'MainIndex',
    component: Main_Index,
    redirec: '/report/month/monitor',
    children: [
      {
        path: '/report/month/monitor',
        name: 'MainMonitor',
        component: Main_Monitor
      },
      {
        path: '/report/month/energy',
        name: 'MainEnergy',
        component: Main_Energy
      },
      {
        path: '/report/month/tagadvise',
        name: 'MainTagAdvise',
        component: Main_TagAdvise
      }
    ]
  }
]

const router = new VueRouter({
  routes
})

export default router
