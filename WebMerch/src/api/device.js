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

export function rebootSys(data) {
  return request({
    url: '/device/rebootSys',
    method: 'post',
    data
  })
}

export function shutdownSys(data) {
  return request({
    url: '/device/shutdownSys',
    method: 'post',
    data
  })
}

export function setSysStatus(data) {
  return request({
    url: '/device/setSysStatus',
    method: 'post',
    data
  })
}

export function getSysParams(params) {
  return request({
    url: '/device/getSysParams',
    method: 'get',
    params
  })
}

export function setSysParams(data) {
  return request({
    url: '/device/setSysParams',
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

export function openPickupDoor(data) {
  return request({
    url: '/device/openPickupDoor',
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
  rebootSys: rebootSys,
  shutdownSys: shutdownSys,
  setSysStatus: setSysStatus,
  setSysParams: setSysParams,
  getSysParams: getSysParams,
  queryMsgPushResult: queryMsgPushResult,
  openPickupDoor: openPickupDoor,
  bindShop: bindShop,
  unBindShop: unBindShop
}
