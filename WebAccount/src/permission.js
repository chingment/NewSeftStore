import router from './router'
import store from './store'
import NProgress from 'nprogress' // progress bar
import 'nprogress/nprogress.css' // progress bar style
import { getToken } from '@/utils/auth' // get token from cookie
import { getUrlParam, changeURLArg } from '@/utils/commonUtil'
import getPageTitle from '@/utils/get-page-title'

NProgress.configure({ showSpinner: false }) // NProgress Configuration

const whiteList = ['/login'] // no redirect whitelist

router.beforeEach(async(to, from, next) => {
  NProgress.start()
  document.title = getPageTitle(to.meta.title)
  var logout = getUrlParam('logout')
  var redirect = getUrlParam('redirect')
  var token = getToken()
  console.log('logout：' + logout)
  console.log('redirect：' + redirect)
  console.log('token befefore: ' + token)
  if (logout !== null) {
    token = undefined
    if (logout === '1') {
      await store.dispatch('own/logout')
    }
  }
  console.log('token after: ' + token)
  if (token) {
    var hasRedirect = false
    if (redirect != null) {
      if (redirect.toLowerCase().indexOf('http') > -1) {
        hasRedirect = true
      }
    }
    console.log('store.getters.userInfo：' + store.getters.userInfo)
    if (store.getters.userInfo == null) {
      await store.dispatch('own/getInfo', to.path).then((res) => {
        next({ ...to, replace: true })
      })
    } else {
      await store.dispatch('own/checkPermission', '1', to.path).then((res) => {
        if (hasRedirect) {
          var url = changeURLArg(decodeURIComponent(redirect), 'token', getToken())
          window.location.href = url
        } else {
          if (to.path === '/login') {
            next({ path: '/' })
            NProgress.done()
          } else {
            next()
          }
        }
      })
    }
  } else {
    if (whiteList.indexOf(to.path) !== -1) {
      next()
    } else {
      next(`/login?redirect=${to.path}`)
      NProgress.done()
    }
  }
})

router.afterEach(() => {
  // finish progress bar
  NProgress.done()
})
