const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function pageData(urlParams) {

  return lumos.getJson({
    url: config.apiUrl.shopMarketPageData,
    urlParams:urlParams
  })
}

module.exports = {
  pageData: pageData
}