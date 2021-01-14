const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')
const apiGlobal = require('../api/global.js')

function operate(params) {

  var promise = new Promise((resolve, reject) => {
    
    lumos.postJson({
      url: config.apiUrl.cartOperate,
      dataParams: params,
    }).then(function (res) {

      if (res.result == 1) {
        storeage.setCart(res.data)
        apiGlobal.msgTips({
          storeId: storeage.getStoreId()
        })
      }
      resolve(res);
    })

  })

  return promise;
}

function pageData() {

  var promise = new Promise((resolve, reject) => {
    lumos.getJson({
      url: config.apiUrl.cartPageData,
      isShowLoading: false,
      urlParams: {
        storeId: storeage.getStoreId()
      }
    }).then(function (res) {

      if (res.result == 1) {
        storeage.setCart(res.data)
      }

      resolve(res);
    })

  })

  return promise
}

function getCartData(urlParams) {

  return lumos.getJson({
    url: config.apiUrl.cartGetCartData,
    urlParams: urlParams,
    isShowLoading: false
  })
}

module.exports = {
  operate: operate,
  pageData: pageData,
  getCartData: getCartData
}