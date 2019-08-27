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
      }
    }
  },
  data: {},
  methods: {
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
    }
  }
})