import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/machine/getlist',
    method: 'get',
    params
  })
}

export function initGetList() {
  return request({
    url: '/machine/initGetList',
    method: 'get'
  })
}

export function edit(data) {
  return request({
    url: '/machine/edit',
    method: 'post',
    data
  })
}

export function initManage(params) {
  return request({
    url: '/machine/initManage',
    method: 'get',
    params
  })
}

export function initManageBaseInfo(params) {
  return request({
    url: '/machine/initManageBaseInfo',
    method: 'get',
    params
  })
}

export function initManageStock(params) {
  return request({
    url: '/machine/initManageStock',
    method: 'get',
    params
  })
}

export function manageStockGetStocks(params) {
  return request({
    url: '/machine/manageStockGetStocks',
    method: 'get',
    params
  })
}

export function manageStockEditStock(data) {
  return request({
    url: '/machine/manageStockEditStock',
    method: 'post',
    data
  })
}

export function sysReboot(data) {
  return request({
    url: '/machine/sysReboot',
    method: 'post',
    data
  })
}

export function sysShutdown(data) {
  return request({
    url: '/machine/sysShutdown',
    method: 'post',
    data
  })
}

export function sysSetStatus(data) {
  return request({
    url: '/machine/sysSetStatus',
    method: 'post',
    data
  })
}

export function queryMsgPushResult(data) {
  return request({
    url: '/machine/queryMsgPushResult',
    method: 'post',
    data
  })
}

export function dsx01OpenPickupDoor(data) {
  return request({
    url: '/machine/dsx01OpenPickupDoor',
    method: 'post',
    data
  })
}

export function bindShop(data) {
  return request({
    url: '/machine/bindShop',
    method: 'post',
    data
  })
}

export function unBindShop(data) {
  return request({
    url: '/machine/unBindShop',
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
