const toast = require('../../utils/toastutil')
const ownRequest = require('../../own/ownRequest.js')

Page({

  /**
   * 页面的初始数据
   */
  data: {
    navH: 40,
    statusBarHeight: 0,
    level: 1
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var _this = this
    wx.getSystemInfo({
      success: res => {
        //导航高度
        var statusBarHeight = res.statusBarHeight
        var navHeight = statusBarHeight + 46
        _this.setData({
          statusBarHeight: statusBarHeight,
          navH: navHeight
        })
      },
      fail(err) {
        console.log(err);
      }
    })
  },

  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady: function () {

  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function () {

  },

  /**
   * 生命周期函数--监听页面隐藏
   */
  onHide: function () {

  },

  /**
   * 生命周期函数--监听页面卸载
   */
  onUnload: function () {

  },

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh: function () {

  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function () {

  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function () {

  },
  clickToPurchase: function (e) {

    var _this = this

    if (!ownRequest.isLogin()) {
      ownRequest.goLogin()
      return
    }

    var skuId = '00000000000000000000000000000000' //对应页面data-reply-index
    var productSkus = []
    productSkus.push({
      cartId: 0,
      id: skuId,
      quantity: 1,
      shopMode: 4
    })
    wx.navigateTo({
      url: '/pages/orderconfirm/orderconfirm?productSkus=' + JSON.stringify(productSkus),
      success: function (res) {
        // success
      },
    })
  },
  clickToNavGoBack: function () {
    wx.navigateBack({
      complete: (res) => {},
    })
  },
  clickToTabLevel(e) {
    var _this = this
    var level = e.currentTarget.dataset.replyLevel

    console.log("level:" + level)

    _this.setData({
      level: level
    })

  },
})