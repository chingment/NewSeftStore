const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function confirm(params, requestHandler) {
  lumos.postJson({
    url: config.apiUrl.orderConfirm,
    dataParams: params,
    success: function (res) {
        requestHandler.success(res)
    }
  })
}

function reserve(params, requestHandler) {
  lumos.postJson({
    url: config.apiUrl.orderReserve,
    dataParams: params,
    success: function (res) {
      requestHandler.success(res)
    }
  })
}


function list(urlParams, requestHandler) {

  lumos.getJson({
    url: config.apiUrl.orderList,
    urlParams: urlParams,
    success: function (res) {
      requestHandler.success(res)
    }
  })
}

function details(urlParams, requestHandler) {

  lumos.getJson({
    url: config.apiUrl.orderDetails,
    urlParams: urlParams,
    success: function (res) {
      requestHandler.success(res)
    }
  })
}

function cancle(params, requestHandler) {
  lumos.postJson({
    url: config.apiUrl.orderCancle,
    dataParams: params,
    success: function (res) {
      requestHandler.success(res)
    }
  })
}

function buildPayParams(params, requestHandler) {

  lumos.postJson({
    url: config.apiUrl.orderBuildPayParams,
    dataParams: params,
    success: function (res) {
      requestHandler.success(res)
    }
  })
}

function buildPayOptions(urlParams, requestHandler) {

  lumos.getJson({
    url: config.apiUrl.orderBuildPayOptions,
    urlParams: urlParams,
    success: function (res) {
      requestHandler.success(res)
    }
  })
}

module.exports = {
  confirm: confirm,
  reserve: reserve,
  list: list,
  details: details,
  cancle: cancle,
  buildPayParams: buildPayParams,
  buildPayOptions: buildPayOptions
}