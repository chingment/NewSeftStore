const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')


function pageData(params,requestHandler) {

  lumos.getJson({
    url: config.apiUrl.productKindPageData,
    urlParams:params,
    success: function (res) {
      if (res.result == 1) {
        storeage.setCart(res.data)
        requestHandler.success(res)
      }
    }
  })
}

module.exports = {
  pageData: pageData
}