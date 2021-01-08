const apiMember = require('../../api/member.js')
const storeage = require('../../utils/storeageutil.js')
const toast = require('../../utils/toastutil')
const util = require('../../utils/util')
const app = getApp()

Page({

  /**
   * 页面的初始数据
   */
  data: {
    tag: "memberrightdesc",
    pageIsReady: false,
    right: 1,
    isMember: false,
    timespan: (new Date()).getTime(),
    userInfo: {}
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var _this = this
    var right = parseInt(options.right)

    _this.setData({
      right: right
    })

    var _this = this


    if (app.globalData.checkConfig) {
      _this.getRightDescSt()
    } else {
      app.checkConfigReadyCallback = res => {
        _this.getRightDescSt()
      }
    }


  },
  getRightDescSt: function () {
    var _this = this
    apiMember.getRightDescSt({
      openId: storeage.getOpenId()
    }).then(function (res) {
      if (res.result == 1) {

        var d = res.data
        _this.setData({
          pageIsReady: true,
          userInfo: d.userInfo
        })
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
    wx.navigateTo({
      url: '/pages/membercenter/membercenter'
    })
  },
  clickToTryUse: function (e) {
    var _this = this
    if (!util.isEmptyOrNull(_this.data.userInfo.phoneNumber)) {
      toast.show({
        title: '欢迎体验'
      })
      return
    }

    wx.navigateTo({
      url: '/pages/bindphonenumber/bindphonenumber'
    })
  }
})