const config = require('../../config')
const ownRequest = require('../../own/ownRequest.js')
const apiCoupon = require('../../api/coupon.js')
const skeletonData = require('./skeletonData')
const toast = require('../../utils/toastutil')
const storeageutil = require('../../utils/storeageutil')
const app = getApp()

Page({

  /**
   * 页面的初始数据
   */
  data: {
    skeletonLoadingTypes: ['spin', 'chiaroscuro', 'shine', 'null'],
    skeletonSelectedLoadingType: 'shine',
    skeletonIsDev: false,
    skeletonBgcolor: '#FFF',
    skeletonData,
    pageIsReady: false,
    topImgUrl: '',
    coupons: []
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var _this = this
    
    _this.getCoupons()
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
    var _this = this
    app.globalData.skeletonPage = _this
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
  getCoupons: function () {
    var _this = this
    apiCoupon.revCenterSt({}).then(function (res) {
      if (res.result == 1) {
        var d = res.data

        _this.setData({
          topImgUrl: d.topImgUrl,
          coupons: d.coupons,
          pageIsReady:true
        })
      }
    })

  },
  clickToReceive: function (e) {
    var _this = this

    var coupon = e.currentTarget.dataset.replyItem

    apiCoupon.receive({
      couponId: coupon.id
    }).then(function (res) {

      toast.show({
        title: res.message
      })

      if (res.result == 1) {
        _this.getCoupons()
      }
    })

  }
})