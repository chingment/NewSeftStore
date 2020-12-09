const toast = require('../../utils/toastutil')
const ownRequest = require('../../own/ownRequest.js')
const config = require('../../config')
const apiMember = require('../../api/member.js')
Page({

  /**
   * 页面的初始数据
   */
  data: {
    navH: 40,
    statusBarHeight: 0,
    curlevelSt: 1,
    levelSt1: null,
    levelSt2: null,
    isMember: false
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


    apiMember.getPayLevelSt({
      appCaller: 1
    }).then(function (res) {
      if (res.result == 1) {

        var d = res.data

        _this.setData({
          levelSt1: d.levelSt1,
          levelSt2: d.levelSt2
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

    var _this = this

    if (!ownRequest.isLogin()) {
      ownRequest.goLogin()
      return
    }

    var curFeeSt
    if (_this.data.curlevelSt == 1) {
      console.log("1")
      curFeeSt = _this.data.levelSt1.feeSts[_this.data.levelSt1.curFeeStIdx]
    } else if (_this.data.curlevelSt == 2) {
      console.log("2")
      curFeeSt = _this.data.levelSt2.feeSts[_this.data.levelSt2.curFeeStIdx]
    }

    console.log("_this.data.curlevelSt:"+_this.data.curlevelSt)
    console.log("curFeeSt:"+JSON.stringify(curFeeSt))

    var skuId = curFeeSt.id //对应页面data-reply-index
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
      curlevelSt: level
    })
  },
  clickToFeeSt(e) {
    var _this = this
    var level = e.currentTarget.dataset.replyLevel
    var feeStIdx = e.currentTarget.dataset.replyFeestidx

    if (level == 1) {
      var levelSt1 = _this.data.levelSt1
      levelSt1.curFeeStIdx = feeStIdx
      _this.setData({
        levelSt1: levelSt1
      })
    }
    console.log("level:" + level + ",feeStIdx:" + feeStIdx)
  }
})