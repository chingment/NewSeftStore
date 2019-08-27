
function getAuthorize(options) {
  console.log("wxutil.getAuthorize")
  var _success = options.success
  var _fail = options.fail
  wx.getUserInfo({
    openIdList: ['selfOpenId'],
    lang: 'zh_CN',
    success: res => {
      console.log('wxutil.success->确定授权')
      var userInfo = res.userInfo
      console.log("can get userInfo:" + JSON.stringify(userInfo))

      _success(res)
    },
    fail: res => {
      console.log('wxutil.fail->拒绝授权')
      _fail(res)
      // wx.showModal({
      //   title: '警告',
      //   content: '您点击了拒绝授权,将无法正常显示个人信息,点击确定重新获取授权。',
      //   success: function (res) {
      //     if (res.confirm) {
      //       wx.openSetting({
      //         success: (res) => {
      //           if (res.authSetting["scope.userInfo"]) {////如果用户重新同意了授权登录
      //             wx.getUserInfo({
      //               success: function (res) {
      //                 console.log('wxutil.success->重新确定授权')
      //                 _success(res)
      //               }
      //             })
      //           }
      //         }, fail: function (res) {
      //           console.log('wxutil.fail->重新取消授权')
      //           _fail(res)
      //         }
      //       })
      //     }
      //     else {
      //       console.log('wxutil.fail->重新取消授权')
      //       _fail(res)
      //     }
      //   }
      // })
    }
  })
}

module.exports.getAuthorize = getAuthorize; 