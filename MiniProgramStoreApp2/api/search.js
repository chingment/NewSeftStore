const config = require('../config');
const lumos = require('../utils/lumos.minprogram.js')

function tobeSearch(urlParams, requestHandler) {

  lumos.getJson({
    url: config.apiUrl.tobeSearch,
    urlParams: urlParams,
    success: function (res) {
      requestHandler.success(res)
    }
  })
}

module.exports = {
  tobeSearch: tobeSearch
}