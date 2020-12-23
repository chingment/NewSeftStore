const ownRequest = require('../../own/ownRequest.js')
const apiPersonal = require('../../api/personal.js')
const storeageutil = require('../../utils/storeageutil.js')
const app = getApp()
Component({
  options: {
    addGlobalClass: true,
    multipleSlots: true // 在组件定义时的选项中启用多slot支持
  },
  properties: {
    height: {
      type: Number
    }
  },
  data: {
    tag: "personal",
    isOnReady: false,
    isLogin: false,
    userInfo: {},
    badgeByWaitPayOrders: {}
  },
  methods: {
    goLogin: function (e) {
      ownRequest.goLogin()
    },
    navigateToClick: function (e) {
      var url = e.currentTarget.dataset.url
      var ischecklogin = e.currentTarget.dataset.ischecklogin
      if (ischecklogin == "true") {
        if (!ownRequest.isLogin()) {
          ownRequest.goLogin(url)
          return
        }
      }


      wx.navigateTo({
        url: url
      })
    },
    getPageData: function (e) {
      var _this = this
      if (ownRequest.getCurrentStoreId() != undefined) {
        apiPersonal.pageData({
          openId: storeageutil.getOpenId()
        }).then(function (res) {
          if (res.result == 1) {
            var d = res.data
            _this.setData({
              isLogin: ownRequest.isLogin(),
              userInfo: d.userInfo,
              badgeByWaitPayOrders:d.badgeByWaitPayOrders
            })
          }
        })
      }
    },
    onReady: function () {
      var _this = this
      console.log("personal.onReady")
    },
    onShow() {
      var _this = this
      console.log("personal.onShow")
      _this.setData({
        isLogin: ownRequest.isLogin()
      })
      _this.getPageData()
    }
  }
})