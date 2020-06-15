const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function result(urlParams) {

  return lumos.getJson({
    url: config.apiUrl.operateResult,
    urlParams: urlParams,
    isShowLoading: false
  })
}

module.exports = {
  result: result
}