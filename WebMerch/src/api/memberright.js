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

export function initManageBaseInfo(params) {
  return request({
    url: '/memberright/initManageBaseInfo',
    method: 'get',
    params
  })
}

export function getFeeSts(params) {
  return request({
    url: '/memberright/getFeeSts',
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

export function getCouponsByLevelSt(params) {
  return request({
    url: '/memberright/getCouponsByLevelSt',
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

export default {
  getLevelSts: getLevelSts,
  initManage: initManage,
  initManageBaseInfo: initManageBaseInfo,
  getFeeSts: getFeeSts,
  setFeeSt: setFeeSt,
  getCouponsByLevelSt: getCouponsByLevelSt,
  removeCoupon: removeCoupon,
  addCoupon: addCoupon,
  searchCoupon: searchCoupon
}
