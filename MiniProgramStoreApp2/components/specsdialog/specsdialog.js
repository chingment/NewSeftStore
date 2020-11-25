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
        //console.log("newVal:" + newVal + ",oldVal:" + oldVal)
        if (newVal) {
          this._dialogOpen()
        }
        else {
          this._dialogClose();
        }
      }
    },
    productSku: {
      type: Object,
      value: null,
      observer: function (newVal, oldVal) {
        var _this = this
        //console.log("newVal:" + JSON.stringify(newVal))
        //console.log("oldVal:" + JSON.stringify(oldVal))
        var productSku = newVal
        var specIdxSkus = productSku.specIdxSkus
        var specShopItemInfo = {}
        var specItems = productSku.specItems
        for (var i in specIdxSkus) {
          specShopItemInfo[specIdxSkus[i].specIdx] = specIdxSkus[i];
        }

        //console.log("specShopItemInfo=>"+JSON.stringify(specShopItemInfo))

        for (var i = 0; i < specItems.length; i++) {
          for (var o = 0; o < specItems[i].value.length; o++) {
            specItems[i].value[o].isShow = true
          }
        }
        productSku.specItem = specItems
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
    myStop: true,
    mySpecinfoHeight: 100,
    myQuantity: 1
  },
  methods: {
    _dialogOpen: function (e) {
      var _this = this;

      if (!_this.data.myStop)
        return


      var animation = wx.createAnimation({
        duration: 300,
        timingFunction: 'linear'
      })

      animation.translateY(500).step()
      _this.setData({
        myAnimationData: animation.export(),
        isShow: true,
        myShow: true,
        myStop: false
      })

      setTimeout(function () {
        animation.translateY(0).step()
        _this.setData({
          myAnimationData: animation.export()
        })
      }, 100)

    },
    _dialogClose: function (e) {
      var _this = this;
      var animation = wx.createAnimation({
        duration: 300,
        timingFunction: 'linear'
      })
     
      animation.translateY(500).step()
      _this.setData({
        myStop:true,
        myAnimationData: animation.export()
      });

      setTimeout(function () {
        _this.setData({
          myShow: false,
          isShow: false
        })
      }, 300)
    },
    _qtyOperate: function (e) {
      var _this = this
      var operate = parseInt(e.currentTarget.dataset.replyOperate)
      var quantity= _this.data.myQuantity
      if (operate == 1) {
        if (quantity > 1) {
          quantity -= 1;
        }
      }
      else {
        quantity += 1;
      }
      _this.setData({ myQuantity: quantity })
    },
    _specValueSelect:function(e) {
      var _this = this;
      var myProductSku = _this.data.myProductSku

      var specItemIndex = e.currentTarget.dataset.specitemindex
      var specItemValueIndex = e.currentTarget.dataset.specitemvalueindex
      var specItemValueName = e.currentTarget.dataset.specitemvaluename
      var mySpecSubIndex = _this.data.mySpecSubIndex

      var mySpecShopItemInfo = _this.data.mySpecShopItemInfo
      if (mySpecSubIndex[specItemIndex] != specItemValueName) {
        mySpecSubIndex[specItemIndex] = specItemValueName;
      } 

      var selSpec = mySpecShopItemInfo[mySpecSubIndex];

      //选中修改当前SKU.ID
      if (selSpec) {
        
        apiProduct.skuStockInfo({
          storeId: _this.data.storeId,
          skuId: selSpec.skuId,
          shopMode: _this.data.shopMode
        }).then(function (res) {
          
          if (res.result == 1) {
           
            var d = res.data
            myProductSku.id = d.skuId;
            myProductSku.specIdx = selSpec.specIdx;
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
              productSku: myProductSku
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
          _this._dialogClose()
        }
        else {
          toast.show({
            title: res.message
          })
        }
      })
    }
  }
})