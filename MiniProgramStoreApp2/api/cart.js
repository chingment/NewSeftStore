const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function operate(params, requestHandler) {
  return lumos.postJson({
    url: config.apiUrl.cartOperate,
    dataParams: params
  }).then(function (res) {
    if (res.result == 1) {
      pageData(requestHandler)
    }
  })
}

function pageData(requestHandler) {

  return lumos.getJson({
    url: config.apiUrl.cartPageData,
    urlParams: {
      storeId: ownRequest.getCurrentStoreId()
    }
  }).then(function (res) {

    if (res.result == 1) {
      storeage.setCart(res.data)
    }
  })
}

module.exports = {
  operate: operate,
  pageData: pageData
}