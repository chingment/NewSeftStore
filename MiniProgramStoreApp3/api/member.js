const config = require('../config');
const lumos = require('../utils/lumos.minprogram.js')

function getPayLevelSt(urlParams) {

 return lumos.getJson({
    url: config.apiUrl.getPayLevelSt,
    urlParams: urlParams,
    isShowLoading:false
  })
}

module.exports = {
  getPayLevelSt: getPayLevelSt
}