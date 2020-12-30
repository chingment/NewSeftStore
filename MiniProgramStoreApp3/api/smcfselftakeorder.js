const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function cfTake(params) {

  return lumos.postJson({
    url: config.apiUrl.smCfSelfTakeOrderCfTake,
    dataParams: params
  })
}

function details(urlParams) {

  return lumos.getJson({
    url: config.apiUrl.smCfSelfTakeOrderDetails,
    urlParams: urlParams
  })
}


module.exports = {
  cfTake: cfTake,
  details:details
}