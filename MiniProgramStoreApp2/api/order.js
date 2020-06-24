const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function confirm(params, requestHandler) {
  return lumos.postJson({
    url: config.apiUrl.orderConfirm,
    dataParams: params
  })
}

function reserve(params) {
  return lumos.postJson({
    url: config.apiUrl.orderReserve,
    dataParams: params
  })
}


function list(urlParams) {

  return lumos.getJson({
    url: config.apiUrl.orderList,
    urlParams: urlParams
  })
}

function details(urlParams) {

  return lumos.getJson({
    url: config.apiUrl.orderDetails,
    urlParams: urlParams
  })
}

function cancle(params) {
  return lumos.postJson({
    url: config.apiUrl.orderCancle,
    dataParams: params
  })
}

function buildPayParams(params) {

  return lumos.postJson({
    url: config.apiUrl.orderBuildPayParams,
    dataParams: params
  })
}

function buildPayOptions(urlParams) {

 return lumos.getJson({
    url: config.apiUrl.orderBuildPayOptions,
    urlParams: urlParams
  })
}

function receiptTimeAxis(urlParams) {

  return lumos.getJson({
     url: config.apiUrl.orderReceiptTimeAxis,
     urlParams: urlParams
   })
 }

module.exports = {
  confirm: confirm,
  reserve: reserve,
  list: list,
  details: details,
  receiptTimeAxis:receiptTimeAxis,
  cancle: cancle,
  buildPayParams: buildPayParams,
  buildPayOptions: buildPayOptions
}