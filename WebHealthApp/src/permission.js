import router from './router'
import NProgress from 'nprogress' // progress bar
import 'nprogress/nprogress.css' // progress bar style

NProgress.configure({ showSpinner: false }) // NProgress Configuration

router.beforeEach(async(to, from, next) => {
  console.log('beforeEach')
  NProgress.start()

  // if (to.name !== 'auth') { // 判断当前是不是新建的auth路由空白页面
  //   const _token = sessionStorage.getItem('wechataccess_token')
  //   if (!_token) { // 若是没有token,则让它受权
  //     // 保存当前路由地址，受权后还会跳到此地址
  //     sessionStorage.setItem('beforeUrl', to.fullPath)
  //     // 受权请求,并跳转http://m.water.ui-tech.cn/auth路由页面
  //     window.location.href = 'https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxf0d98b28bebd0c82&redirect_uri=http%3A%2F%2Fm.water.ui-tech.cn%2Fauth&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect'
  //   } else {
  //     next()
  //   }
  // } else {
  //   next()
  // }

  next()
  NProgress.done()
})

router.afterEach(() => {
  // finish progress bar
  NProgress.done()
})
