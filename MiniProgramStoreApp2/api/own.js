const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function loginByMinProgram(dataParams) {

 return  lumos.postJson({
    url: config.apiUrl.ownLoginByMinProgram,
    dataParams: dataParams
  })
}

module.exports = {
  loginByMinProgram: loginByMinProgram
}