import Vue from 'vue'

import 'normalize.css/normalize.css' // A modern alternative to CSS reset

import '@/styles/index.scss' // global css

import App from './views/App'
import store from './store'
import router from './router'
import VueCookies from 'vue-cookies'
import Vant from 'vant'
import 'vant/lib/index.css'

import '@/permission' // permission control

/**
 * If you don't want to use mock-server
 * you want to use MockJs for mock api
 * you can execute: mockXHR()
 *
 * Currently MockJs will be used in the production environment,
 * please remove it before going online! ! !
 */
// import { mockXHR } from '../mock'
// if (process.env.NODE_ENV === 'production') {
//   mockXHR()
// }
Vue.use(Vant)
Vue.use(VueCookies)
Vue.config.productionTip = false

new Vue({
  el: '#app',
  router,
  store,
  render: h => h(App)
})
