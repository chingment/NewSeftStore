Page({

  data: {

    simulatedDATA: {

      difference: [{

        id: "19",

        price: "200.00",

        stock: "19",

        difference: "红色,x"

      },

      {

        id: "20",

        price: "300.00",

        stock: "29",

        difference: "白色,x"

      },

      {

        id: "21",

        price: "300.00",

        stock: "10",

        difference: "黑色,x"

      },

      {

        id: "21",

        price: "300.00",

        stock: "10",

        difference: "黑色,xl"

      },

      {

        id: "24",

        price: "500.00",

        stock: "10",

        difference: "白色,xl"

      }

      ],

      specifications: [{

        name: "颜色",

        item: [{

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

        item: [{

          name: "x"

        },

        {

          name: "xl"

        }

        ]

      }

      ]

    },

    selectArr: [], //存放被选中的值

    shopItemInfo: {}, //存放要和选中的值进行匹配的数据

    subIndex: [], //是否选中 因为不确定是多规格还是但规格，所以这里定义数组来判断

    content: "",

    specifications: '',

    boxArr: {},

  },

  onLoad() {

    var self = this

    var simulatedDATA = self.data.simulatedDATA

    var difference = self.data.simulatedDATA.difference

    var shopItemInfo = self.data.shopItemInfo

    var specifications = self.data.simulatedDATA.specifications

    for (var i in difference) {

      shopItemInfo[difference[i].difference] =

        difference[i]; //修改数据结构格式，改成键值对的方式，以方便和选中之后的值进行匹配

    }

    console.log(JSON.stringify(shopItemInfo))

    for (var i = 0; i < specifications.length; i++) {

      for (var o = 0; o < specifications[i].item.length; o++) {

        specifications[i].item[o].isShow = true

      }

    }

    console.log(JSON.stringify(specifications))

    simulatedDATA.specifications = specifications

    self.setData({

      simulatedDATA: simulatedDATA,

      shopItemInfo: shopItemInfo,

      specifications: specifications

    })

  },

  onShow() {



  },

  specificationBtn(e) {

    var n = e.currentTarget.dataset.n

    var index = e.currentTarget.dataset.index

    var item = e.currentTarget.dataset.name

    var self = this;

    var selectArr = self.data.selectArr

    var subIndex = self.data.subIndex

    var boxArr = self.data.boxArr

    var shopItemInfo = self.data.shopItemInfo

    if (selectArr[n] != item) {

      selectArr[n] = item;

      subIndex[n] = index;

    } else {

      // self.selectArr[n] = "";

      // self.subIndex[n] = -1; //去掉选中的颜色

    }

    self.checkItem();

    var arr = shopItemInfo[selectArr];

    if (arr) {

      boxArr.id = arr.id;

      boxArr.price = arr.price;

    }

    self.setData({

      selectArr: selectArr, subIndex: subIndex, boxArr: boxArr, shopItemInfo: shopItemInfo

    })

    console.log(boxArr)

  },

  checkItem() {

    var self = this;

    var simulatedDATA = self.data.simulatedDATA

    var option = self.data.simulatedDATA.specifications;

    var result = []; //定义数组存储被选中的值

    for (var i in option) {

      result[i] = self.data.selectArr[i] ? self.data.selectArr[i] : "";

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

    simulatedDATA.specifications = option

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

    return !this.data.shopItemInfo[result] ?

      false :

      this.data.shopItemInfo[result].stock == 0 ?

        false :

        true; //匹配选中的数据的库存，若不为空返回true反之返回false

  },

})