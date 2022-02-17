import Vue from 'vue'

import 'normalize.css/normalize.css' // A modern alternative to CSS reset

import '@/styles/index.scss' // global css

import App from './views/App'
import store from './store'
import router from './router'
import VueCookies from 'vue-cookies'
import 'mint-ui/lib/style.css'
import Mint from 'mint-ui'
import '@/icons' // icon
import '@/permission' // permission control
import vueHashCalendar from 'vue-hash-calendar'
import 'vue-hash-calendar/lib/vue-hash-calendar.css'
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
Vue.use(Mint)
Vue.use(VueCookies)
Vue.use(vueHashCalendar)
Vue.config.productionTip = false

new Vue({
  el: '#app',
  router,
  store,
  render: h => h(App)
})
