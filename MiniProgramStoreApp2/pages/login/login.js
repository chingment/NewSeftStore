const config = require('../../config')
const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const apiOwn = require('../../api/own.js')

Page({
  data: {
    isAuthUserInfo: false
  },
  onLoad: function(options) {
    var _this = this;
    // 查看是否授权
    wx.getSetting({
      success(res) {
        if (res.authSetting['scope.userInfo']) {

          _this.setData({
            isAuthUserInfo: true
          })

          // console.log("res:"+JSON.stringify(res))
          // // 已经授权，可以直接调用 getUserInfo 获取头像昵称
          // wx.getUserInfo({
          //   success: function (res) {
          //     console.log(JSON.stringify(res))
          //     //"errMsg":"getUserInfo:ok"
          //     _this.login(storeage.getOpenId(), '', res.iv, res.encryptedData)
          //     // console.log( JSON.stringify(res))
          //     // wx.reLaunch({ //关闭所有页面，打开到应用内的某个页面
          //     //   url: ownRequest.getReturnUrl()
          //     // })
          //   }
          // })
        }
      }
    })
  },
  onReady: function() {},
  onShow: function() {},
  login: function(openid, code, iv, encryptedData) {

    apiOwn.loginByMinProgram({
      merchId: config.merchId,
      appId: config.appId,
      openId: openid,
      code: code,
      iv: iv,
      encryptedData: encryptedData
    }).then(function(res) {
      if (res.result == 1) {
        storeage.setOpenId(res.data.openId);
        storeage.setAccessToken(res.data.token);
        wx.reLaunch({ //关闭所有页面，打开到应用内的某个页面
          url: ownRequest.getReturnUrl()
        })
      } else {
        toast.show({
          title: res.message
        })
      }
    })

  },
  bindgetuserinfo: function(e) {
    var _this = this
    if (_this.data.isAuthUserInfo) {
      _this.login(storeage.getOpenId(), '', e.detail.iv, e.detail.encryptedData)
    } else {
      if (e.detail.userInfo) {
        wx.login({
          success(res) {
            console.log(JSON.stringify(res))
            if (res.code) {
              _this.login('', res.code, e.detail.iv, e.detail.encryptedData)
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
  },
  bindgetphonenumber:function(e){
    var _this=this;

    console.log(e);
    if (e.detail.errMsg == "getPhoneNumber:ok") {
      _this.login('', '', e.detail.iv, e.detail.encryptedData)
    }
    else{
      toast.show({
        title: '登录失败'
      })
    }
  }
})