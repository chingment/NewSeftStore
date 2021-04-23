import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/payrefund/getList',
    method: 'get',
    params
  })
}

export function getListByHandle(params) {
  return request({
    url: '/payrefund/getListByHandle',
    method: 'get',
    params
  })
}

export function searchOrder(params) {
  return request({
    url: '/payrefund/searchOrder',
    method: 'get',
    params
  })
}

export function getOrderDetails(params) {
  return request({
    url: '/payrefund/GetOrderDetails',
    method: 'get',
    params
  })
}

export function getHandleDetails(params) {
  return request({
    url: '/payrefund/GetHandleDetails',
    method: 'get',
    params
  })
}

export function apply(data) {
  return request({
    url: '/payrefund/apply',
    method: 'post',
    data
  })
}

export function handle(data) {
  return request({
    url: '/payrefund/handle',
    method: 'post',
    data
  })
}

export default {
  getList: getList,
  getListByHandle: getListByHandle,
  searchOrder: searchOrder,
  getOrderDetails: getOrderDetails,
  getHandleDetails: getHandleDetails,
  apply: apply,
  handle: handle
}

