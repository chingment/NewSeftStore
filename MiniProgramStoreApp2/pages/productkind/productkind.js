const ownRequest = require('../../own/ownRequest.js')
const apiProduct = require('../../api/product.js')

var getList = function (_this) {
  var currentTab;
  var currentTabIndex = -1;
  for (var i = 0; i < _this.data.tabs.length; i++) {
    if (_this.data.tabs[i].selected == true) {
      currentTab = _this.data.tabs[i];
      currentTabIndex = i;
    }
  }

  if (currentTabIndex == -1) {
    currentTabIndex = 0;
    currentTab = _this.data.tabs[currentTabIndex];
  }

  var pageIndex = currentTab.pageIndex
  var kindId = currentTab.id == undefined ? "" : currentTab.id

  apiProduct.list({
    storeId: ownRequest.getCurrentStoreId(),
    pageIndex: pageIndex,
    kindId: kindId,
    name: ""
  }, {
      success: function (res) {
        if (res.result == 1) {
          var list
          if (currentTab.pageIndex == 0) {
            list = res.data
          } else {
            list = _this.data.tabs[currentTabIndex].list.concat(res.data)
          }

          _this.data.tabs[currentTabIndex].list = list;

          _this.setData({
            tabs: _this.data.tabs
          })
        }
      },
      fail: function () { }
    })

}

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
    },
    height: {
      type: Number
    }
  },
  data: {},
  methods: {
    itemClick(e) {
  
      var _self = this
      var index = e.currentTarget.dataset.replyIndex //对应页面data-reply-index
      var tabs = _self.data.tabs;
      for (var i = 0; i < tabs.length; i++) {
        if (i == index) {
          tabs[i].selected = true
        } else {
          tabs[i].selected = false
        }
      }
      _self.data.tabs = tabs
      this.setData(_self.data)
    },
     productLoadMore: function (e) {
      var _this = this
      var index = e.currentTarget.dataset.replyIndex
       console.log("productLoadMore.index:" + index)

       _this.data.tabs[index].pageIndex += 1
       _this.setData({
         tabs: _this.data.tabs
       })

       getList(_this)
    },
    productRefesh: function (e) {
      var _this = this
      var index = e.currentTarget.dataset.replyIndex

      _this.data.tabs[index].pageIndex = 0

      console.log("productLoadMore.index:" + index)
      getList(_this)
    }
  }
})