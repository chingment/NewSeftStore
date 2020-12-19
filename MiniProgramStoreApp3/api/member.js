const config = require('../config');
const lumos = require('../utils/lumos.minprogram.js')

function getPromSt(urlParams) {

  return lumos.getJson({
    url: config.apiUrl.memberGetPromSt,
    urlParams: urlParams,
    isShowLoading: false
  })
}

function getRightDescSt(urlParams) {

  return lumos.getJson({
    url: config.apiUrl.memberGetRightDescSt,
    urlParams: urlParams,
    isShowLoading: false
  })
}


function getPayLevelSt(urlParams) {

  return lumos.getJson({
    url: config.apiUrl.memberGetPayLevelSt,
    urlParams: urlParams,
    isShowLoading: false
  })
}

module.exports = {
  getPayLevelSt: getPayLevelSt,
  getPromSt: getPromSt,
  getRightDescSt: getRightDescSt
}