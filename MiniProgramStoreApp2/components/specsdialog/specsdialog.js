const toast = require('../../utils/toastutil')
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const apiCart = require('../../api/cart.js')
const apiProduct = require('../../api/product.js')

Component({
  options: {
    addGlobalClass: true
  },
  properties: {
    storeId: {
      type: String,
      value: "",
    },
    shopMode: {
      type: String,
      value: "",
    },
    isShow: {
      type: Boolean,
      value: false,
      observer: function (newVal, oldVal) {
        console.log("newVal:" + newVal + ",oldVal:" + oldVal)
        if (newVal) {
          this._open()
        }
        else {
          this._close();
        }
      }
    },
    productSku: {
      type: Object,
      value: null,
      observer: function (newVal, oldVal) {
        var _this = this

        var productSku = newVal

        var specIdxSkus = productSku.specIdxSkus

        var specShopItemInfo = {}

        var specItems = productSku.specItems

        for (var i in specIdxSkus) {
          specShopItemInfo[specIdxSkus[i].specIdx] = specIdxSkus[i]; //修改数据结构格式，改成键值对的方式，以方便和选中之后的值进行匹配
        }

        for (var i = 0; i < specItems.length; i++) {
          for (var o = 0; o < specItems[i].value.length; o++) {
            specItems[i].value[o].isShow = true
          }
        }

        productSku.specItem = specItems

        console.log("newVal:" + JSON.stringify(newVal))
        console.log("oldVal:" + JSON.stringify(oldVal))

        _this.setData({
          myQuantity: 1,
          mySpecShopItemInfo: specShopItemInfo,
          myProductSku: productSku,
          mySpecSubIndex: productSku.specIdx.split(',')
        })
      }
    }
  },
  data: {
    myAnimationData: {},
    mySpecSubIndex: null,
    myProductSku: null,
    mySpecSelectArr: [], //存放被选中的值
    mySpecShopItemInfo: {}, //存放要和选中的值进行匹配的数据
    mySpecSubIndex: [], //是否选中 因为不确定是多规格还是但规格，所以这里定义数组来判断
    mySpecBoxArr: {},
    myShow: false,
    mySpecinfoHeight: 100,
    myQuantity: 1
  },
  methods: {
    _open: function (e) {
      var _this = this;



      if (!_this.data.myShow) {
        var animation = wx.createAnimation({
          duration: 500,
          timingFunction: 'linear'
        })
        _this.animation = animation
        animation.translateY(500).step()
        _this.setData({
          myAnimationData: animation.export(),
          isShow: true,
          myShow: true
        })



        setTimeout(function () {

          const query = wx.createSelectorQuery().in(_this)
          query.selectAll('.content,.prdinfo,.qtyinfo,.bottominfo').boundingClientRect(function (rect) {
            console.log(JSON.stringify(rect))
            if (rect != null) {
              if (rect.length >= 4) {
                var height = rect[0].height - rect[1].height - rect[2].height - rect[3].height
                console.log(JSON.stringify(rect) + ".height:" + height)
                _this.setData({
                  mySpecinfoHeight: height
                })
              }
            }

          }).exec()


          animation.translateY(0).step()
          _this.setData({
            myAnimationData: animation.export()
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
        myAnimationData: animation.export()
      });

      setTimeout(function () {
        _this.setData({
          myShow: false,
          isShow: false
        })
      }, 500)
    },
    _qtyOperate: function (e) {
      var _this = this

      console.log("_qtyOperate")
      var operate = parseInt(e.currentTarget.dataset.replyOperate)

      if (operate == 1) {
        if (_this.data.myQuantity > 1) {
          _this.data.myQuantity -= 1;
          _this.setData({ myQuantity: _this.data.myQuantity })
        }
      }
      else {
        _this.data.myQuantity += 1;
        _this.setData({ myQuantity: _this.data.myQuantity })
      }

    },
    _specValueBtn(e) {
      var _this = this;
      var n = e.currentTarget.dataset.n
      var index = e.currentTarget.dataset.index
      var item = e.currentTarget.dataset.name
      var myProductSku = _this.data.myProductSku
      var mySpecSubIndex = _this.data.mySpecSubIndex
      var mySpecBoxArr = _this.data.mySpecBoxArr
      var mySpecShopItemInfo = _this.data.mySpecShopItemInfo
      if (mySpecSubIndex[n] != item) {
        mySpecSubIndex[n] = item;
        mySpecSubIndex[n] = item;
      } else {
        // self.selectArr[n] = "";
        // self.subIndex[n] = -1; //去掉选中的颜色
      }

      _this.checkItem();

      var arr = mySpecShopItemInfo[mySpecSubIndex];


      //选中修改当前SKU.ID
      if (arr) {
 
        apiProduct.skuStockInfo({
          storeId: _this.data.storeId,
          skuId: arr.skuId,
          shopMode: _this.data.shopMode
        }).then(function (res) {
      
          if (res.result == 1) {
            var d = res.data
            
            myProductSku.id = arr.skuId;
            myProductSku.specIdx = arr.specIdx;
            myProductSku.name = d.name
            myProductSku.isOffSell = d.isOffSell
            myProductSku.salePrice = d.salePrice
            myProductSku.isShowPrice = d.isShowPrice
            myProductSku.salePriceByVip = d.salePriceByVip
            myProductSku.sellQuantity = d.sellQuantity
      
            _this.setData({
              myProductSku: myProductSku,
              mySpecSubIndex: mySpecSubIndex,
              mySpecShopItemInfo: mySpecShopItemInfo
            })

            _this.triggerEvent('_updateProductSku', {
              productSku:myProductSku
            })

          }
        })
      }
    },
    _addToCart: function (e) {
      var _this = this

      if (_this.data.myProductSku.isOffSell)
        return

      var skuId = _this.data.myProductSku.id
      var productSkus = new Array();
      productSkus.push({
        id: skuId,
        quantity: _this.data.myQuantity,
        selected: true,
        shopMode: _this.data.shopMode
      });

      apiCart.operate({
        storeId: _this.data.storeId,
        operate: 2,
        productSkus: productSkus
      }).then(function (res) {

        if (res.result == 1) {
          toast.show({
            title: '加入购物车成功'
          })

          _this._close()
        }
        else {
          toast.show({
            title: res.message
          })
        }

      })

    },
    checkItem() {
      var _this = this;
      var myProductSku = _this.data.myProductSku
      var specItems = _this.data.myProductSku.specItems;
      var result = []; //定义数组存储被选中的值
      for (var i in specItems) {
        result[i] = _this.data.mySpecSelectArr[i] ? _this.data.mySpecSelectArr[i] : "";
      }

      for (var i in specItems) {
        var last = result[i]; //把选中的值存放到字符串last去
        for (var k in specItems[i].item) {
          result[i] = specItems[i].item[k].name; //赋值，存在直接覆盖，不存在往里面添加name值
          specItems[i].item[k].isShow = _this.isMay(result); //在数据里面添加字段isShow来判断是否可以选择
        }
        result[i] = last; //还原，目的是记录点下去那个值，避免下一次执行循环时避免被覆盖
      }

      myProductSku.specItems = specItems
      _this.setData({
        myProductSku: myProductSku
      })
    },
    isMay(result) {
      var _this = this
      for (var i in result) {
        if (result[i] == "") {
          return true; //如果数组里有为空的值，那直接返回true
        }
      }
      return !_this.data.mySpecShopItemInfo[result] ? false : _this.data.mySpecShopItemInfo[result].stock == 0 ? false : true; //匹配选中的数据的库存，若不为空返回true反之返回false
    }
  }
})