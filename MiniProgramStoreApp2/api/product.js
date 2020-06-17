const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function initSearchPageData(urlParams) {

  return lumos.getJson({
    url: config.apiUrl.productInitSearchPageData,
    urlParams: urlParams
  })
}

function search(urlParams,isShowLoading) {
  return lumos.getJson({
    url: config.apiUrl.productSearch,
    isShowLoading:isShowLoading,
    urlParams: urlParams
  })
}

function details(urlParams) {

  return lumos.getJson({
    url: config.apiUrl.productDetails,
    urlParams: urlParams
  })
}

function skuStockInfo(urlParams) {

  return lumos.getJson({
    url: config.apiUrl.productSkuStockInfo,
    urlParams: urlParams
  })
}

module.exports = {
  initSearchPageData: initSearchPageData,
  search: search,
  details: details,
  skuStockInfo: skuStockInfo
}