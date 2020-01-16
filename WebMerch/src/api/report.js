import request from '@/utils/request'

export function machineStockInit(params) {
  return request({
    url: '/report/machineStockInit',
    method: 'get',
    params
  })
}

export function machineStockGet(data) {
  return request({
    url: '/report/machineStockGet',
    method: 'post',
    data
  })
}

export function productSkuDaySalesInit(params) {
  return request({
    url: '/report/ProductSkuDaySalesInit',
    method: 'get',
    params
  })
}

export function productSkuDaySalesGet(data) {
  return request({
    url: '/report/ProductSkuDaySalesGet',
    method: 'post',
    data
  })
}

export default {
  machineStockInit: machineStockInit,
  machineStockGet: machineStockGet
}
