const storeage = require('../utils/storeageutil.js')
var loginPage = '../login/login'

function getAccessonToken() {
  var token = storeage.getAccessToken()
  return token
}

function getCurrentStoreId() {
  var store = storeage.getCurrentStore();
  return store.id
}

function getCurrentStore() {
  var store = storeage.getCurrentStore();
  if (store == "") {
    store = {
      id: "",
      name: "未选择店铺"
    }
  } else {
    store = storeage.getCurrentStore()
  }
  console.log("ownRequest.getCurrentStore->>>" + JSON.stringify(store))
  return store
}

function setCurrentStore(store) {
  storeage.setCurrentStore(store)
  console.log("ownRequest.setCurrentStore->>>" + JSON.stringify(store))
}

function isLogin() {

  var acctoken = storeage.getAccessToken()
  if (acctoken == "") {
    showLoginModal()
    return false
  } else {
    return true
  }
}

function goLogin() {
  showLoginModal()
}

function getReturnUrl() {
  var pages = getCurrentPages()
  var url = "/pages/main/main";
  if (pages.length >= 2) {
    var currentPage = pages[pages.length - 2] //获取当前页面的对象
    url = "/" + currentPage.route //当前页面url
    url += "?"
    for (let key in currentPage.options) {
      const value = currentPage.options[key]
      url += `${key}=${value}&`
    }
    url = url.substring(0, url.length - 1)
  }
  console.log("ownRequest.getReturnUrl->>>" + url)
  return url;
}


function isSelectedStore(isGoSelect) {
  var store = getCurrentStore()
  if (store.id == "") {
    console.log("ownRequest.isSelectedStore->>>当前店铺id为空，未选择店铺")
    isGoSelect = isGoSelect == undefined ? false : isGoSelect
    if (isGoSelect) {
      wx.navigateTo({ //保留当前页面，跳转到应用内的某个页面（最多打开5个页面，之后按钮就没有响应的）
        url: "/pages/store/store"
      })
    }
    return false
  } else {
    return true
  }
}

function login(callback) {
  wx.showLoading()
  wx.login({
    success(res) {
      if (res.code) {
        // 登录成功，获取用户信息
        getUserInfo(res.code, callback)
      } else {
        // 否则弹窗显示，showToast需要封装
        showToast()
      }
    },
    fail() {
      showToast()
    }
  })
}

// 获取用户信息
function getUserInfo(code, callback) {
  wx.getUserInfo({
    // 获取成功，全局存储用户信息，开发者服务器登录
    success(res) {
      //console.log("code:" + code)
      //console.log("iv:" + res.iv)
      //console.log("encryptedData:" + res.encryptedData)
      console.log("userInfo:" + JSON.stringify(res))
      // 全局存储用户信息
      //store.commit('storeUpdateWxUser', res.userInfo)
      // postLogin(code, res.iv, res.encryptedData, callback)


      let params = {
        code: code,
        iv: res.iv,
        encryptedData: res.encryptedData
      }
      callback && callback(params)
      // lumos.postJson({
      //   url: config.apiUrl.userLoginByMinProgram, dataParams: params,
      //   success: function (res) {
      //     if (res.result == 1) {
      //       storeage.setAccessToken(res.data.accessToken);
      //       console.log("getAccessonToken" + storeage.getAccessToken())
      //       callback && callback()
      //     }
      //   },
      //   fail: function () {
      //     showToast()
      //   }
      // })
    },
    // 获取失败，弹窗提示一键登录
    fail() {
      wx.hideLoading()
      // 获取用户信息失败，清楚全局存储的登录状态，弹窗提示一键登录
      // 使用token管理登录态的，清楚存储全局的token
      // 使用cookie管理登录态的，可以清楚全局登录状态管理的变量
      //store.commit('storeUpdateToken', '')
      // 获取不到用户信息，说明用户没有授权或者取消授权。弹窗提示一键登录，后续会讲
      showLoginModal()
    }
  })
}


function showLoginModal() {
  wx.showModal({
    title: '提示',
    content: '你还未登录，登录后可获得完整体验 ',
    showCancel: false,
    confirmText: '一键登录',
    success(res) {
      // 点击一键登录，去授权页面
      if (res.confirm) {
        wx.navigateTo({
          url: loginPage,
        })
      }
    }
  })
}

function showToast(content = '登录失败，请稍后再试') {
  wx.showToast({
    title: content,
    icon: 'none'
  })
}

function rem2px(rem) {
  var size = 0;

  var wHidth = wx.getSystemInfoSync().windowWidth;
  console.log("rem2px->>>wHidth:" + wHidth)

  if (wHidth >= 360 & wHidth < 414) {
    size = wHidth / 23 * rem
  }
  else if (wHidth >= 414) {
    size = wHidth / 26 * rem
  }
  else {
    size = wHidth / 26 * rem
  }

  console.log("rem2px->>>size:" + rem + "," + size)
  return size
}

module.exports = {
  goLogin: goLogin,
  getCurrentStoreId: getCurrentStoreId,
  setCurrentStore: setCurrentStore,
  getCurrentStore: getCurrentStore,
  isSelectedStore: isSelectedStore,
  isLogin: isLogin,
  login: login,
  getReturnUrl: getReturnUrl,
  rem2px: rem2px
}