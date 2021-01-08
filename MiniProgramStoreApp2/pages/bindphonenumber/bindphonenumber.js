const config = require('../../config')
const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const apiOwn = require('../../api/own.js')
const app = getApp()
Page({
  data: {
    appId: '',
    tag: "bindphonenumber",
    returnUrl: '',
  },
  onLoad: function (options) {
    var _this = this
    var returnUrl = typeof options.returnUrl == 'undefined' ? '' : options.returnUrl

    const accountInfo = wx.getAccountInfoSync()

    _this.setData({
      appId: accountInfo.miniProgram.appId,
      returnUrl: returnUrl
    })

    if (!ownRequest.isLogin()) {
      ownRequest.goLogin()
      return
    }
  },
  onReady: function () {},
  onUnload: function () {

  },
  onShow: function () {

  },
  bindPhoneNumberByWx: function (openId, phoneNumber) {
    var _this = this

    apiOwn.bindPhoneNumberByWx({
      appId: _this.data.appId,
      merchId: storeage.getMerchId(),
      openId: openId,
      phoneNumber: phoneNumber,
    }).then(function (res) {


      toast.show({
        title: res.message
      })

      if (res.result == 1) {
        wx.reLaunch({ //关闭所有页面，打开到应用内的某个页面
          url: '/pages/memberprom/memberprom'
        })
      }
    })
  },
  bindgetphonenumber: function (e) {
    var _this = this


    if (e.detail.errMsg == "getPhoneNumber:ok") {
      apiOwn.wxPhoneNumber({
        encryptedData: e.detail.encryptedData,
        iv: e.detail.iv,
        session_key: storeage.getSessionKey(),
      }).then(function (res) {

        if (res.result == 1) {
          var d = res.data
          _this.bindPhoneNumberByWx(storeage.getOpenId(), d.phoneNumber)
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