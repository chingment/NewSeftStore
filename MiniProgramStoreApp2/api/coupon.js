const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function my(params, requestHandler) {

  lumos.postJson({
    url: config.apiUrl.couponMy,
    dataParams: params,
    success: function (res) {
      requestHandler.success(res)
    }
  })
}

module.exports = {
  my: my
}