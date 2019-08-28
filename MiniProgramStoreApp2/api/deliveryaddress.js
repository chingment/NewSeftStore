const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function my(urlParams, requestHandler) {

  lumos.getJson({
    url: config.apiUrl.deliveryAddressMy,
    urlParams: urlParams,
    success: function (res) {
      requestHandler.success(res)
    }
  })
}

function edit(dataParams, requestHandler) {

  lumos.postJson({
    url: config.apiUrl.deliveryAddressEdit,
    dataParams: dataParams,
    success: function (res) {
      requestHandler.success(res)
    }
  })

}

module.exports = {
  my: my,
  edit: edit
}