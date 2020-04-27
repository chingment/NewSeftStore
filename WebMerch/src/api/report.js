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

export function machineStockDateHisInit(params) {
  return request({
    url: '/report/machineStockDateHisInit',
    method: 'get',
    params
  })
}

export function machineStockDateHisGet(data) {
  return request({
    url: '/report/machineStockDateHisGet',
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

export function orderInit(params) {
  return request({
    url: '/report/OrderInit',
    method: 'get',
    params
  })
}

export function orderGet(data) {
  return request({
    url: '/report/OrderGet',
    method: 'post',
    data
  })
}

export default {
  machineStockInit: machineStockInit,
  machineStockGet: machineStockGet,
  productSkuDaySalesInit: productSkuDaySalesInit,
  productSkuDaySalesGet: productSkuDaySalesGet,
  orderInit: orderInit,
  orderGet: orderGet,
  machineStockDateHisInit: machineStockDateHisInit,
  machineStockDateHisGet: machineStockDateHisGet
}
