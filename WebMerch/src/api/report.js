import request from '@/utils/request'

export function storeStockRealDataInit(params) {
  return request({
    url: '/report/storeStockRealDataInit',
    method: 'get',
    params
  })
}

export function storeStockRealDataGet(data) {
  return request({
    url: '/report/storeStockRealDataGet',
    method: 'post',
    data
  })
}

export function storeStockDateHisInit(params) {
  return request({
    url: '/report/storeStockDateHisInit',
    method: 'get',
    params
  })
}

export function storeStockDateHisGet(data) {
  return request({
    url: '/report/storeStockDateHisGet',
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

export function storeSalesDateHisInit(params) {
  return request({
    url: '/report/storeSalesDateHisInit',
    method: 'get',
    params
  })
}

export function storeSalesDateHisGet(data) {
  return request({
    url: '/report/storeSalesDateHisGet',
    method: 'post',
    data
  })
}

export default {
  storemachineStockRealDataInit: storeStockRealDataInit,
  storehStockRealDataGet: storeStockRealDataGet,
  productSkuSalesDateHisInit: productSkuSalesDateHisInit,
  productSkuSalesDateHisGet: productSkuSalesDateHisGet,
  orderSalesDateHisInit: orderSalesDateHisInit,
  orderSalesDateHisGet: orderSalesDateHisGet,
  storeStockDateHisInit: storeStockDateHisInit,
  storeStockDateHisGet: storeStockDateHisGet,
  storeSalesDateHisInit: storeSalesDateHisInit,
  storeSalesDateHisGet: storeSalesDateHisGet
}
