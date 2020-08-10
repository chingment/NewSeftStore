import request from '@/utils/request'

export function getIndexPageData(params) {
  return request({
    url: '/home/getIndexPageData',
    method: 'get',
    params
  })
}

export function getTodaySummary(params) {
  return request({
    url: '/home/getTodaySummary',
    method: 'get',
    params
  })
}

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
  getTodaySummary: getTodaySummary,
  getProductSkuSaleRl: getProductSkuSaleRl,
  getStoreGmvRl: getStoreGmvRl,
  getTodayStoreGmvRl: getTodayStoreGmvRl,
  get7DayGmv: get7DayGmv
}
