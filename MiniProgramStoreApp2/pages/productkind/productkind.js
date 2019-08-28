Component({
  options: {
    addGlobalClass: true,
    multipleSlots: true // 在组件定义时的选项中启用多slot支持
  },
  properties: {
    initdata: {
      type: Object,
      observer: function (newVal, oldVal, changedPath) {

        var _self = this
        _self.setData(newVal)

      _self.red()

      }
    },
    height: {
      type: Number
    }
  },
  data: {
    contentActive: '', // 内容栏id
    navActive: 0, // 导航栏选中id
    heightArr: [],
    containerH: 0,
  },
  ready() {  
    
  },
  methods: {
    red(e){
      console.log("ssss")
      setTimeout(function () {
      let query = wx.createSelectorQuery().in(this);
      let heightArr = [];
      let s = 0;
      query.selectAll('.pesticide').boundingClientRect((react) => {
        react.forEach((res) => {
          console.log("ssss1:" + res.height)
          s += res.height;
          console.log("ssss2:" + s)
          heightArr.push(s)
        });
        this.setData({
          heightArr: heightArr
        })
      }).exec();
      },1000)
    },
    itemClick(e) {
      console.log('kindBarItemClick');
      var _self = this
      var index = e.currentTarget.dataset.replyIndex //对应页面data-reply-index
      console.log('kindBarItemClick.index' + index)
      var list = _self.data.list;
      for (var i = 0; i < list.length; i++) {
        if (i == index) {
          list[i].selected = true
        } else {
          list[i].selected = false
        }
      }
      _self.data.list = list
      this.setData(_self.data)
    },
    onScroll(e) {
      let scrollTop = e.detail.scrollTop;
      let scrollArr = this.data.heightArr;
      console.log("i1:" + scrollTop)
      console.log("containerH:" + this.data.containerH)
      console.log("scrollArr:" + JSON.stringify(scrollArr))
      console.log("i2:" + (scrollArr[scrollArr.length - 1] - this.data.containerH))
      if (scrollTop >= scrollArr[scrollArr.length - 1] - this.data.containerH) {
        return
      } else {
        for (let i = 0; i < scrollArr.length; i++) {
          if (scrollTop >= 0 && scrollTop < scrollArr[0]) {
            this.setData({
              navActive: 0
            })
          } else if (scrollTop >= scrollArr[i - 1] && scrollTop < scrollArr[i]) {
            console.log("i:"+i)
            this.setData({
              navActive: i
            })
          }
        }
      }
    },
  }
})