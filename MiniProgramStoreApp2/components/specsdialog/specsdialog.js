Component({
  options: {
    addGlobalClass: true
  },
  properties: {
    isShow: {
      type: Boolean,
      value: false,
      observer:function(newVal,oldVal){
        console.log("newVal:"+newVal+",oldVal:"+oldVal)
        if(newVal){
          this._open()
        }
        else{
          this._close();
        }
      }
    },
    productSku:{
      type: Object,
      value:null,
      observer:function(newVal,oldVal){
        console.log("newVal:"+JSON.stringify(newVal))
        console.log("oldVal:"+JSON.stringify(oldVal))
      }
    }
  },
  data: {
    animationData: {},
    show:false
  },
  methods: {
    _open: function (e) {

      var _this = this;
      if (!_this.data.show) {
        // 创建一个动画实例
        var animation = wx.createAnimation({
          // 动画持续时间
          duration: 500,
          // 定义动画效果，当前是匀速
          timingFunction: 'linear'
        })
        // 将该变量赋值给当前动画
        _this.animation = animation
        // 先在y轴偏移，然后用step()完成一个动画
        animation.translateY(500).step()
        // 用setData改变当前动画
        _this.setData({
          // 通过export()方法导出数据
          animationData: animation.export(),
          // 改变view里面的Wx：if
          show: true,
          isShow:true
        })
        // 设置setTimeout来改变y轴偏移量，实现有感觉的滑动
        setTimeout(function () {
          animation.translateY(0).step()
          _this.setData({
            animationData: animation.export()
          })
        }, 400)
      }
    },
    _close: function (e) {
      var _this = this;
      var animation = wx.createAnimation({
        duration: 500,
        timingFunction: 'linear'
      })
      _this.animation = animation
      animation.translateY(500).step()
      _this.setData({
        animationData: animation.export()
      });

      setTimeout(function () {
        _this.setData({
          show: false,
          isShow:false
        })
      }, 500)
    },
    angle: function (start, end) {

      var _X = end.X - start.X,

        _Y = end.Y - start.Y

      //返回角度 /Math.atan()返回数字的反正切值

      return 360 * Math.atan(_Y / _X) / (2 * Math.PI);

    }
  }
})