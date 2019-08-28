const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function confirm(params, requestHandler) {
  lumos.postJson({
    url: config.apiUrl.cartOperate,
    dataParams: params,
    success: function (res) {
        requestHandler.success(res)
    }
  })
}

function reserve(params, requestHandler) {
  lumos.postJson({
    url: config.apiUrl.cartOperate,
    dataParams: params,
    success: function (res) {
      requestHandler.success(res)
    }
  })
}


function list(urlParams, requestHandler) {

  lumos.getJson({
    url: config.apiUrl.productGetList,
    urlParams: urlParams,
    success: function (res) {
      requestHandler.success(res)
    }
  })
}

function details(urlParams, requestHandler) {

  lumos.getJson({
    url: config.apiUrl.productGetDetails,
    urlParams: urlParams,
    success: function (res) {
      requestHandler.success(res)
    }
  })
}

function cancle(params, requestHandler) {
  lumos.postJson({
    url: config.apiUrl.cartOperate,
    dataParams: params,
    success: function (res) {
      requestHandler.success(res)
    }
  })
}

function getJsApiPaymentPms(urlParams, requestHandler) {

  lumos.getJson({
    url: config.apiUrl.productGetDetails,
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
  getJsApiPaymentPms: getJsApiPaymentPms
}