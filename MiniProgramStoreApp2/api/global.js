const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function dataSet(urlParams) {

return  lumos.getJson({
    url: config.apiUrl.globalDataSet,
    urlParams: urlParams
  })
}

module.exports = {
  dataSet: dataSet
}