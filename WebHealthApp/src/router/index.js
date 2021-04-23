import Vue from 'vue'
import VueRouter from 'vue-router'

import Main_Index from '../views/main/Index.vue'
import Main_Report from '../views/main/Report.vue'
import Main_Energy from '../views/main/Energy.vue'
import Main_Advise from '../views/main/Advise.vue'

Vue.use(VueRouter)

const routes = [
  {
    path: '/',
    name: 'MainIndex',
    component: Main_Index,
    redirec: 'report',
    children: [
      {
        path: '/',
        redirect: 'report'
      },
      {
        path: '/report',
        name: 'MainReport',
        component: Main_Report
      },
      {
        path: '/energy',
        name: 'MainEnergy',
        component: Main_Energy
      },
      {
        path: '/advise',
        name: 'MainAdvise',
        component: Main_Advise
      }
    ]
  }
]

const router = new VueRouter({
  routes
})

export default router
