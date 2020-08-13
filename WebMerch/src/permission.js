import router from './router'
import store from './store'
import NProgress from 'nprogress' // progress bar
import 'nprogress/nprogress.css' // progress bar style
import { getToken } from '@/utils/auth' // get token from cookie
import { getUrlParam } from '@/utils/commonUtil'
import getPageTitle from '@/utils/get-page-title'

NProgress.configure({ showSpinner: false }) // NProgress Configuration

router.beforeEach(async(to, from, next) => {
  NProgress.start()

  document.title = getPageTitle(to.meta.title)

  if (to.meta.auth === undefined) {
    var token = getUrlParam('token')
    if (token !== null) {
      store.dispatch('own/setToken', token)
    }
    token = getToken()
    // console.log('token: ' + token)
    var path = encodeURIComponent(window.location.href)
    if (token) {
      if (store.getters.userInfo == null) {
        await store.dispatch('own/getInfo').then((res) => {
          next({ ...to, replace: true })
        })
      }

      await store.dispatch('own/checkPermission', '1', to.path).then((res) => {
        if (res.code === 2401) {
          next('/401')
        } else {
          next()
        }
      })

      NProgress.done()
    } else {
      window.location.href = `${process.env.VUE_APP_LOGIN_URL}?appId=${process.env.VUE_APP_ID}&logout=2&redirect=${path}`
      NProgress.done()
    }
  } else {
    console.log('dsdad2')
    next()
    NProgress.done()
  }
})

router.afterEach(() => {
  // finish progress bar
  NProgress.done()
})
