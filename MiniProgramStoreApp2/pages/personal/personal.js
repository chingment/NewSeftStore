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
        var _self = this
        _self.setData({
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
    orderStatuItemClick: function(e) {

      if (ownRequest.isLogin()) {
        var url = e.currentTarget.dataset.url
        wx.navigateTo({
          url: url
        })
      } else {
        ownRequest.goLogin()
      }
    },
    navItemClick: function(e) {

      if (ownRequest.isLogin()) {
        var url = e.currentTarget.dataset.url
        wx.navigateTo({
          url: url
        })
      } else {
        ownRequest.goLogin()
      }
    },
  }
})