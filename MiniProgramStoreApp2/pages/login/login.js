const config = require('../../config')
const ownRequest = require('../../own/ownRequest.js')
const storeage = require('../../utils/storeageutil.js')
const apiOwn = require('../../api/own.js')
const toast = require('../../utils/showtoastutil')

// pages/login/login.js
Page({

  /**
   * 页面的初始数据
   */
  data: {

  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function(options) {

  },

  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady: function() {

  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function() {


  },


  /**
   * 生命周期函数--监听页面隐藏
   */
  onHide: function() {

  },

  /**
   * 生命周期函数--监听页面卸载
   */
  onUnload: function() {

  },

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh: function() {

  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function() {

  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function() {

  },


  bindgetuserinfo: function(e) {
    var pages = getCurrentPages() //获取加载的页面
    var currentPage = pages[pages.length - 2] //获取当前页面的对象
    var url = currentPage.route //当前页面ur
    console.log("url:" + url)

    ownRequest.login((params) => {
      apiOwn.loginByMinProgram({
        merchId: config.merchId,
        appId: config.appId,
        code: params.code,
        iv: params.iv,
        encryptedData: params.encryptedData
      }, {
        success: function(res) {
          if (res.result == 1) {
            storeage.setAccessToken(res.data.token);
            console.log("token:" + storeage.getAccessToken())
            wx.reLaunch({ //关闭所有页面，打开到应用内的某个页面
              url: ownRequest.getReturnUrl()
            })
          } else {
            toast.show({
              title: res.message
            })
          }
        },
        fail: function() {}
      })
    })
  }
})