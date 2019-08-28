const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const deliveryaddress = require('../../api/deliveryaddress.js')
const app = getApp()

Page({

  /**
   * 页面的初始数据
   */
  data: {
    operate: 0,
    operateIndex: 0,
    list: []
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var _this = this
    var operate = parseInt(options.operate)
    var operateIndex = parseInt(options.operateIndex)
    console.log("operateIndex:" + operateIndex)
    var title = ""
    switch (operate) {
      case 1:
        title = "地址管理";
        break;
      case 2:
        title = "选择地址";
        break;
    }
    wx.setNavigationBarTitle({
      title: title,
    })

    _this.setData({ operate: operate, operateIndex: operateIndex })

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
    deliveryaddress.my({}, {
      success: function (res) {
        _this.setData({
          list: res.data
        })
      },
      fail: function () { }
    })
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
  goEdit: function (e) {
    var _this = this
    var index = e.currentTarget.dataset.replyIndex //对应页面data-reply-index
    var deliveryAddress = _this.data.list[index]
    wx.navigateTo({
      url: '/pages/deliveryaddressedit/deliveryaddressedit?id=' + deliveryAddress.id + "&deliveryAddress=" + JSON.stringify(deliveryAddress),
      success: function (res) {
        // success
      },
    })
  },
  goSelect: function (e) {
    var _this = this
    var index = e.currentTarget.dataset.replyIndex //对应页面data-reply-index
    var deliveryAddress = _this.data.list[index]

    if (_this.data.operate == 2) {
      console.log('deliveryAddress:' + JSON.stringify(deliveryAddress))
      var pages = getCurrentPages();
      var prevPage = pages[pages.length - 2];
      prevPage.data.block[_this.data.operateIndex].deliveryAddress.id = deliveryAddress.id
      prevPage.data.block[_this.data.operateIndex].deliveryAddress.consignee = deliveryAddress.consignee
      prevPage.data.block[_this.data.operateIndex].deliveryAddress.phoneNumber = deliveryAddress.phoneNumber
      prevPage.data.block[_this.data.operateIndex].deliveryAddress.address = deliveryAddress.address
      prevPage.data.block[_this.data.operateIndex].deliveryAddress.areaName = deliveryAddress.areaName
      prevPage.data.block[_this.data.operateIndex].deliveryAddress.isDefault = deliveryAddress.isDefault
      prevPage.data.block[_this.data.operateIndex].deliveryAddress.defaultText = deliveryAddress.isDefault==true?"默认":""
      prevPage.setData({
        block: prevPage.data.block
      })

      wx.navigateBack()
    }

  }
})