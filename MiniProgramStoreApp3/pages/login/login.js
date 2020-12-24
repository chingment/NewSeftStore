const config = require('../../config')
const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const apiOwn = require('../../api/own.js')
const app = getApp()
Page({
  data: {
    appId: '',
    tag: "login",
    isAuthUserInfo: false,
    returnUrl: '',
    code: ''
  },
  onLoad: function (options) {
    var _this = this

    var accountInfo = wx.getAccountInfoSync()
    var appId = accountInfo.miniProgram.appId
    var returnUrl = typeof options.returnUrl == 'undefined' ? '' : options.returnUrl
    _this.setData({
      appId: appId,
      returnUrl: returnUrl
    })

    // 查看是否授权
    wx.getSetting({
      success(res) {
        if (res.authSetting['scope.userInfo']) {
          _this.setData({
            isAuthUserInfo: true
          })
        }
      }
    })

    wx.checkSession({
      success() {
        console.log('session 未过期')
      },
      fail() {
        console.log('session 已过期')
      }
    })

    wx.login({
      success(res) {
        console.log('=>>login.code3:' + res.code)
      },
      fail() {

      }
    })

  },
  onReady: function () {},
  onShow: function () {},
  loginByMinProgram: function (openId, code, iv, encryptedData) {
    var _this = this
    apiOwn.loginByMinProgram({
      appId: _this.data.appId,
      merchId: storeage.getMerchId(),
      openId: openId,
      code: code,
      iv: iv,
      encryptedData: encryptedData
    }).then(function (res) {
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
  bindgetuserinfo: function (e) {
    var _this = this

    if (e.detail.userInfo) {
      wx.login({
        success(res) {
          console.log('=>>login.code2:' + res.code)

          if (res.code) {
            _this.loginByMinProgram(storeage.getOpenId(), res.code, e.detail.iv, e.detail.encryptedData)
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

  },
  bindgetphonenumber: function (e) {
    var _this = this
    console.log(e)
    if (e.detail.errMsg == "getPhoneNumber:ok") {
      apiOwn.wxPhoneNumber({
        encryptedData: e.detail.encryptedData,
        iv: e.detail.iv,
        session_key: storeage.getSessionKey(),
      }).then(function (res) {
        console.log(res)
        if (res.result == 1) {
          var d = res.data
        } else {
          toast.show({
            title: res.message
          })
        }
      })
    } else {
      toast.show({
        title: '您点击了拒绝授权登录！'
      })
    }
  }
})