const ownRequest = require('../../own/ownRequest.js')
const apiStore = require('../../api/store.js')
const app = getApp()

Page({
  data: {},
  onLoad: function (options) {
    var _this = this

    var isClearCache = options.isClearCache

    if (isClearCache != undefined) {
      wx.clearStorage()
    }

    function getStoreList(lat, lng) {
      apiStore.list({
        lat: lat,
        lng: lng
      }, {
        success: function (res) {
          if (res.result == 1) {
            _this.setData({
              list: res.data,
              currentStoreId: ownRequest.getCurrentStoreId() == undefined ? '' : ownRequest.getCurrentStoreId()
            })
          }
        },
        fail: function () { }
      })
    }

    wx.getLocation({
      type: 'wgs84',
      success: function (res) {
        var latitude = res.latitude
        var longitude = res.longitude
        wx.setStorageSync("key_loaction", { lat: latitude, lng: longitude })
        getStoreList(latitude, longitude)
      }
    })

    var loaction = wx.getStorageSync("key_loaction");

    if (loaction == null) {
      loaction = { lat: 0, lng: 0 }
    }

    getStoreList(loaction.lat, loaction.lng)
  },
  selectStore: function (e) {
    var store = e.currentTarget.dataset.replyStore
    ownRequest.setCurrentStoreId(store.id);
    wx.reLaunch({
      url: ownRequest.getReturnUrl()
    })
  }
})