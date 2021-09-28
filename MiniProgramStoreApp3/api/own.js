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

function getWxACodeUnlimit(dataParams) {

  return lumos.postJson({
    url: config.apiUrl.ownGetWxACodeUnlimit,
    dataParams: dataParams
  })
}

function WxApiCode2Session(dataParams) {

  return lumos.postJson({
    url: config.apiUrl.ownWxApiCode2Session,
    dataParams: dataParams,
    isShowLoading: false
  })
}

function wxPhoneNumber(dataParams) {

  return lumos.postJson({
    url: config.apiUrl.ownWxPhoneNumber,
    dataParams: dataParams
  })
}

function wxConfig(dataParams) {
  return lumos.postJson({
    url: config.apiUrl.ownConfig,
    dataParams: dataParams,
    isShowLoading: false
  })
}

function bindPhoneNumberByWx(dataParams) {

  return lumos.postJson({
    url: config.apiUrl.ownBindPhoneNumberByWx,
    dataParams: dataParams
  })
}

module.exports = {
  loginByMinProgram: loginByMinProgram,
  wxConfig: wxConfig,
  wxPhoneNumber: wxPhoneNumber,
  bindPhoneNumberByWx: bindPhoneNumberByWx,
  getWxACodeUnlimit:getWxACodeUnlimit
}