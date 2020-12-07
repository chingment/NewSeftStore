const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function loginByMinProgram(dataParams) {

  return lumos.postJson({
    url: config.apiUrl.ownLoginByMinProgram,
    dataParams: dataParams
  })
}

function WxApiCode2Session(dataParams) {

  return lumos.postJson({
    url: config.apiUrl.ownWxApiCode2Session,
    dataParams: dataParams,
    isShowLoading:false
  })
}

function wxPhoneNumber(dataParams) {

  return lumos.postJson({
    url: config.apiUrl.ownWxPhoneNumber,
    dataParams: dataParams
  })
}

module.exports = {
  loginByMinProgram: loginByMinProgram,
  WxApiCode2Session: WxApiCode2Session,
  wxPhoneNumber: wxPhoneNumber
}