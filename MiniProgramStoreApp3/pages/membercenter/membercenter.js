const toast = require('../../utils/toastutil')
const ownRequest = require('../../own/ownRequest.js')
const config = require('../../config')
const apiMember = require('../../api/member.js')
const skeletonData = require('./skeletonData')
const util = require('../../utils/util')
const storeage = require('../../utils/storeageutil.js')

const app = getApp()

Page({

  /**
   * 页面的初始数据
   */
  data: {
    tag: "membercenter",
    skeletonLoadingTypes: ['spin', 'chiaroscuro', 'shine', 'null'],
    skeletonSelectedLoadingType: 'shine',
    skeletonIsDev: false,
    skeletonBgcolor: '#FFF',
    skeletonData,
    pageIsReady: false,
    navH: 40,
    statusBarHeight: 0,
    curlevelSt: 1,
    levelSt1: null,
    levelSt2: null,
    isMember: false,
    isOptSaleOutlet: false,
    curSaleOutlet: {
      id: '',
      tagName: '',
      tagTip: '',
      contentBm: '',
      contentSm: ''
    },
    saleOutletDialog: {
      isShow: false,
      dataS: {}
    }
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
      
      }
    })

    apiMember.getPayLevelSt({
      saleOutletId: storeage.getLastSaleOutletId()
    }).then(function (res) {
      if (res.result == 1) {

        var d = res.data

        _this.setData({
          levelSt1: d.levelSt1,
          levelSt2: d.levelSt2,
          curlevelSt:d.curlevelSt,
          isOptSaleOutlet: d.isOptSaleOutlet,
          curSaleOutlet: d.curSaleOutlet,
          pageIsReady: true
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
  clickToPurchase: function (e) {

    var _this = this

    var _data = _this.data
    if (!ownRequest.isLogin()) {
      ownRequest.goLogin()
      return
    }

    if (_data.isOptSaleOutlet) {
      if (util.isEmptyOrNull(_data.curSaleOutlet.id)) {
        toast.show({
          title: '请选择服务网点'
        })
        return
      }
    }

    var curFeeSt
    if (_data.curlevelSt == 1) {
  
      curFeeSt = _data.levelSt1.feeSts[_data.levelSt1.curFeeStIdx]
    } else if (_this.data.curlevelSt == 2) {
      curFeeSt = _data.levelSt2.feeSts[_data.levelSt2.curFeeStIdx]
    }

    var skuId = curFeeSt.id //对应页面data-reply-index
    var skus = []
    skus.push({
      cartId: 0,
      id: skuId,
      quantity: 1,
      shopMode: 1,
      shopMethod:3,
      shopId:'0'
    })
    wx.navigateTo({
      url: '/pages/orderconfirm/orderconfirm?skus=' +  encodeURIComponent(JSON.stringify(skus))+ "&shopMethod=3&action=memberfee&saleOutletId=" + _data.curSaleOutlet.id,
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
    var isStop = e.currentTarget.dataset.replyIsstop
    
    if(isStop)
    return

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
  },
  clickToOpenSaleOutletDialog: function (e) {
    ("goCart")
    var _this = this
    var saleOutletDialog = _this.data.saleOutletDialog
    saleOutletDialog.isShow = true
    saleOutletDialog.dataS.curSaleOutletId = _this.data.curSaleOutlet.id
    _this.setData({
      saleOutletDialog: saleOutletDialog
    })
  },
  selectSaleoutletItem: function (e) {
    var _this = this
    var saleOutlet = e.detail.saleOutlet

    var curSaleOutlet = _this.data.curSaleOutlet
    curSaleOutlet.id = saleOutlet.id
    curSaleOutlet.contentBm = saleOutlet.name
    curSaleOutlet.contentSm = saleOutlet.contactAddress
    _this.setData({
      curSaleOutlet: curSaleOutlet
    })

    storeage.setLastSaleOutletId(saleOutlet.id)
  }
})