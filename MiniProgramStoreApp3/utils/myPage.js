const apiGlobal = require('../api/global.js')
const util = require('../utils/util.js')
let data
// 这里data从app.js中传入，为了获取data的一些参数
function init(_data) {
  data = _data
  // 重写page函数，增加阿里云监控和日志记录
  let oldPage = Page
  Page = function (obj) {
    // 重写onShow方法，用一个变量保存旧的onShow函数
    // let oldOnLoad= obj.onLoad

    // console.log('oldOnLoad:'+obj.onLoad)

    // obj.onLoad = function (e) {
    //   console.log("==>parent.onLoad==");
    //   // 此处不能写成oldOnShow()，否则没有this，this.setData等方法为undefined。这里的this在Page构造函数实例化的时候才会指定
    //   // 在Page构造函数实例化的时候，小程序会将当前的Page对象的原型链（__proto__）增加很多方法，例如setData。当前的obj没有setData
    //   // 上面一段是我猜的
    //   oldOnLoad.call(this,e)
    // }


    let oldOnShow = obj.onShow

    console.log('oldOnShow:' + obj.onShow)

    obj.onShow = function () {
      console.log("==>parent.onShow==");

      this.setData({
        start_time: new Date()
      })
      apiGlobal.byPoint(this.data.tag, 'browse_page', {
        starTime: util.formatTime(new Date())
      })

      oldOnShow.call(this)
    }


    // 重写onHide方法，用一个变量保存旧的onHide函数
    let oldOnUnload = obj.onUnload

    console.log('oldOnUnload:' + obj.oldOnUnload)

    obj.onUnload = function () {
      console.log("==>parent.onUnload==");

      const page_stay_time = (new Date() - this.data.start_time) / 1000;

      apiGlobal.byPoint(this.data.tag, 'browse_page', {
        starTime: util.formatTime(new Date(this.data.start_time)),
        endtTime: util.formatTime(new Date()),
        staySecond: page_stay_time
      })

      // 此处不能写成oldOnHide()，否则没有this，this.setData等方法为undefined。这里的this在Page对象实例化的时候才会指定
      oldOnUnload.call(this)
    }

    return oldPage(obj)
  }
}

module.exports = {
  init: init
}