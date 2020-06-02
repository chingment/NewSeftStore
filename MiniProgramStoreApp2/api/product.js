const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function list(urlParams, requestHandler) {

  lumos.getJson({
    url: config.apiUrl.productList,
    urlParams: urlParams,
    success: function (res) {
      requestHandler.success(res)
    }
  })
}

function details(urlParams, requestHandler) {

  lumos.getJson({
    url: config.apiUrl.productDetails,
    urlParams: urlParams,
    success: function (res) {
      requestHandler.success(res)
    }
  })
}

function skuStockInfo(urlParams, requestHandler) {

  lumos.getJson({
    url: config.apiUrl.productSkuStockInfo,
    urlParams: urlParams,
    success: function (res) {
      requestHandler.success(res)
    }
  })
}

module.exports = {
  list: list,
  details: details,
  skuStockInfo:skuStockInfo
}