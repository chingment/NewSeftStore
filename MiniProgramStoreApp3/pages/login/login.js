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
    userInfoEp: {
      code: '',
      iv: '',
      encryptedData: ''
    },
    phoneNumberEp: {
      session_key: '',
      iv: '',
      encryptedData: ''
    }

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
    // wx.getSetting({
    //   success(res) {
    //     if (res.authSetting['scope.userInfo']) {
    //       _this.setData({
    //         isAuthUserInfo: true
    //       })
    //     }
    //   }
    // })

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
  loginByMinProgram: function (openId, userInfoEp, phoneNumberEp) {
    var _this = this
    apiOwn.loginByMinProgram({
      appId: _this.data.appId,
      merchId: storeage.getMerchId(),
      openId: openId,
      userInfoEp: userInfoEp,
      phoneNumberEp: phoneNumberEp
    }).then(function (res) {
      if (res.result == 1) {

        storeage.setOpenId(res.data.openId);
        storeage.setAccessToken(res.data.token);
        wx.reLaunch({ //关闭所有页面，打开到应用内的某个页面
          url: ownRequest.getReturnUrl()
        })
      } else {

        if (res.code == 2405) {

          _this.setData({
            isAuthUserInfo: true
          })

        } else {

          toast.show({
            title: res.message
          })
        }
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
            _this.loginByMinProgram(storeage.getOpenId(), {
              code: res.code,
              iv: e.detail.iv,
              encryptedData: e.detail.encryptedData
            }, null)
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
      _this.loginByMinProgram(storeage.getOpenId(), null, {
        encryptedData: e.detail.encryptedData,
        iv: e.detail.iv,
        session_key: storeage.getSessionKey(),
      })

    } else {
      toast.show({
        title: '您点击了拒绝授权登录！'
      })
    }
  },
  clickToRefuse: function () {

    wx.navigateBack({
      delta: 1,
    })
  }
})