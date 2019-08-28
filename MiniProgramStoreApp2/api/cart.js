const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function operate(params, requestHandler) {
  lumos.postJson({
    url: config.apiUrl.cartOperate,
    dataParams: params,
    success: function (res) {
      if (res.result == 1) {
        requestHandler.success(res)
        pageData(requestHandler)
      }
    }
  })
}

function pageData(requestHandler) {

  lumos.getJson({
    url: config.apiUrl.cartPageData,
    urlParams: {
      storeId: ownRequest.getCurrentStoreId()
    },
    success: function (res) {
      if (res.result == 1) {
        storeage.setCart(res.data)
        requestHandler.success(res)
      }
    }
  })
}

module.exports = {
  operate: operate,
  pageData: pageData
}