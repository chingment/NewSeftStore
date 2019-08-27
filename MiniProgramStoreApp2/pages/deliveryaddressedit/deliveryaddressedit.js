const config = require('../../config')
const storeage = require('../../utils/storeageutil.js')
const ownRequest = require('../../own/ownRequest.js')
const lumos = require('../../utils/lumos.minprogram.js')
const cityList = require('./cityList').cityList;
const toastUtil = require('../../utils/showtoastutil');//引入消息提醒暴露的接口

const app = getApp()

Page({
  data: {
    address: { area: "" },
    showArea: false,
    currentTab: 1,
    country: [],
    residecity: [],
    resideprovince: [],

    curr_pro: '',
    curr_cit: '',
    curr_cou: '',
    deliveryAddress: {
      id: 0,
      consignee: "",
      phoneNumber: "",
      area: "",
      address: "",
      isDefault: false
    },
    deliveryAddress_isDefault: false
  },

  choosearea: function () {

    let result = this.data.address;
    var currentTab = 1;
    if (result.country) {
      currentTab = 3;
    } else if (result.residecity) {
      currentTab = 3;
    } else if (result.resideprovince) {
      currentTab = 1;
    } else {
      currentTab = 1;
    }

    let resideprovince = [];
    let residecity = [];
    let country = [];

    cityList.forEach((item) => {
      resideprovince.push({
        name: item.name
      });
      if (item.name == result.resideprovince) {
        item.city.forEach((item) => {
          residecity.push({
            name: item.name
          });
          if (item.name == result.residecity) {
            item.area.forEach((item) => {
              country.push({
                name: item.name
              });
            });
          }
        });
      }
    });

    this.setData({
      showArea: true,
      resideprovince: resideprovince,
      residecity: residecity,
      country: country,

      currentTab: currentTab,
      curr_pro: result.resideprovince || '请选择',
      curr_cit: result.residecity || '请选择',
      curr_cou: result.country || '请选择',
    });
  },
  areaClose: function () {
    this.setData({
      showArea: false
    });
  },
  //点击省选项卡
  resideprovince: function (e) {
    this.setData({
      currentTab: 1
    });
  },
  //点击市选项卡
  residecity: function () {
    this.setData({
      currentTab: 2
    });
  },
  country: function () {
    this.setData({
      currentTab: 3
    });
  },
  //点击选择省
  selectResideprovince: function (e) {
    let residecity = [];
    let country = [];
    let name = e.currentTarget.dataset.itemName;

    cityList.forEach((item) => {
      if (item.name == name) {
        item.city.forEach((item, index) => {
          residecity.push({
            name: item.name
          });
          if (index == 0) {
            item.area.forEach((item) => {
              country.push({
                name: item.name
              });
            });
          }
        });
      }
    });

    this.setData({
      currentTab: 2,
      residecity: residecity,
      country: country,
      curr_pro: e.currentTarget.dataset.itemName,
      curr_cit: '请选择',
      curr_cou: '',
    });
  },
  //点击选择市
  selectResidecity: function (e) {
    let country = [];
    let name = e.currentTarget.dataset.itemName;
    cityList.forEach((item) => {
      if (item.name == this.data.curr_pro) {
        item.city.forEach((item, index) => {
          if (item.name == name) {
            item.area.forEach((item) => {
              country.push({
                name: item.name
              });
            });
          }
        });
      }
    });

    this.setData({
      currentTab: 3,
      country: country,
      curr_cit: e.currentTarget.dataset.itemName,
      curr_cou: '请选择',
    });
  },
  //点击选择区
  selectCountry: function (e) {
    this.data.curr_cou = e.currentTarget.dataset.itemName;

    this.data.address.resideprovince = this.data.curr_pro;
    this.data.address.residecity = this.data.curr_cit;
    this.data.address.country = this.data.curr_cou;

    this.data.address.area = this.data.curr_pro + "-" + this.data.curr_cit + '-' + this.data.curr_cou


    this.setData({
      showArea: false,
      curr_cou: this.data.curr_cou,
      address: this.data.address
    });

  },
  // 滑动切换tab
  bindChange: function (e) {
    var that = this;
    that.setData({
      currentTab: e.detail.current + 1
    });
  },
  isDefaultEvent: function (e) {
    this.setData({
      deliveryAddress_isDefault: !this.data.deliveryAddress_isDefault
    })
  },
  onLoad: function (options) {
    var _this = this

    if (typeof options.deliveryAddress != 'undefined') {
      console.log(options.deliveryAddress)
      var deliveryAddress = JSON.parse(options.deliveryAddress)

      _this.data.address.area = deliveryAddress.areaName
      _this.data.address.resideprovince = deliveryAddress.areaName.split('-')[0];
      _this.data.address.residecity = deliveryAddress.areaName.split('-')[1];
      _this.data.address.country = deliveryAddress.areaName.split('-')[2];

      _this.data.deliveryAddress_isDefault = deliveryAddress.isDefault
      _this.setData({
        deliveryAddress: deliveryAddress,
        address: _this.data.address,
        deliveryAddress_isDefault: _this.data.deliveryAddress_isDefault
      })
    }

  },
  formSubmit: function (e) {
    var id = e.detail.value.deliveryAddress_id
    var consignee = e.detail.value.deliveryAddress_consignee
    var phoneNumber = e.detail.value.deliveryAddress_phoneNumber
    var areaName = e.detail.value.deliveryAddress_areaName
    var address = e.detail.value.deliveryAddress_address
    var isDefault = e.detail.value.deliveryAddress_isDefault

    if (consignee.length == 0) {
      toastUtil.showToast({ title: '请输入姓名' })
      return
    }
    if (phoneNumber.length == 0) {
      toastUtil.showToast({ title: '请输入手机号码' })
      return
    }
    if (areaName.length == 0) {
      toastUtil.showToast({ title: '请选择身份，城市，区县' })
      return
    }
    if (address.length == 0) {
      toastUtil.showToast({ title: '请输入详细地址' })
      return
    }

    lumos.postJson({
      url: config.apiUrl.deliveryAddressEdit, dataParams: { id: id, consignee: consignee, phoneNumber: phoneNumber, areaName: areaName, address: address, isDefault: isDefault },
      success: function (res) {
        wx.navigateBack()
      }
    })

  }

});
