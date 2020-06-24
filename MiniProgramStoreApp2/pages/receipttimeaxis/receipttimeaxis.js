const config = require('../../config')
const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
const QRCode = require('../../utils/qrcode.js')
const ownRequest = require('../../own/ownRequest.js')
const apiOrder = require('../../api/order.js')
const app = getApp()



Page({

  /**
   * 页面的初始数据
   */
  data: {
    receiptTimeAxis:{
      top:null,
      recordTop:{
        circleText:'',
        description:''
      },
      records:[]
    }
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var _this=this
    var uniqueId = options.uniqueId == undefined ? "" : options.uniqueId
    var uniqueType = options.uniqueType == undefined ? "" : options.uniqueType


    apiOrder.receiptTimeAxis({
      uniqueId: uniqueId,
      uniqueType:uniqueType
    }).then(function (res) {
      if (res.result == 1) {
  
        _this.setData({receiptTimeAxis:res.data})
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

  }
})