const ownRequest = require('../../own/ownRequest.js')
const apiPersonal = require('../../api/personal.js')
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
    isOnLoad:false,
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
            if (res.result == 1) {
              var d = res.data
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
      var _this = this
      console.log("personal.onShow")

      if(!_this.data.isOnLoad){
        _this.setData({isOnLoad:true})
        _this.getPageData()
      }
     
        _this.setData({
          isLogin:ownRequest.isLogin()
        })
      
    }
  }
})