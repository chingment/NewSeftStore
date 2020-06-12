const ownRequest = require('../../own/ownRequest.js')
const apiPersonal = require('../../api/personal.js')
Component({
  options: {
    addGlobalClass: true,
    multipleSlots: true // 在组件定义时的选项中启用多slot支持
  },
  properties: {
    initdata: {
      type: Object,
      observer: function (newVal, oldVal, changedPath) {
        console.log("personal.initData")
        var _this = this
        _this.getPageData()
      }
    },
    height: {
      type: Number
    }
  },
  data: {
    isLogin:false
  },
  methods: {
    goLogin: function (e) {
      ownRequest.goLogin()
    },
    navigateToClick: function (e) {
      var ischecklogin = e.currentTarget.dataset.ischecklogin
      if (ischecklogin == "true") {
        if (!ownRequest.isLogin()) {
          ownRequest.goLogin()
          return
        }
      }

      var url = e.currentTarget.dataset.url
      wx.navigateTo({
        url: url
      })
    },
    getPageData: function (e) {
      var _this = this
      if (ownRequest.getCurrentStoreId() != undefined) {
        apiPersonal.pageData(null, {
          success: function (res) {
            console.log("ab.2")
            if (res.result == 1) {
              var d = res.data
              console.log("ab.1")
              _this.setData({
                isLogin: ownRequest.isLogin(),
                userInfo: d.userInfo
              })

            }
          }
        })
      }
    },
    onShow() {
      console.log("personal.onShow")
    }
  }
})