const config = require('../config')
const app = getApp()
const key_cart = "key_cart"
const key_accesstoken = "key_accesstoken"
const key_store_id = "key_store_id"
const key_open_id = "key_open_id"
const key_merch_id = "key_merch_id"
const key_session_key = "session_key"
const key_cur_store_id = "key_cur_store_id"
function getCart() {
  return wx.getStorageSync(key_cart) || []
}

function setCart(cartData) {
  wx.setStorageSync(key_cart, cartData)

  //设置页面的标志点
  var pages = getCurrentPages();
  for (var i = 0; i < pages.length; i++) {
    if (pages[i].data.tag == "main") {

      pages[i].selectComponent('#cp_cart').setData({
        cartData: cartData
      });

    } else if (pages[i].data.tag == "productdetails") {
      pages[i].setData({
        cartData: cartData
      })
    } else if (pages[i].data.tag == "productsearch") {
      pages[i].setData({
        cartData: cartData
      })
    }
  }

}

function getAccessToken() {
  return wx.getStorageSync(key_accesstoken) || ''
}

function setAccessToken(accesstoken) {
  wx.setStorageSync(key_accesstoken, accesstoken)
}

function getStoreId() {
  return wx.getStorageSync(key_store_id) || undefined
}

function setStoreId(store_id) {
  wx.setStorageSync(key_store_id, store_id)
}


function getOpenId() {
  return wx.getStorageSync(key_open_id) || undefined
}

function setOpenId(open_id) {
  wx.setStorageSync(key_open_id, open_id)
}


function getMerchId() {
  return wx.getStorageSync(key_merch_id) || undefined
}

function setMerchId(merch_id) {
  wx.setStorageSync(key_merch_id, merch_id)
}

function getSessionKey() {
  return wx.getStorageSync(key_session_key) || undefined
}

function setSessionKey(session_key) {
  wx.setStorageSync(key_session_key, session_key)
}

function getCurrentStoreId() {
  return wx.getStorageSync(key_cur_store_id) || undefined
}

function setCurrentStoreId(store_id) {
  wx.setStorageSync(key_cur_store_id, store_id)
}


module.exports = {
  getCart: getCart,
  setCart: setCart,
  setAccessToken: setAccessToken,
  getAccessToken: getAccessToken,
  getStoreId: getStoreId,
  setStoreId: setStoreId,
  getOpenId: getOpenId,
  setOpenId: setOpenId,
  getMerchId: getMerchId,
  setMerchId: setMerchId,
  getSessionKey: getSessionKey,
  setSessionKey: setSessionKey,
  getCurrentStoreId: getCurrentStoreId,
  setCurrentStoreId: setCurrentStoreId,
}