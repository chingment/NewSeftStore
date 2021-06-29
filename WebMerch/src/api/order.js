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

export function getListByDeviceSelfTake(params) {
  return request({
    url: '/order/GetListByDeviceSelfTake',
    method: 'get',
    params
  })
}

export function getDetailsByDeviceSelfTake(params) {
  return request({
    url: '/order/getDetailsByDeviceSelfTake',
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

export function handleExByDeviceSelfTake(data) {
  return request({
    url: '/order/handleExByDeviceSelfTake',
    method: 'post',
    data
  })
}

export default {
  getList: getList,
  getListByDelivery: getListByDelivery,
  getListByDeviceSelfTake: getListByDeviceSelfTake,
  getListByStoreSelfTake: getListByStoreSelfTake,
  getDetailsByDeviceSelfTake: getDetailsByDeviceSelfTake,
  handleExByDeviceSelfTake: handleExByDeviceSelfTake
}
