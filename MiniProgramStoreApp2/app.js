//app.js

const config = require('/config')
const toast = require('/utils/toastutil')
const storeage = require('/utils/storeageutil.js')
const ownRequest = require('/own/ownRequest.js')
const apiOwn = require('/api/own.js')

App({
  onLaunch: function () {
    var _this = this
    console.log('app.onLaunch')
    _this.autoUpdate()
    // 展示本地存储能力
    var logs = wx.getStorageSync('logs') || []
    logs.unshift(Date.now())
    wx.setStorageSync('logs', logs)


    wx.onNetworkStatusChange(function (res) {
      console.log(res)
      if (!res.isConnected) {

      }
    })

  },
  onShow: function () {
    var _this = this
    console.log('app.onShow')

    wx.getNetworkType({
      success: function (res) {
        console.log(res)
      }
    })

    const accountInfo = wx.getAccountInfoSync()
    console.log(accountInfo.miniProgram.appId)

    var appId = accountInfo.miniProgram.appId
    wx.login({
      success: function (res) {
        console.log("minProgram:login")
        if (res.code) {
          console.log(res)
          apiOwn.WxApiCode2Session({
            appId: appId,
            code: res.code,
          }).then(function (res2) {
            console.log(res2)
            if (res2.result == 1) {
              var d = res2.data

              storeage.setOpenId(d.openid)
              storeage.setSessionKey(d.session_key)
              storeage.setMerchId(d.merchid)
              storeage.setStoreId(d.storeid)//指定设置单店铺模式，为NULL 多店铺模式
              storeage.setCurrentStoreId(d.storeid) //设置当前店铺
            } else {
              toast.show({
                title: res2.message
              })
            }
          })
        }
      }
    });
  },
  globalData: {
    // openid: null,
    // merchid: null,
    // storeid: null,
    // session_key: null,
    userInfo: null,
    //mainTabBarIndex: 0,
    currentShopMode: 0,
    skeletonPage: null
  },
  // mainTabBarSetNumber(index, num) {
  //   var pages = getCurrentPages();
  //   for (var i = 0; i < pages.length; i++) {
  //     if (pages[i].data.tag == "main") {
  //       pages[i].data.tabBar[index].number = num
  //       pages[i].setData({
  //         tabBar: pages[i].data.tabBar
  //       })
  //       break;
  //     }
  //   }
  // },
  // mainTabBarSwitch: function (index) {
  //   var _this = this

  //   var pages = getCurrentPages();
  //   var isHasMain = false;
  //   for (var i = 0; i < pages.length; i++) {
  //     if (pages[i].data.tag == "main") {
  //       isHasMain = true
  //       var tabBar = pages[i].data.tabBar;

  //       for (var j = 0; j < tabBar.length; j++) {
  //         if (j == index) {
  //           tabBar[j].selected = true
  //           var s = tabBar[j];
  //           setTimeout(function () {
  //             wx.setNavigationBarTitle({
  //               title: s.navTitle
  //             })

  //           }, 1)

  //           _this.globalData.mainTabBarIndex = index

  //         } else {
  //           tabBar[j].selected = false
  //         }
  //       }

  //       let cp = pages[i].selectComponent('#' + tabBar[index].id);
  //       cp.onReady()

  //       pages[i].setData({
  //         tabBar: tabBar
  //       })

  //       cp = pages[i].selectComponent('#' + tabBar[index].id);
  //       cp.onShow()
  //       break
  //     }
  //   }

  //   if (isHasMain) {
  //     if (pages.length > 1) {
  //       wx.navigateBack({
  //         delta: pages.length
  //       })
  //     }
  //   }
  //   else {
  //     wx.reLaunch({
  //       url: '/pages/main/main'
  //     })
  //   }
  // },
  autoUpdate: function () {
    console.log(new Date())
    var self = this
    // 获取小程序更新机制兼容
    if (wx.canIUse('getUpdateManager')) {
      const updateManager = wx.getUpdateManager()
      //1. 检查小程序是否有新版本发布
      updateManager.onCheckForUpdate(function (res) {
        // 请求完新版本信息的回调
        if (res.hasUpdate) {
          //2. 小程序有新版本，则静默下载新版本，做好更新准备
          updateManager.onUpdateReady(function () {
            console.log(new Date())
            wx.showModal({
              title: '更新提示',
              content: '新版本已经准备好，是否重启应用？',
              success: function (res) {
                if (res.confirm) {
                  //3. 新的版本已经下载好，调用 applyUpdate 应用新版本并重启
                  updateManager.applyUpdate()
                } else if (res.cancel) {
                  //如果需要强制更新，则给出二次弹窗，如果不需要，则这里的代码都可以删掉了
                  wx.showModal({
                    title: '温馨提示~',
                    content: '本次版本更新涉及到新的功能添加，旧版本无法正常访问的哦~',
                    success: function (res) {
                      self.autoUpdate()
                      return;
                      //第二次提示后，强制更新                      
                      if (res.confirm) {
                        // 新的版本已经下载好，调用 applyUpdate 应用新版本并重启
                        updateManager.applyUpdate()
                      } else if (res.cancel) {
                        //重新回到版本更新提示
                        self.autoUpdate()
                      }
                    }
                  })
                }
              }
            })
          })
          updateManager.onUpdateFailed(function () {
            // 新的版本下载失败
            wx.showModal({
              title: '已经有新版本了哟~',
              content: '新版本已经上线啦~，请您删除当前小程序，重新搜索打开哟~',
            })
          })
        }
      })
    } else {
      // 如果希望用户在最新版本的客户端上体验您的小程序，可以这样子提示
      wx.showModal({
        title: '提示',
        content: '当前微信版本过低，无法使用该功能，请升级到最新微信版本后重试。'
      })
    }
  }
})