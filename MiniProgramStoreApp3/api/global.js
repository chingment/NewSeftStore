const config = require('../config');
const storeage = require('../utils/storeageutil.js')
const ownRequest = require('../own/ownRequest.js')
const lumos = require('../utils/lumos.minprogram.js')


function dataSet(urlParams) {

  return lumos.getJson({
    url: config.apiUrl.globalDataSet,
    urlParams: urlParams
  })
}

function msgTips(urlParams) {

  lumos.getJson({
    url: config.apiUrl.globalMsgTips,
    urlParams: urlParams,
    isShowLoading: false
  }).then(function (res) {
    if (res.result == 1) {
      var d = res.data
      var pages = getCurrentPages();
      for (var i = 0; i < pages.length; i++) {
        if (pages[i].data.tag.indexOf("main-") > -1) {
          pages[i].data.tabBar[2].badge = d.badgeByCart
          pages[i].data.tabBar[3].badge = d.badgeByPersonal
          pages[i].setData({
            tabBar: pages[i].data.tabBar
          })
        }
      }
    }
  })

}

function byPoint(page, action, param) {

  const accountInfo = wx.getAccountInfoSync()
  var appId = accountInfo.miniProgram.appId

  lumos.postJson({
    url: config.apiUrl.globalByPoint,
    isShowLoading: false,
    dataParams: {
      appId:appId,
      page: page,
      action: action,
      param: param,
      usrSign:storeage.getOpenId()
    }
  })
}


function getWxSceneData(params) {
  return lumos.getJson({
    url: config.apiUrl.globalGetWxSceneData,
    urlParams: params,
    isShowLoading: false
  })
}

module.exports = {
  dataSet: dataSet,
  msgTips: msgTips,
  byPoint: byPoint,
  getWxSceneData:getWxSceneData
}