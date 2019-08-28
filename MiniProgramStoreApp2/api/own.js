const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function loginByMinProgram(dataParams, requestHandler) {

  lumos.postJson({
    url: config.apiUrl.ownLoginByMinProgram,
    dataParams: dataParams,
    success: function (res) {
      requestHandler.success(res)
    }
  })

}

module.exports = {
  loginByMinProgram: loginByMinProgram
}