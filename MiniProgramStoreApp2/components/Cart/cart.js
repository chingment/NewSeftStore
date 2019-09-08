Component({
  options: {
    addGlobalClass: true,
    multipleSlots: true
  },
  properties: {
    show: {
      type: Boolean,
      value: false
    }
  },
  data: {
    animationData: {}
  },
  methods: {
    open: function (e) {
      // 用that取代this，防止不必要的情况发生
      var that = this;
      // 创建一个动画实例
      var animation = wx.createAnimation({
        // 动画持续时间
        duration: 300,
        // 定义动画效果，当前是匀速
        timingFunction: 'linear'
      })
      // 将该变量赋值给当前动画
      that.animation = animation
      // 先在y轴偏移，然后用step()完成一个动画
      animation.translateY(300).step()
      // 用setData改变当前动画
      that.setData({
        // 通过export()方法导出数据
        animationData: animation.export(),
        // 改变view里面的Wx：if
        show: true
      })
      // 设置setTimeout来改变y轴偏移量，实现有感觉的滑动
      setTimeout(function () {
        animation.translateY(0).step()
        that.setData({
          animationData: animation.export()
        })
      }, 200)

      this.selectComponent("#page_cart").loadData()
    },
    _close: function (e) {
      var that = this;
      var animation = wx.createAnimation({
        duration: 300,
        timingFunction: 'linear'
      })
      that.animation = animation
      animation.translateY(300).step()
      that.setData({
        animationData: animation.export()
      });
    }
  }
})