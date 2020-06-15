const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function operate(params) {
  operate = lumos.postJson({
    url: config.apiUrl.cartOperate,
    dataParams: params
  })

  operate.then(function(res){
    pageData()
  })

  return operate;
}

function pageData() {

  var operate= lumos.getJson({
    url: config.apiUrl.cartPageData,
    urlParams: {
      storeId: ownRequest.getCurrentStoreId()
    }
  })

  operate.then(function(res){
    if (res.result == 1) {
      storeage.setCart(res.data)
    }
  })

  return operate
}

module.exports = {
  operate: operate,
  pageData: pageData
}