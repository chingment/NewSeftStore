import request from '@/utils/request'

export function getListByDelivery(params) {
  return request({
    url: '/ordersub/GetListByDelivery',
    method: 'get',
    params
  })
}

export function getListByStoreSelfTake(params) {
  return request({
    url: '/ordersub/GetListByStoreSelfTake',
    method: 'get',
    params
  })
}

export function getListByMachineSelfTake(params) {
  return request({
    url: '/ordersub/GetListByMachineSelfTake',
    method: 'get',
    params
  })
}

export function getDetailsByMachineSelfTake(params) {
  return request({
    url: '/ordersub/getDetailsByMachineSelfTake',
    method: 'get',
    params
  })
}

export function pickupExceptionHandle(data) {
  return request({
    url: '/ordersub/pickupExceptionHandle',
    method: 'post',
    data
  })
}

export function handleExByMachineSelfTake(data) {
  return request({
    url: '/ordersub/handleExByMachineSelfTake',
    method: 'post',
    data
  })
}

export default {
  getListByDelivery: getListByDelivery,
  getListByMachineSelfTake: getListByMachineSelfTake,
  getListByStoreSelfTake: getListByStoreSelfTake,
  getDetailsByMachineSelfTake: getDetailsByMachineSelfTake,
  handleExByMachineSelfTake: handleExByMachineSelfTake
}
