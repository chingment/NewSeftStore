// pages/memberprom/memberprom.js
const skeletonData = require('./skeletonData')
const apiMember = require('../../api/member.js')
const storeage = require('../../utils/storeageutil.js')
const app = getApp()

Page({

  /**
   * 页面的初始数据
   */
  data: {
    tag: "memberprom",
    skeletonLoadingTypes: ['spin', 'chiaroscuro', 'shine', 'null'],
    skeletonSelectedLoadingType: 'shine',
    skeletonIsDev: false,
    skeletonBgcolor: '#FFF',
    skeletonData,
    pageIsReady: false,
    isShowButtonBottom: false,
    timespan: (new Date()).getTime(),
    userInfo: {},
    isOpenMemberRight: false
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var _this = this

    var reffSign = options.reffSign == undefined ? '' : options.reffSign

    storeage.setReffSign(reffSign)

    if (app.globalData.checkConfig) {
      console.log('a1')

      _this.getPromSt()
    } else {
      console.log('a2')
      app.checkConfigReadyCallback = res => {
        console.log('a3')
        _this.getPromSt()
      }
    }

  },
  getPromSt: function () {
    var _this = this
    apiMember.getPromSt({
      openId: storeage.getOpenId()
    }).then(function (res) {
      if (res.result == 1) {

        var d = res.data
        _this.setData({
          pageIsReady: true,
          userInfo: d.userInfo,
          isOpenMemberRight:d.isOpenMemberRight
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
    var _this = this
    var _data = _this.data
    // 设置转发内容
    var shareObj = {
      title: _this.data.productSku.name,
      path: '/pages/memberprom/memberprom?reffSign=' + storeage.getOpenId(), // 默认是当前页面，必须是以‘/’开头的完整路径
      imgUrl: '', //转发时显示的图片路径，支持网络和本地，不传则使用当前页默认截图。
      success: function (res) { // 转发成功之后的回调　　　　　
        if (res.errMsg == 'shareAppMessage:ok') {}
      },
      fail: function () { // 转发失败之后的回调
        if (res.errMsg == 'shareAppMessage:fail cancel') {
          // 用户取消转发
        } else if (res.errMsg == 'shareAppMessage:fail') {
          // 转发失败，其中 detail message 为详细失败信息
        }
      },
      complete: function () {
        // 转发结束之后的回调（转发成不成功都会执行）
      }
    };
    // 来自页面内的按钮的转发
    if (options.from == 'button') {


    }
    // 返回shareObj
    return shareObj;

  },
  //监听屏幕滚动 判断上下滚动  
  onPageScroll: function (ev) {
    var _this = this;
    //当滚动的top值最大或者最小时，为什么要做这一步是由于在手机实测小程序的时候会发生滚动条回弹，所以为了解决回弹，设置默认最大最小值   
    if (ev.scrollTop <= 0) {
      ev.scrollTop = 0;
    } else if (ev.scrollTop > wx.getSystemInfoSync().windowHeight) {
      ev.scrollTop = wx.getSystemInfoSync().windowHeight;
    }
    var isShowButtonBottom = false
    //判断浏览器滚动条上下滚动   
    if (ev.scrollTop > this.data.scrollTop || ev.scrollTop == wx.getSystemInfoSync().windowHeight) {
      //console.log('向下滚动');

      if (ev.scrollTop > 400) {
        isShowButtonBottom = true
      }
    } else {
      //console.log('向上滚动');
      isShowButtonBottom = false
    }
    //给scrollTop重新赋值    
    setTimeout(function () {
      _this.setData({
        scrollTop: ev.scrollTop,
        isShowButtonBottom: isShowButtonBottom
      })
    }, 0)
  },
  clickToNgItem: function (e) {
    // var ischecklogin = e.currentTarget.dataset.ischecklogin
    // if (ischecklogin == "true") {
    //   if (!ownRequest.isLogin()) {
    //     ownRequest.goLogin()
    //     return
    //   }
    // }

    var right = e.currentTarget.dataset.right
    var title = e.currentTarget.dataset.title
    wx.navigateTo({
      url: '/pages/memberrightdesc/memberrightdesc?right=' + right + '&title=' + title
    })
  },
  clickToCenter: function (e) {
    wx.navigateTo({
      url: '/pages/membercenter/membercenter'
    })
  }
})