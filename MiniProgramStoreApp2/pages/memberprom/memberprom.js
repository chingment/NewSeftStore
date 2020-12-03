// pages/memberprom/memberprom.js
Page({

  /**
   * 页面的初始数据
   */
  data: {
    isShowButtonBottom: false,
    timespan:(new Date()).getTime()
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {

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
  navigateToClick: function (e) {
    // var ischecklogin = e.currentTarget.dataset.ischecklogin
    // if (ischecklogin == "true") {
    //   if (!ownRequest.isLogin()) {
    //     ownRequest.goLogin()
    //     return
    //   }
    // }

    var right = e.currentTarget.dataset.right
    var title = e.currentTarget.dataset.tilte
    wx.navigateTo({
      url: '/pages/memberrightdesc/memberrightdesc?right='+right+'&title='
    })
  }
})