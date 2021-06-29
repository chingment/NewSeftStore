import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/device/getlist',
    method: 'get',
    params
  })
}

export function initGetList() {
  return request({
    url: '/device/initGetList',
    method: 'get'
  })
}

export function edit(data) {
  return request({
    url: '/device/edit',
    method: 'post',
    data
  })
}

export function initManage(params) {
  return request({
    url: '/device/initManage',
    method: 'get',
    params
  })
}

export function initManageBaseInfo(params) {
  return request({
    url: '/device/initManageBaseInfo',
    method: 'get',
    params
  })
}

export function initManageStock(params) {
  return request({
    url: '/device/initManageStock',
    method: 'get',
    params
  })
}

export function manageStockGetStocks(params) {
  return request({
    url: '/device/manageStockGetStocks',
    method: 'get',
    params
  })
}

export function manageStockEditStock(data) {
  return request({
    url: '/device/manageStockEditStock',
    method: 'post',
    data
  })
}

export function sysReboot(data) {
  return request({
    url: '/device/sysReboot',
    method: 'post',
    data
  })
}

export function sysShutdown(data) {
  return request({
    url: '/device/sysShutdown',
    method: 'post',
    data
  })
}

export function sysSetStatus(data) {
  return request({
    url: '/device/sysSetStatus',
    method: 'post',
    data
  })
}

export function queryMsgPushResult(data) {
  return request({
    url: '/device/queryMsgPushResult',
    method: 'post',
    data
  })
}

export function dsx01OpenPickupDoor(data) {
  return request({
    url: '/device/dsx01OpenPickupDoor',
    method: 'post',
    data
  })
}

export function bindShop(data) {
  return request({
    url: '/device/bindShop',
    method: 'post',
    data
  })
}

export function unBindShop(data) {
  return request({
    url: '/device/unBindShop',
    method: 'post',
    data
  })
}

export default {
  initGetList: initGetList,
  getList: getList,
  edit: edit,
  initManage: initManage,
  initManageBaseInfo: initManageBaseInfo,
  initManageStock: initManageStock,
  manageStockGetStocks: manageStockGetStocks,
  manageStockEditStock: manageStockEditStock,
  sysReboot: sysReboot,
  sysShutdown: sysShutdown,
  sysSetStatus: sysSetStatus,
  queryMsgPushResult: queryMsgPushResult,
  dsx01OpenPickupDoor: dsx01OpenPickupDoor,
  bindShop: bindShop,
  unBindShop: unBindShop
}
