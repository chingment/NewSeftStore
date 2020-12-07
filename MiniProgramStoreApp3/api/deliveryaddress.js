const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function my(urlParams) {

return  lumos.getJson({
    url: config.apiUrl.deliveryAddressMy,
    urlParams: urlParams
  })
}

 function edit(dataParams) {

 return lumos.postJson({
    url: config.apiUrl.deliveryAddressEdit,
    dataParams: dataParams
  })

}

module.exports = {
  my: my,
  edit: edit
}