import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/payrefund/getList',
    method: 'get',
    params
  })
}

export function getDetails(params) {
  return request({
    url: '/payrefund/getDetails',
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

export function getApplyDetails(params) {
  return request({
    url: '/payrefund/GetApplyDetails',
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
  getDetails: getDetails,
  getListByHandle: getListByHandle,
  searchOrder: searchOrder,
  getApplyDetails: getApplyDetails,
  getHandleDetails: getHandleDetails,
  apply: apply,
  handle: handle
}

