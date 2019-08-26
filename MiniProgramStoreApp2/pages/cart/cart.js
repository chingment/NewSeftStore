const config = require('../../config');
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const lumos = require('../../utils/lumos.minprogram.js')
var app = getApp()

function operate(params, requestHandler) {

  console.log('ownRequest.getCurrentStoreId():' + ownRequest.getCurrentStoreId())

  lumos.postJson({
    url: config.apiUrl.cartOperate,
    dataParams: params,
    success: function (d) {
      requestHandler.success(d)
      if (d.result == 1) {
        getPageData()
      }
    }
  })
}

function getPageData() {

  lumos.getJson({
    url: config.apiUrl.cartGetPageData,
    urlParams: {
      storeId: ownRequest.getCurrentStoreId()
    },
    success: function (res) {
      if (res.result == 1) {
        storeage.setCart(res.data)
        app.mainTabBarSetNumber(2, res.data.count)
      }
    }
  })
}

module.exports = {
  operate: operate,
  getPageData: getPageData
}