const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function pageData(params) {
  return lumos.getJson({
    url: config.apiUrl.indexPageData,
    urlParams: params,
    isShowLoading:false
  })
}

module.exports = {
  pageData: pageData
}