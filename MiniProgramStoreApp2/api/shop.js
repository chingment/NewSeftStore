const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function list(urlParams) {

  return lumos.getJson({
    url: config.apiUrl.shopList,
    urlParams:urlParams
  })
}

module.exports = {
  list: list
}