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
        if (newVal) {
          this._dialogOpen()
        }
        else {
          this._dialogClose();
        }
      }
    },
    sku: {
      type: Object,
      value: null,
      observer: function (newVal, oldVal) {
        var _this = this
        var sku = newVal
        var specIdxSkus = sku.specIdxSkus
        var specShopItemInfo = {}
        var specItems = sku.specItems
        for (var i in specIdxSkus) {
          specShopItemInfo[specIdxSkus[i].specIdx] = specIdxSkus[i];
        }

        for (var i = 0; i < specItems.length; i++) {
          for (var o = 0; o < specItems[i].value.length; o++) {
            specItems[i].value[o].isShow = true
          }
        }
        sku.specItem = specItems
        _this.setData({
          myQuantity: 1,
          mySpecShopItemInfo: specShopItemInfo,
          mySku: sku,
          mySpecSubIndex: sku.specIdx.split(',')
        })
      }
    }
  },
  data: {
    myAnimationData: {},
    mySpecSubIndex: null,
    mySku: null,
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
      var mySku = _this.data.mySku

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
            mySku.id = d.skuId;
            mySku.specIdx = selSpec.specIdx;
            mySku.name = d.name
            mySku.isOffSell = d.isOffSell
            mySku.salePrice = d.salePrice
            mySku.isShowPrice = d.isShowPrice
            mySku.salePriceByVip = d.salePriceByVip
            mySku.sellQuantity = d.sellQuantity

            _this.setData({
              mySku: mySku,
              mySpecSubIndex: mySpecSubIndex,
              mySpecShopItemInfo: mySpecShopItemInfo
            })

            _this.triggerEvent('_updateSku', {
              sku: mySku
            })

          }
        })
      }
    },
    _addToCart: function (e) {
      var _this = this
      if (_this.data.mySku.isOffSell)
        return

      var skuId = _this.data.mySku.id
      var skus = new Array();
      skus.push({
        id: skuId,
        quantity: _this.data.myQuantity,
        selected: true,
        shopId:0,
        shopMode: _this.data.shopMode
      });

      apiCart.operate({
        storeId: _this.data.storeId,
        operate: 2,
        skus: skus
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
    },
    myClose: function (e) {
      var self = e.target.dataset.ref
      if (self == 'self') {
        this._dialogClose()
      }
    }
  }
})