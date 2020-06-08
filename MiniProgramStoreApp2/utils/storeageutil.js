

const config = require('../config')

const key_productkind = "key_productkind"
const key_cart = "key_cart"
const key_accesstoken = "key_accesstoken"
const key_store = "key_store"
const key_openid = "openid"

function getProductKind() {
  return wx.getStorageSync(key_productkind) || []
}

function setProductKind(productkind) {
  wx.setStorageSync(key_productkind, productkind)
}

function getCart() {
  return wx.getStorageSync(key_cart) || []
}

function setCart(cart) {
  wx.setStorageSync(key_cart, cart)

  //设置页面的标志点
  var pages = getCurrentPages();
  for (var i = 0; i < pages.length; i++) {
    if (pages[i].data.tag == "main") {
      pages[i].data.tabBar[2].number = cart.count
      pages[i].setData({ cart: cart, tabBar: pages[i].data.tabBar })
    }
    else if (pages[i].data.tag == "productdetails") {
      pages[i].setData({ cart: cart })
    }
    else if (pages[i].data.tag == "productlist") {
      pages[i].setData({ cart: cart })
    }
  }

}

function getAccessToken() {
  return wx.getStorageSync(key_accesstoken) || []
}

function setAccessToken(accesstoken) {
  wx.setStorageSync(key_accesstoken, accesstoken)
}

function getCurrentStoreId() {

  if( typeof config.storeId != 'undefined'){
if(config.storeId!=''){
   return config.storeId
}
  }

  console.log("storeId:"+wx.getStorageSync(key_store))

  return wx.getStorageSync(key_store) || undefined
}

function setCurrentStoreId(id) {
  wx.setStorageSync(key_store, id)
}


function getOpenId() {
  return wx.getStorageSync(key_openid) || undefined
}

function setOpenId(openid) {
  wx.setStorageSync(key_openid, openid)
}

module.exports = {
  getProductKind: getProductKind,
  setProductKind: setProductKind,
  getCart: getCart,
  setCart: setCart,
  setAccessToken: setAccessToken,
  getAccessToken: getAccessToken,
  getCurrentStoreId: getCurrentStoreId,
  setCurrentStoreId: setCurrentStoreId,
  getOpenId: getOpenId,
  setOpenId: setOpenId
}