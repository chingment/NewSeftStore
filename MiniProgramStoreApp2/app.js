//app.js
App({
  onLaunch: function () {
    // 展示本地存储能力
    var logs = wx.getStorageSync('logs') || []
    logs.unshift(Date.now())
    wx.setStorageSync('logs', logs)


    // 登录
    // wx.login({
    //   success: res => {
    //     // 发送 res.code 到后台换取 openId, sessionKey, unionId
    //   }
    // })


    // 获取用户信息
    // wx.getSetting({
    //   success: res => {
    //     if (res.authSetting['scope.userInfo']) {
    //       // 已经授权，可以直接调用 getUserInfo 获取头像昵称，不会弹框
    //       wx.getUserInfo({
    //         success: res => {
    //           // 可以将 res 发送给后台解码出 unionId
    //           this.globalData.userInfo = res.userInfo

    //           // 由于 getUserInfo 是网络请求，可能会在 Page.onLoad 之后才返回
    //           // 所以此处加入 callback 以防止这种情况
    //           if (this.userInfoReadyCallback) {
    //             this.userInfoReadyCallback(res)
    //           }
    //         }
    //       })
    //     }
    //   }
    // })


  },
  onShow: function () {
    console.log("App.onShow")
  },
  globalData: {
    userInfo: null,
    mainTabBarIndex: 0,
    currentShopMode: 0,
    skeletonPage: null
  },
  mainTabBarSetNumber(index, num) {
    var pages = getCurrentPages();
    for (var i = 0; i < pages.length; i++) {
      if (pages[i].data.tag == "main") {
        pages[i].data.tabBar[index].number = num
        pages[i].setData({
          tabBar: pages[i].data.tabBar
        })
        break;
      }
    }
  },
  mainTabBarSwitch: function (index) {
    var _this = this

    var pages = getCurrentPages();
    var isHasMain = false;
    for (var i = 0; i < pages.length; i++) {
      if (pages[i].data.tag == "main") {
        isHasMain = true
        var tabBar = pages[i].data.tabBar;

        for (var j = 0; j < tabBar.length; j++) {
          if (j == index) {
            tabBar[j].selected = true
            var s = tabBar[j];
            setTimeout(function () {
              wx.setNavigationBarTitle({
                title: s.navTitle
              })

            }, 1)

            _this.globalData.mainTabBarIndex = index

          } else {
            tabBar[j].selected = false
          }
        }

        let cp = pages[i].selectComponent('#' + tabBar[index].id);
        cp.onReady()

        pages[i].setData({
          tabBar: tabBar
        })

        cp = pages[i].selectComponent('#' + tabBar[index].id);
        cp.onShow()
        break
      }
    }

    if (isHasMain) {
      if (pages.length > 1) {
        wx.navigateBack({
          delta: pages.length
        })
      }
    }
    else {
      wx.reLaunch({
        url: '/pages/main/main'
      })
    }
  }
})