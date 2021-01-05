const config = require('../../config')
const ownRequest = require('../../own/ownRequest.js')
const apiCoupon = require('../../api/coupon.js')
const toast = require('../../utils/toastutil')
const pageMain = require('../../pages/main/main.js')
const app = getApp()

Page({

  /**
   * 页面的初始数据
   */
  data: {
    tag: "mycoupon",
    op_faceTypes: ''
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var _this = this
    var operate = parseInt(options.operate)

    var operateIndex = parseInt(options.operateIndex)
    var productSkus = options.productSkus == undefined ? [] : JSON.parse(options.productSkus)
    var isGetHis = options.isGetHis
    var couponIds = options.couponIds == undefined ? [] : JSON.parse(options.couponIds)
    var shopMethod = options.shopMethod == undefined ? 0 : options.shopMethod
    var storeId = options.storeId == undefined ? '' : options.storeId
    var op_faceTypes = options.faceTypes == undefined ? '' : options.faceTypes

    var faceTypes
    if (op_faceTypes != '') {
      faceTypes = op_faceTypes.split(',')
    }

    _this.setData({
      op_faceTypes: op_faceTypes
    })

    var title = ""
    switch (operate) {
      case 1:
        title = "我的优惠卷";
        break;
      case 2:
        title = "选择优惠卷";
        switch (op_faceTypes) {
          case '1,2':
            title = "选择优惠卷";
            break;
          case '3':
            title = "选择租金卷";
            break;
          case '4':
            title = "选择押金卷";
            break;
        }
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
      couponIds: couponIds,
      productSkus: productSkus,
      shopMethod: shopMethod,
      storeId: storeId,
      faceTypes: faceTypes
    }).then(function (res) {
      if (res.result == 1) {
        var d = res.data
        _this.setData({
          coupons: d.coupons,
          operate: operate
        })
      }
    })

  },
  goSelect: function (e) {
    var _this = this

    var index = e.currentTarget.dataset.replyIndex //对应页面data-reply-index
    var coupon = _this.data.coupons[index]
    var operate = _this.data.operate

    if (operate == 1) {


      if (coupon.faceType == 5) {

        wx.navigateTo({
          url: '/pages/mycoupondetails/mycoupondetails?id=' + coupon.id,
          success: function (res) {
            // success
          },
        })

      } else {

        pageMain.mainTabBarSwitch(0)
      }


    } else if (operate == 2) {
      if (!coupon.canSelected) {
        toast.show({
          title: '抱歉，不能使用该优惠券'
        })
        return
      }
      var couponIds = []
      if (coupon.isSelected) {
        coupon.isSelected = false
      } else {
        coupon.isSelected = true
      }

      if (coupon.isSelected) {
        couponIds.push(coupon.id)
      }

      console.log(">>>" + JSON.stringify(couponIds))
      var pages = getCurrentPages();
      var prevPage = pages[pages.length - 2];

      if (_this.data.op_faceTypes == '1,2') {
        var coupon = prevPage.data.couponByShop
        coupon.selectedCouponIds = couponIds.length > 0 ? couponIds[0] : ['']
        prevPage.setData({
          couponIdsByShop: couponIds.length > 0 ? [couponIds[0]] : [''],
          couponByShop: coupon
        })
      } else if (_this.data.op_faceTypes == '3') {
        var coupon = prevPage.data.couponByRent
        coupon.selectedCouponIds = couponIds
        prevPage.setData({
          couponIdByRent: couponIds.length > 0 ? couponIds[0] : '',
          couponByRent: coupon
        })
      } else if (_this.data.op_faceTypes == '4') {
        var coupon = prevPage.data.couponByDeposit
        coupon.selectedCouponIds = couponIds
        console.log('couponIds>>' + couponIds)
        prevPage.setData({
          couponIdByDeposit: couponIds.length > 0 ? couponIds[0] : '',
          couponByDeposit: coupon
        })
      }

      prevPage.getConfirmData()

      wx.navigateBack()
    }
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