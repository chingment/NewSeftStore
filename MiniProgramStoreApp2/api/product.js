const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function initSearchPageData(urlParams, requestHandler) {

  lumos.getJson({
    url: config.apiUrl.productInitSearchPageData,
    urlParams: urlParams,
    success: function (res) {
      requestHandler.success(res)
    }
  })
}

function search(urlParams, requestHandler) {

  lumos.getJson({
    url: config.apiUrl.productSearch,
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
  initSearchPageData:initSearchPageData,
  search: search,
  details: details,
  skuStockInfo:skuStockInfo
}