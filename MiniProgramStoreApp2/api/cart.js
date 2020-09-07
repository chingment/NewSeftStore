const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')

function operate(params) {

  var promise = new Promise((resolve, reject) => {
    lumos.postJson({
      url: config.apiUrl.cartOperate,
      dataParams: params
    }).then(function (res1) {


      if (res1.result == 1) {
        pageData().then(function (res2) {
          var res3={"result":1,"code":1000,"message":"操作成功",data:res2.data}
          resolve(res3);
        })
      }
      else{
        resolve(res1);
      }
    })
  })

  return promise;
}

function pageData() {


  var promise = new Promise((resolve, reject) => {
    lumos.getJson({
      url: config.apiUrl.cartPageData,
      isShowLoading: false,
      urlParams: {
        storeId: ownRequest.getCurrentStoreId()
      }
    }).then(function (res) {

      if (res.result == 1) {
        storeage.setCart(res.data)
      }

      resolve(res);
    })

  })


  return promise
}

module.exports = {
  operate: operate,
  pageData: pageData
}