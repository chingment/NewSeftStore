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

export function skuSalesHisInit(params) {
  return request({
    url: '/report/skuSalesHisInit',
    method: 'get',
    params
  })
}

export function skuSalesHisGet(data) {
  return request({
    url: '/report/skuSalesHisGet',
    method: 'post',
    data
  })
}

export function orderSalesHisInit(params) {
  return request({
    url: '/report/orderSalesHisInit',
    method: 'get',
    params
  })
}

export function orderSalesHisGet(data) {
  return request({
    url: '/report/orderSalesHisGet',
    method: 'post',
    data
  })
}

export function storeSalesHisInit(params) {
  return request({
    url: '/report/storeSalesHisInit',
    method: 'get',
    params
  })
}

export function storeSalesHisGet(data) {
  return request({
    url: '/report/storeSalesHisGet',
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
  skuSalesHisInit: skuSalesHisInit,
  skuSalesHisGet: skuSalesHisGet,
  orderSalesHisInit: orderSalesHisInit,
  orderSalesHisGet: orderSalesHisGet,
  storeStockDateHisInit: storeStockDateHisInit,
  storeStockDateHisGet: storeStockDateHisGet,
  storeSalesHisInit: storeSalesHisInit,
  storeSalesHisGet: storeSalesHisGet,
  checkRightExport: checkRightExport
}
