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

  }
})