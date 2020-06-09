const apiSearch = require('../../api/search.js')
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')

Page({

  /**
   * 页面的初始数据
   */
  data: {
    navH:40,
    statusBarHeight:0,
    searchInputFocus: false,
    searchResult:{
      productSkus:[]
    }
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
   var _this=this
    wx.getSystemInfo({
      success: res => {
        //导航高度
        var statusBarHeight=res.statusBarHeight
        var navHeight = statusBarHeight + 46
     
        _this.data.statusBarHeight=statusBarHeight
        _this.data.navH=navHeight

        _this.setData( _this.data)
      }, fail(err) {
        console.log(err);
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
  navGoBackClick :function(){
    wx.navigateBack({
      complete: (res) => {},
    })
  },
  bindKeyInput: function (e) {
    var _this=this
    var val = e.detail.value
    console.log(val)

    apiSearch.tobeSearch({
      storeId:ownRequest.getCurrentStoreId(),
      key: val
    }, {
      success: function(res) {
        if (res.result == 1) {
          _this.data.searchResult=res.data
          _this.setData(_this.data)
        }
      },
      fail: function() {}
    })

  }
})