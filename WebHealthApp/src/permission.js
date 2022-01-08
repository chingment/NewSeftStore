import router from './router'
import NProgress from 'nprogress' // progress bar
import 'nprogress/nprogress.css' // progress bar style
import { isEmpty } from '@/utils/commonUtil'
import { authUrl, authInfo } from '@/api/own'
import { getToken, setToken } from '@/utils/auth'
NProgress.configure({ showSpinner: false }) // NProgress Configuration

function delUrlParams(url, names) {
  if (typeof (names) === 'string') {
    names = [names]
  }
  //  http:/dsfsfsfs/?dfdsf=32321&code=33
  if (url.indexOf('?') > -1) {
    var href_cp = url.split('?')
    var loc_url = href_cp[0]
    var loc_qry = href_cp[1]
    var obj = {}
    var arr = loc_qry.split('&')
    // 获取参数转换为object
    for (var i = 0; i < arr.length; i++) {
      arr[i] = arr[i].split('=')
      obj[arr[i][0]] = arr[i][1]
    }
    // 删除指定参数
    for (var j = 0; j < names.length; j++) {
      delete obj[names[j]]
    }
    // 重新拼接url
    url = loc_url + '?' + JSON.stringify(obj).replace(/[\"\{\}]/g, '').replace(/\:/g, '=').replace(/\,/g, '&')
  }
  return url
}

router.beforeEach(async(to, from, next) => {
  NProgress.start()

  // 判断是否需要微信授权
  if (to.meta.isAuth) {
    var token = getToken()
    var isGoAuth = true
    if (token != null) {
      isGoAuth = false
      // 检查token是否过期
    }

    var code = to.query.code
    var merchId = to.query.merchId
    var deviceId = to.query.deviceId

    if (isGoAuth) {
      if (!isEmpty(code)) {
        await authInfo({ merchId: merchId, deviceId: deviceId, code: code }).then((res) => {
          if (res.result === 1) {
            var d = res.data
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

  NProgress.done()
})

router.afterEach(() => {
  // finish progress bar
  NProgress.done()
})
