import request from '@/utils/request'

export function machineStockRealDataInit(params) {
  return request({
    url: '/report/machineStockRealDataInit',
    method: 'get',
    params
  })
}

export function machineStockRealDataGet(data) {
  return request({
    url: '/report/machineStockRealDataGet',
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

export function productSkuSalesDateHisInit(params) {
  return request({
    url: '/report/productSkuSalesDateHisInit',
    method: 'get',
    params
  })
}

export function productSkuSalesDateHisGet(data) {
  return request({
    url: '/report/productSkuSalesDateHisGet',
    method: 'post',
    data
  })
}

export function orderSalesDateHisInit(params) {
  return request({
    url: '/report/orderSalesDateHisInit',
    method: 'get',
    params
  })
}

export function orderSalesDateHisGet(data) {
  return request({
    url: '/report/orderSalesDateHisGet',
    method: 'post',
    data
  })
}

export default {
  machineStockRealDataInit: machineStockRealDataInit,
  machineStockRealDataGet: machineStockRealDataGet,
  productSkuSalesDateHisInit: productSkuSalesDateHisInit,
  productSkuSalesDateHisGet: productSkuSalesDateHisGet,
  orderSalesDateHisInit: orderSalesDateHisInit,
  orderSalesDateHisGet: orderSalesDateHisGet,
  machineStockDateHisInit: machineStockDateHisInit,
  machineStockDateHisGet: machineStockDateHisGet
}
