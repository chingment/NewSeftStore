import request from '@/utils/request'

export function getProductSkuSaleRl(params) {
  return request({
    url: '/home/GetProductSkuSaleRl',
    method: 'get',
    params
  })
}

export function getStoreGmvRl(params) {
  return request({
    url: '/home/GetStoreGmvRl',
    method: 'get',
    params
  })
}

export function getTodayStoreGmvRl(params) {
  return request({
    url: '/home/GetTodayStoreGmvRl',
    method: 'get',
    params
  })
}

export function get7DayGmv(params) {
  return request({
    url: '/home/Get7DayGmv',
    method: 'get',
    params
  })
}

export default {
  getProductSkuSaleRl: getProductSkuSaleRl,
  getStoreGmvRl: getStoreGmvRl,
  getTodayStoreGmvRl: getTodayStoreGmvRl,
  get7DayGmv: get7DayGmv
}
