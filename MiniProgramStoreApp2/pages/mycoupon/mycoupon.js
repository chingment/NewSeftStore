const config = require('../../config')
const ownRequest = require('../../own/ownRequest.js')
const apiCoupon = require('../../api/coupon.js')
const app = getApp()

Page({

  /**
   * 页面的初始数据
   */
  data: {

  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function(options) {
    var _this = this
    var operate = parseInt(options.operate)

    var operateIndex = parseInt(options.operateIndex)
    var productSkuIds = options.productSkuIds == undefined ? [] : JSON.parse(options.productSkuIds)
    var isGetHis = options.isGetHis
    var couponId = options.couponId == undefined ? [] : JSON.parse(options.couponId)
    var title = ""
    switch (operate) {
      case 1:
        title = "我的优惠卷";
        break;
      case 2:
        title = "选择优惠卷";
        break;
      case 3:
        title = "历史优惠卷";
        break;
    }
    wx.setNavigationBarTitle({
      title: title
    })

    var isGetHis = false

    apiCoupon.my({
      isGetHis: isGetHis,
      couponId: couponId,
      productSkuIds: productSkuIds
    }, {
        success: function (res) {
          if (res.result == 1) {
            _this.setData({
              coupon: res.data,
              operate: operate
            })
          }
        },
        fail: function () { }
      })

  },
  goSelect: function(e) {
    var _this = this
    var index = e.currentTarget.dataset.replyIndex //对应页面data-reply-index
    var coupon = _this.data.coupon[index]
    var operate = _this.data.operate

    if (operate == 1) {
      app.switchTab(0)
    } else if (operate == 2) {
      var couponId = []
      if (coupon.isSelected) {
        coupon.isSelected = false
      } else {
        coupon.isSelected = true
      }

      if (coupon.isSelected) {
        couponId.push(coupon.id)
      }

      var pages = getCurrentPages();
      var prevPage = pages[pages.length - 2];
      prevPage.setData({
        couponId: couponId
      })
      wx.navigateBack()
    }
  },
  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady: function() {

  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function() {

  },

  /**
   * 生命周期函数--监听页面隐藏
   */
  onHide: function() {

  },

  /**
   * 生命周期函数--监听页面卸载
   */
  onUnload: function() {

  },

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh: function() {

  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function() {

  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function() {

  }
})