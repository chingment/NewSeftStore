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

function details(params) {

  return lumos.getJson({
    url: config.apiUrl.couponDetails,
    urlParams: params,
    isShowLoading: false
  })
}

function revPosSt(params) {

  return lumos.getJson({
    url: config.apiUrl.couponRevPosSt,
    urlParams: params,
    isShowLoading: false
  })
}

function receive(params) {

  return lumos.postJson({
    url: config.apiUrl.couponReceive,
    dataParams: params
  })
}

module.exports = {
  my: my,
  revPosSt: revPosSt,
  receive: receive,
  details: details
}