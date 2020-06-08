const config = require('../config');
const lumos = require('../utils/lumos.minprogram.js')

function tobeSearch(urlParams, requestHandler) {

  lumos.getJson({
    url: config.apiUrl.tobeSearch,
    urlParams: urlParams,
    isShowLoading:false,
    success: function (res) {
      requestHandler.success(res)
    }
  })
}

module.exports = {
  tobeSearch: tobeSearch
}