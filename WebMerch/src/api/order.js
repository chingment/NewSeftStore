import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/order/getList',
    method: 'get',
    params
  })
}

export function getListByDelivery(params) {
  return request({
    url: '/order/GetListByDelivery',
    method: 'get',
    params
  })
}

export function getListByStoreSelfTake(params) {
  return request({
    url: '/order/GetListByStoreSelfTake',
    method: 'get',
    params
  })
}

export function getListByMachineSelfTake(params) {
  return request({
    url: '/order/GetListByMachineSelfTake',
    method: 'get',
    params
  })
}

export function getDetailsByMachineSelfTake(params) {
  return request({
    url: '/order/getDetailsByMachineSelfTake',
    method: 'get',
    params
  })
}

export function pickupExceptionHandle(data) {
  return request({
    url: '/order/pickupExceptionHandle',
    method: 'post',
    data
  })
}

export function handleExByMachineSelfTake(data) {
  return request({
    url: '/order/handleExByMachineSelfTake',
    method: 'post',
    data
  })
}

export default {
  getList: getList,
  getListByDelivery: getListByDelivery,
  getListByMachineSelfTake: getListByMachineSelfTake,
  getListByStoreSelfTake: getListByStoreSelfTake,
  getDetailsByMachineSelfTake: getDetailsByMachineSelfTake,
  handleExByMachineSelfTake: handleExByMachineSelfTake
}
