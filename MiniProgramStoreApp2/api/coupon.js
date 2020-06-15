const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function my(params, requestHandler) {

 return lumos.postJson({
    url: config.apiUrl.couponMy,
    dataParams: params
  })
}

module.exports = {
  my: my
}