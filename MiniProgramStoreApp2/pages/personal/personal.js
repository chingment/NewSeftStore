const ownRequest = require('../../own/ownRequest.js')

Component({
  options: {
    addGlobalClass: true,
    multipleSlots: true // 在组件定义时的选项中启用多slot支持
  },
  properties: {
    initdata: {
      type: Object,
      observer: function(newVal, oldVal, changedPath) {

        var _this = this
        _this.setData({
          isLogin: ownRequest.isLogin(),
          userInfo: newVal.userInfo
        })

      }
    },
    height: {
      type: Number
    }
  },
  data: {},
  methods: {
    goLogin: function(e) {
      ownRequest.goLogin()
    },
    navigateToClick: function(e) {
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
    onShow(){
      console.log("personal.onShow")
    }
  }
})