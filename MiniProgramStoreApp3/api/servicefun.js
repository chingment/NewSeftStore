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

module.exports = {
  scanCodeResult: scanCodeResult
}