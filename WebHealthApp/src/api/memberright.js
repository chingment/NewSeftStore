import request from '@/utils/request'

export function getLevelSts(params) {
  return request({
    url: '/memberright/getLevelSts',
    method: 'get',
    params
  })
}

export function initManage(params) {
  return request({
    url: '/memberright/initManage',
    method: 'get',
    params
  })
}

export function getLevelSt(params) {
  return request({
    url: '/memberright/getLevelSt',
    method: 'get',
    params
  })
}

export function setFeeSt(data) {
  return request({
    url: '/memberright/setFeeSt',
    method: 'post',
    data
  })
}

export function getFeeSts(params) {
  return request({
    url: '/memberright/getFeeSts',
    method: 'get',
    params
  })
}

export function setLevelSt(data) {
  return request({
    url: '/memberright/setLevelSt',
    method: 'post',
    data
  })
}

export function getCoupons(params) {
  return request({
    url: '/memberright/getCoupons',
    method: 'get',
    params
  })
}

export function removeCoupon(data) {
  return request({
    url: '/memberright/removeCoupon',
    method: 'post',
    data
  })
}

export function addCoupon(data) {
  return request({
    url: '/memberright/addCoupon',
    method: 'post',
    data
  })
}

export function searchCoupon(params) {
  return request({
    url: '/memberright/searchCoupon',
    method: 'get',
    params
  })
}

export function getSkus(params) {
  return request({
    url: '/memberright/getSkus',
    method: 'get',
    params
  })
}

export function addSku(data) {
  return request({
    url: '/memberright/addSku',
    method: 'post',
    data
  })
}

export function editSku(data) {
  return request({
    url: '/memberright/editSku',
    method: 'post',
    data
  })
}

export default {
  getLevelSts: getLevelSts,
  initManage: initManage,
  getLevelSt: getLevelSt,
  setLevelSt: setLevelSt,
  getFeeSts: getFeeSts,
  setFeeSt: setFeeSt,
  getCoupons: getCoupons,
  removeCoupon: removeCoupon,
  addCoupon: addCoupon,
  searchCoupon: searchCoupon,
  getSkus: getSkus,
  addSku: addSku,
  editSku: editSku
}
