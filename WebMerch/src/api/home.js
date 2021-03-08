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

export function getSkuSaleRl(params) {
  return request({
    url: '/home/getSkuSaleRl',
    method: 'get',
    params
  })
}

export function getStoreGmvRl(params) {
  return request({
    url: '/home/getStoreGmvRl',
    method: 'get',
    params
  })
}

export function getTodayStoreGmvRl(params) {
  return request({
    url: '/home/getTodayStoreGmvRl',
    method: 'get',
    params
  })
}

export function get7DayGmv(params) {
  return request({
    url: '/home/get7DayGmv',
    method: 'get',
    params
  })
}

export default {
  getTodaySummary: getTodaySummary,
  getSkuSaleRl: getSkuSaleRl,
  getStoreGmvRl: getStoreGmvRl,
  getTodayStoreGmvRl: getTodayStoreGmvRl,
  get7DayGmv: get7DayGmv
}
