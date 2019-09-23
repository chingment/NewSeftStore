const config = require('../../config')
const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const apiOwn = require('../../api/own.js')

Page({
  data: {
  },
  onLoad: function(options) {
    var that = this;
    // 查看是否授权
    wx.getSetting({
      success(res) {
        if (res.authSetting['scope.userInfo']) {
          // 已经授权，可以直接调用 getUserInfo 获取头像昵称
          wx.getUserInfo({
            success: function (res) {
              //"errMsg":"getUserInfo:ok"
              console.log( JSON.stringify(res))
              wx.reLaunch({ //关闭所有页面，打开到应用内的某个页面
                url: ownRequest.getReturnUrl()
              })
            }
          })
        }
      }
    })
  },
  onReady: function() {
  },
  onShow: function() {
  },
  bindgetuserinfo: function(e) {
    if (e.detail.userInfo) {
      wx.login({
        success(res) {
          if (res.code) {

            apiOwn.loginByMinProgram({
              merchId: config.merchId,
              appId: config.appId,
              code: res.code,
              iv: e.detail.iv,
              encryptedData: e.detail.encryptedData
            }, {
                success: function (res) {
                  if (res.result == 1) {
                    storeage.setAccessToken(res.data.token);
                    wx.reLaunch({ //关闭所有页面，打开到应用内的某个页面
                      url: ownRequest.getReturnUrl()
                    })
                  } else {
                    toast.show({
                      title: res.message
                    })
                  }
                },
                fail: function () { }
              })
        
          } else {
            
          }
        },
        fail() {
         
        }
      })
    } else {
      toast.show({
        title: '只有允许授权才能进行微信登录，请再次点击登录按钮'
      })
    }
  }
})