import router from './router'
import NProgress from 'nprogress' // progress bar
import 'nprogress/nprogress.css' // progress bar style

import { isEmpty, delUrlParams } from '@/utils/commonUtil'
import { authUrl, authInfo, authTokenCheck } from '@/api/own'
import { getToken, setToken, removeToken } from '@/utils/auth'
NProgress.configure({ showSpinner: false }) // NProgress Configuration

router.beforeEach(async(to, from, next) => {
  NProgress.start()

  // 判断是否需要微信授权
  if (to.meta.isAuth) {
    var token = getToken()
    var isGoAuth = true
    if (token != null) {
      await authTokenCheck({ token: token }).then((res) => {
        if (res.code === 2501) {
          removeToken()
          isGoAuth = true
        } else {
          isGoAuth = false
        }
      })
    }

    var code = to.query.code
    var merchId = to.query.merchId
    var deviceId = to.query.deviceId

    if (isGoAuth) {
      if (!isEmpty(code)) {
        await authInfo({ merchId: merchId, deviceId: deviceId, code: code }).then((res) => {
          if (res.result === 1) {
            var d = res.data
            console.log('token1:' + d.token)
            setToken(d.token)
            isGoAuth = false
          } else {
            next('/errorpage/service')
          }
        })
      }
    }

    if (isGoAuth) {
      if (isEmpty(merchId) && isEmpty(deviceId)) {
        next('/errorpage/invalid')
      } else {
        var redriect_Url = encodeURIComponent(delUrlParams(window.location.href, ['code', 'state']))
        await authUrl({ merchId: merchId, deviceId: deviceId, redriectUrl: redriect_Url }).then((res) => {
          if (res.result === 1) {
            var d = res.data
            window.location.href = d.url
          } else {
            next('/errorpage/service')
          }
        })
      }
    }
    next()
  } else {
    next()
  }

  // next()
  NProgress.done()
})

router.afterEach(() => {
  // finish progress bar
  NProgress.done()
})
