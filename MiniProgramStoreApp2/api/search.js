const config = require('../config');
const lumos = require('../utils/lumos.minprogram.js')

function tobeSearch(urlParams) {

 return lumos.getJson({
    url: config.apiUrl.tobeSearch,
    urlParams: urlParams,
    isShowLoading:false
  })
}

module.exports = {
  tobeSearch: tobeSearch
}