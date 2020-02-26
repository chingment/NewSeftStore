const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function list(urlParams, requestHandler) {

  lumos.getJson({
    url: config.apiUrl.storeList,
    urlParams: {
      lat: urlParams.lat,
      lng: urlParams.lng
    },
    success: function(res) {
      requestHandler.success(res)
    }
  })
}

module.exports = {
  list: list
}