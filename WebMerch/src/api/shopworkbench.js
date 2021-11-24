import request from '@/utils/request'

export function getInitData(params) {
  return request({
    url: '/shopworkbench/getInitData',
    method: 'get',
    params
  })
}

export function getTodaySummary(params) {
  return request({
    url: '/shopworkbench/getTodaySummary',
    method: 'get',
    params
  })
}

export function getSkuSaleRl(params) {
  return request({
    url: '/shopworkbench/getSkuSaleRl',
    method: 'get',
    params
  })
}

export function getStoreGmvRl(params) {
  return request({
    url: '/shopworkbench/getStoreGmvRl',
    method: 'get',
    params
  })
}

export function getTodayStoreGmvRl(params) {
  return request({
    url: '/shopworkbench/getTodayStoreGmvRl',
    method: 'get',
    params
  })
}

export function get7DayGmv(params) {
  return request({
    url: '/shopworkbench/get7DayGmv',
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
