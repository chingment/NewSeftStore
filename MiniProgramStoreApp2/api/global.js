const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function getDataSet(urlParams,requestHandler) {

  lumos.getJson({
    url: config.apiUrl.globalDataSet,
    urlParams: urlParams,
    success: function (res) {
      if (res.result == 1) {
        requestHandler.success(res)
      }
    }
  })
}

module.exports = {
  getDataSet: getDataSet
}