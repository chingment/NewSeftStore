const config = require('../../config')
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const lumos = require('../../utils/lumos.minprogram.js')
const app = getApp()

Page({
  data: {
    result: {
      isComplete: false,
      message: "正在处理，请耐心等候"
    }
  },
  getResult: function (id, tp, caller) {

    var _this = this;
    lumos.getJson({
      url: config.apiUrl.operateGetResult,
      urlParams: {
        id: id,
        type: tp,
        caller: caller
      },
      isShowLoading: false,
      success: function (res) {
        if (res.data.isComplete) {
          clearInterval(_this.countDown);
        }
        _this.setData({
          result: res.data
        })
      }
    })
  },

  onLoad: function (options) {
    var _this = this
    var _id = options.id == undefined ? "" : options.id
    var _type = options.type == undefined ? "" : options.type
    var _caller = options.caller == undefined ? "" : options.caller

    console.log("operate.load()->>>id:" + _id + ",type:" + _type)


    var step = 1, //计数动画次数
      num = 0, //计数倒计时秒数（n - num）
      start = 1.5 * Math.PI, // 开始的弧度
      end = -0.5 * Math.PI, // 结束的弧度
      time = null; // 计时器容器

    var animation_interval = 1000, // 每1秒运行一次计时器
      n = 120; // 当前倒计时为10秒
    // 动画函数
    function animation() {
      if (step <= n) {
        end = end + 2 * Math.PI / n;
        ringMove(start, end);
        step++;
        _this.getResult(_id, _type, _caller)
      } else {
        clearInterval(time);
      }
    };


    var context = wx.createCanvasContext('secondCanvas')

    context.setStrokeStyle('#cccccc')
    context.beginPath()
    context.setLineWidth(2)
    context.arc(90, 90, 60, 0, 2 * Math.PI, false);
    context.stroke()
    context.closePath()

    // 画布绘画函数
    function ringMove(s, e) {


      context.setStrokeStyle('#f18d00')
      context.beginPath()
      context.setLineWidth(2)
      context.arc(90, 90, 60, 0, 2 * Math.PI, false);
      context.stroke()
      context.closePath()

      // 绘制圆环
      context.setStrokeStyle('#cccccc')
      context.beginPath()
      context.setLineWidth(2)
      context.arc(90, 90, 60, s, e, true)
      context.stroke()
      context.closePath()




      // 绘制倒计时文本
      context.beginPath()
      context.setLineWidth(1)
      context.setFontSize(25)
      context.setFillStyle('#333333')
      context.setTextAlign('center')
      context.setTextBaseline('middle')
      context.fillText(n - num + '', 90, 90, 60)
      context.fill()
      context.closePath()

      context.draw()

      // 每完成一次全程绘制就+1
      num++;
    }
    // 倒计时前先绘制整圆的圆环
    ringMove(start, end);
    // 创建倒计时m.h987yuitryuioihyhujik[jhgvfbnvnjmnbvbnm,nbvfcgklkjhg545545545u ]


    time = setInterval(animation, animation_interval);

    _this.countDown = time
  },



  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady: function () {

  },

  operate: function (e) {

    var opType = e.currentTarget.dataset.replyOptype
    var opVal = e.currentTarget.dataset.replyOpval

    console.log("opType:" + opType + ",opVal:" + opVal)
    switch (opType) {
      case "FUN":
        switch (opVal) {
          case "goHome":
            app.mainTabBarSwitch(0)
            break;
        }
        break;
      case "URL":
        wx.redirectTo({
          url: opVal
        })
        break;
    }
  }

})