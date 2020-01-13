import request from '@/utils/request'

export function machineStockInit(params) {
  return request({
    url: '/report/machineStockInit',
    method: 'get',
    params
  })
}

export function machineStockGet(params) {
  return request({
    url: '/report/machineStockGet',
    method: 'get',
    params
  })
}

export function productSkuDaySalesInit(params) {
  return request({
    url: '/report/ProductSkuDaySalesInit',
    method: 'get',
    params
  })
}

export function productSkuDaySalesGet(params) {
  return request({
    url: '/report/ProductSkuDaySalesGet',
    method: 'get',
    params
  })
}

export default {
  machineStockInit: machineStockInit,
  machineStockGet: machineStockGet
}
