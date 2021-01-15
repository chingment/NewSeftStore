const ownRequest = require('../../own/ownRequest.js')
const apiShop = require('../../api/shop.js')
const storeage = require('../../utils/storeageutil.js')
const app = getApp()

Page({
  data: {
    tag: 'store'
  },
  onLoad: function (options) {
    var _this = this

    var isClearCache = options.isClearCache

    if (isClearCache != undefined) {
      wx.clearStorage()
    }

    var shopId = storeage.getShopId()

    function getShopList(lat, lng) {
      apiShop.list({
        storeId: storeage.getStoreId(),
        lat: lat,
        lng: lng
      }).then(function (res) {
        if (res.result == 1) {
          _this.setData({
            list: res.data,
            currentChoiceId: shopId == undefined ? '' : shopId
          })
        }
      })
    }


    var loaction = wx.getStorageSync("key_loaction");
    if (loaction == null) {
      loaction = {
        lat: 0,
        lng: 0
      }
    }

    wx.getLocation({
      type: 'wgs84',
      success: function (res) {
        var latitude = res.latitude
        var longitude = res.longitude
        wx.setStorageSync("key_loaction", {
          lat: latitude,
          lng: longitude
        })
        if (loaction.lat != latitude || loaction.lng != longitude) {
          getShopList(latitude, longitude)
        }
      }
    })

    getShopList(loaction.lat, loaction.lng)

  },
  onShow: function () {},
  onUnload: function () {},
  choice: function (e) {
    var choice = e.currentTarget.dataset.replyItem
    storeage.setShopId(choice.id);

    wx.navigateBack({
      //返回
      delta: 1
    })
    
  }
})