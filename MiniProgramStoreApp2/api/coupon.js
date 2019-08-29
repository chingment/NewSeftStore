const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function my(urlParams, requestHandler) {

  lumos.postJson({
    url: config.apiUrl.couponMy,
    urlParams: urlParams,
    success: function (res) {
      requestHandler.success(res)
    }
  })
}

module.exports = {
  my: my
}