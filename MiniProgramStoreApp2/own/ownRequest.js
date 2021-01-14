const storeage = require('../utils/storeageutil.js')
var loginPage = '../login/login'


function isLogin() {
  var acctoken = storeage.getAccessToken()
  if (acctoken == "") {
    // showLoginModal()
    return false
  } else {
    return true
  }
}

function goLogin(returnUrl) {

  if (typeof returnUrl != 'undefined'&&returnUrl!='') {
    loginPage += '?returnUrl=' + returnUrl
  }

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
  console.log("ownRequest.getReturnUrl->>>" + url)
  return url;
}


// function isSelectedStore(isGoSelect) {
//   var storeId = 
//   if (storeId == undefined || storeId == null || storeId == "") {
//     isGoSelect = isGoSelect == undefined ? false : isGoSelect
//     if (isGoSelect) {
//       wx.navigateTo({ //保留当前页面，跳转到应用内的某个页面（最多打开5个页面，之后按钮就没有响应的）
//         url: "/pages/store/store"
//       })
//     }
//     return false
//   } else {
//     return true
//   }
// }

module.exports = {
  goLogin: goLogin,
  isLogin: isLogin,
  getReturnUrl: getReturnUrl
}