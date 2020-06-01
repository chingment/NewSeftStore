Page({

  data: {

    simulatedDATA: {

       specIdxSkus: [{

        id: "19",

        price: "200.00",

        stock: "19",

        specIdx: "红色,x"

      },

      {

        id: "20",

        price: "300.00",

        stock: "29",

        specIdx: "白色,x"

      },

      {

        id: "21",

        price: "300.00",

        stock: "10",

        specIdx: "黑色,x"

      },

      {

        id: "21",

        price: "300.00",

        stock: "10",

        specIdx: "黑色,xl"

      },

      {

        id: "24",

        price: "500.00",

        stock: "10",

        specIdx: "白色,xl"

      }

      ],

      specItems: [{

        name: "颜色",

        value: [{

          name: "白色"

        },

        {

          name: "黑色"

        },

        {

          name: "红色"

        }

        ]

      },

      {

        name: "尺码",

        value: [{

          name: "x"

        },

        {

          name: "xl"

        }

        ]

      }

      ]

    },
    specSelectArr: [], //存放被选中的值
    specShopItemInfo: {}, //存放要和选中的值进行匹配的数据
    specSubIndex: [], //是否选中 因为不确定是多规格还是但规格，所以这里定义数组来判断
    specBoxArr: {},

  },

  onLoad() {

    var self = this

    var simulatedDATA = self.data.simulatedDATA

    var specIdxSkus = self.data.simulatedDATA.specIdxSkus

    var specShopItemInfo = self.data.specShopItemInfo

    var specItems = self.data.simulatedDATA.specItems

    for (var i in specIdxSkus) {

      specShopItemInfo[specIdxSkus[i].specIdx] =

      specIdxSkus[i]; //修改数据结构格式，改成键值对的方式，以方便和选中之后的值进行匹配

    }

    console.log(JSON.stringify(specShopItemInfo))

    for (var i = 0; i < specItems.length; i++) {

      for (var o = 0; o < specItems[i].value.length; o++) {

        specItems[i].value[o].isShow = true

      }

    }

    console.log(JSON.stringify(specItems))

    simulatedDATA.specItems = specItems

    self.setData({

      simulatedDATA: simulatedDATA,

      specShopItemInfo: specShopItemInfo,

      specItems: specItems

    })

  },

  onShow() {



  },

  specificationBtn(e) {

    var n = e.currentTarget.dataset.n

    var index = e.currentTarget.dataset.index

    var item = e.currentTarget.dataset.name

    var self = this;

    var specSelectArr = self.data.specSelectArr

    var specSubIndex = self.data.specSubIndex

    var specBoxArr = self.data.specBoxArr

    var specShopItemInfo = self.data.specShopItemInfo

    if (specSelectArr[n] != item) {

      specSelectArr[n] = item;

      specSubIndex[n] = index;

    } else {

      // self.selectArr[n] = "";

      // self.subIndex[n] = -1; //去掉选中的颜色

    }

    self.checkItem();

    console.log("dasdasddd:"+specSelectArr)

    var arr = specShopItemInfo[specSelectArr];

    if (arr) {

      specBoxArr.id = arr.id;

      specBoxArr.price = arr.price;

    }

    self.setData({

      specSelectArr: specSelectArr, specSubIndex: specSubIndex, specBoxArr: specBoxArr, specShopItemInfo: specShopItemInfo

    })

    console.log(specBoxArr)

  },

  checkItem() {

    var self = this;

    var simulatedDATA = self.data.simulatedDATA

    var option = self.data.simulatedDATA.specItems;

    var result = []; //定义数组存储被选中的值

    for (var i in option) {

      result[i] = self.data.specSelectArr[i] ? self.data.specSelectArr[i] : "";

    }

    console.log(JSON.stringify(result))

    for (var i in option) {

      var last = result[i]; //把选中的值存放到字符串last去

      for (var k in option[i].item) {

        result[i] = option[i].item[k].name; //赋值，存在直接覆盖，不存在往里面添加name值

        option[i].item[k].isShow = self.isMay(result); //在数据里面添加字段isShow来判断是否可以选择

      }

      result[i] = last; //还原，目的是记录点下去那个值，避免下一次执行循环时避免被覆盖

    }

    simulatedDATA.specItems = option

    self.setData({

      simulatedDATA: simulatedDATA

    })

  },

  isMay(result) {

    for (var i in result) {

      if (result[i] == "") {

        return true; //如果数组里有为空的值，那直接返回true

      }

    }

    return !this.data.specShopItemInfo[result] ?

      false :

      this.data.specShopItemInfo[result].stock == 0 ?

        false :

        true; //匹配选中的数据的库存，若不为空返回true反之返回false

  },

})