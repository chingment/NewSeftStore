import request from '@/utils/request'

export function deviceStockRealDataInit(params) {
  return request({
    url: '/report/deviceStockRealDataInit',
    method: 'get',
    params
  })
}

export function deviceStockRealDataGet(data) {
  return request({
    url: '/report/deviceStockRealDataGet',
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

export function skuSalesDateHisInit(params) {
  return request({
    url: '/report/skuSalesDateHisInit',
    method: 'get',
    params
  })
}

export function skuSalesDateHisGet(data) {
  return request({
    url: '/report/skuSalesDateHisGet',
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

export function checkRightExport(data) {
  return request({
    url: '/report/checkRightExport',
    method: 'post',
    data
  })
}

export function deviceReplenishPlanInit(params) {
  return request({
    url: '/report/deviceReplenishPlanInit',
    method: 'get',
    params
  })
}

export function deviceReplenishPlanGet(data) {
  return request({
    url: '/report/deviceReplenishPlanGet',
    method: 'post',
    data
  })
}

export default {
  deviceStockRealDataInit: deviceStockRealDataInit,
  devicehStockRealDataGet: deviceStockRealDataGet,
  skuSalesDateHisInit: skuSalesDateHisInit,
  skuSalesDateHisGet: skuSalesDateHisGet,
  orderSalesDateHisInit: orderSalesDateHisInit,
  orderSalesDateHisGet: orderSalesDateHisGet,
  storeStockDateHisInit: storeStockDateHisInit,
  storeStockDateHisGet: storeStockDateHisGet,
  storeSalesDateHisInit: storeSalesDateHisInit,
  storeSalesDateHisGet: storeSalesDateHisGet,
  checkRightExport: checkRightExport
}
