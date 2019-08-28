const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function result(urlParams, requestHandler) {

  lumos.getJson({
    url: config.apiUrl.operateResult,
    urlParams: urlParams,
    success: function (res) {
      requestHandler.success(res)
    }
  })
}

module.exports = {
  result: result
}