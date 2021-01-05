const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const apiDeliveryaddress = require('../../api/deliveryaddress.js')
const app = getApp()

Page({

  /**
   * 页面的初始数据
   */
  data: {
    tag: 'deliveryaddress',
    operate: 0,
    orderBlockIndex: 0,
    list: []
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var _this = this
    var operate = parseInt(options.operate)
    var orderBlockIndex = parseInt(options.orderBlockIndex)
    var currentSelectId = typeof options.currentSelectId == "undefined" ? "" : options.currentSelectId


    var title = ""
    switch (operate) {
      case 1:
        title = "我的联系方式";
        break;
      case 2:
        title = "选择联系方式";
        break;
    }
    wx.setNavigationBarTitle({
      title: title,
    })

    _this.setData({
      operate: operate,
      orderBlockIndex: orderBlockIndex,
      currentSelectId: currentSelectId
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
    var _this = this
    apiDeliveryaddress.my({}).then(function (res) {
      if (res.result == 1) {
        _this.setData({
          list: res.data
        })
      }
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
      var pages = getCurrentPages();
      var prevPage = pages[pages.length - 2];
      if (prevPage.data.tag == 'orderconfirm')

        if (typeof prevPage.data.blocks[_this.data.orderBlockIndex].delivery != 'undefined') {
          prevPage.data.blocks[_this.data.orderBlockIndex].delivery.contact.id = deliveryAddress.id
          prevPage.data.blocks[_this.data.orderBlockIndex].delivery.contact.consignee = deliveryAddress.consignee
          prevPage.data.blocks[_this.data.orderBlockIndex].delivery.contact.phoneNumber = deliveryAddress.phoneNumber
          prevPage.data.blocks[_this.data.orderBlockIndex].delivery.contact.address = deliveryAddress.address
          prevPage.data.blocks[_this.data.orderBlockIndex].delivery.contact.areaName = deliveryAddress.areaName
          prevPage.data.blocks[_this.data.orderBlockIndex].delivery.contact.areaCode = deliveryAddress.areaCode
          prevPage.data.blocks[_this.data.orderBlockIndex].delivery.contact.markName = deliveryAddress.markName
          prevPage.data.blocks[_this.data.orderBlockIndex].delivery.contact.isDefault = deliveryAddress.isDefault
  
        }

      if (typeof prevPage.data.blocks[_this.data.orderBlockIndex].selfTake != 'undefined') {
        prevPage.data.blocks[_this.data.orderBlockIndex].selfTake.contact.id = deliveryAddress.id
        prevPage.data.blocks[_this.data.orderBlockIndex].selfTake.contact.consignee = deliveryAddress.consignee
        prevPage.data.blocks[_this.data.orderBlockIndex].selfTake.contact.phoneNumber = deliveryAddress.phoneNumber
        prevPage.data.blocks[_this.data.orderBlockIndex].selfTake.contact.address = deliveryAddress.address
        prevPage.data.blocks[_this.data.orderBlockIndex].selfTake.contact.areaName = deliveryAddress.areaName
        prevPage.data.blocks[_this.data.orderBlockIndex].selfTake.contact.areaCode = deliveryAddress.areaCode
        prevPage.data.blocks[_this.data.orderBlockIndex].selfTake.contact.markName = deliveryAddress.markName
        prevPage.data.blocks[_this.data.orderBlockIndex].selfTake.contact.isDefault = deliveryAddress.isDefault
      }



      prevPage.setData({
        blocks: prevPage.data.blocks
      })

      wx.navigateBack()
    }

  }
})