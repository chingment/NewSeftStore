import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/clientuser/getlist',
    method: 'get',
    params
  })
}

export function initDetails(params) {
  return request({
    url: '/clientuser/initDetails',
    method: 'get',
    params
  })
}

export function initDetailsBaseInfo(params) {
  return request({
    url: '/clientuser/initDetailsBaseInfo',
    method: 'get',
    params
  })
}

export function initDetailsOrders(params) {
  return request({
    url: '/clientuser/initDetailsOrders',
    method: 'get',
    params
  })
}

export function detailsOrdersGetOrderList(params) {
  return request({
    url: '/clientuser/detailsOrdersGetOrderList',
    method: 'get',
    params
  })
}

export default {
  getList: getList,
  initDetails: initDetails,
  initDetailsBaseInfo: initDetailsBaseInfo,
  initDetailsOrders: initDetailsOrders,
  detailsOrdersGetOrderList: detailsOrdersGetOrderList
}
