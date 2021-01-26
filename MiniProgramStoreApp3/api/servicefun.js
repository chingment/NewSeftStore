const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function scanCodeResult(params) {

  return lumos.postJson({
    url: config.apiUrl.serviceFunScanCodeResult,
    dataParams: params
  })
}

function getMyReffSkus(urlParams) {

  return  lumos.getJson({
      url: config.apiUrl.serviceFunGetMyReffSkus,
      urlParams: urlParams
    })
  }

module.exports = {
  scanCodeResult: scanCodeResult,
  getMyReffSkus:getMyReffSkus
}