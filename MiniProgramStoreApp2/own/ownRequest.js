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
  return store
}

function setCurrentStore(store) {
  storeage.setCurrentStore(store)
}

function isLogin() {

  var acctoken = storeage.getAccessToken()
  if (acctoken == "") {
    // showLoginModal()
    return false
  } else {
    return true
  }
}

function goLogin() {
  wx.navigateTo({
    url: loginPage,
  })
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
  // console.log("ownRequest.getReturnUrl->>>" + url)
  return url;
}


function isSelectedStore(isGoSelect) {
  var store = getCurrentStore()
  if (store.id == "") {
//    console.log("ownRequest.isSelectedStore->>>当前店铺id为空，未选择店铺")
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

module.exports = {
  goLogin: goLogin,
  getCurrentStoreId: getCurrentStoreId,
  setCurrentStore: setCurrentStore,
  getCurrentStore: getCurrentStore,
  isSelectedStore: isSelectedStore,
  isLogin: isLogin,
  getReturnUrl: getReturnUrl
}