const config = require('../../config')
const ownRequest = require('../../own/ownRequest.js')
const lumos = require('../../utils/lumos.minprogram.js')
const app = getApp()

Page({
  data: {},
  onLoad: function(options) {
    var _this = this


    function getStores(lat, lng) {
      lumos.getJson({
        url: config.apiUrl.storeList,
        urlParams: {
          merchId: config.merchId,
          lat: lat,
          lng: lng
        },
        success: function (res) {
          _this.setData({
            list: res.data,
            currentStore: ownRequest.getCurrentStore()
          })
        }
      })
    }

    wx.getLocation({
      type: 'wgs84',
      success: function(res) {
        // console.log(res);
        var latitude = res.latitude
        var longitude = res.longitude


        getStores(latitude, longitude)

        //弹框
        // wx.showModal({
        //   title: '当前位置',
        //   content: "纬度:" + latitude + ",经度:" + longitude,
        // })
      }
    })


    getStores(0,0)
  },
  selectStore: function(e) {
    var store = e.currentTarget.dataset.replyStore
    ownRequest.setCurrentStore(store);
    wx.reLaunch({
      url: ownRequest.getReturnUrl()
    })
  }
})