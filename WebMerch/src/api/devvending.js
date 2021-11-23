import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/devvending/getlist',
    method: 'get',
    params
  })
}

export function initGetList() {
  return request({
    url: '/devvending/initGetList',
    method: 'get'
  })
}

export function edit(data) {
  return request({
    url: '/devvending/edit',
    method: 'post',
    data
  })
}

export function getListByShop(params) {
  return request({
    url: '/devvending/getListByShop',
    method: 'get',
    params
  })
}

export function getListBySbShop(params) {
  return request({
    url: '/devvending/getListBySbShop',
    method: 'get',
    params
  })
}

export function initManage(params) {
  return request({
    url: '/devvending/initManage',
    method: 'get',
    params
  })
}

export function initManageBaseInfo(params) {
  return request({
    url: '/devvending/initManageBaseInfo',
    method: 'get',
    params
  })
}

export function initManageStock(params) {
  return request({
    url: '/devvending/initManageStock',
    method: 'get',
    params
  })
}

export function manageStockGetStocks(params) {
  return request({
    url: '/devvending/manageStockGetStocks',
    method: 'get',
    params
  })
}

export function manageStockEditStock(data) {
  return request({
    url: '/devvending/manageStockEditStock',
    method: 'post',
    data
  })
}

export function rebootSys(data) {
  return request({
    url: '/devvending/rebootSys',
    method: 'post',
    data
  })
}

export function shutdownSys(data) {
  return request({
    url: '/devvending/shutdownSys',
    method: 'post',
    data
  })
}

export function setSysStatus(data) {
  return request({
    url: '/devvending/setSysStatus',
    method: 'post',
    data
  })
}

export function getSysParams(params) {
  return request({
    url: '/devvending/getSysParams',
    method: 'get',
    params
  })
}

export function setSysParams(data) {
  return request({
    url: '/devvending/setSysParams',
    method: 'post',
    data
  })
}

export function openPickupDoor(data) {
  return request({
    url: '/devvending/openPickupDoor',
    method: 'post',
    data
  })
}

export function updateApp(data) {
  return request({
    url: '/devvending/updateApp',
    method: 'post',
    data
  })
}

export function bindShop(data) {
  return request({
    url: '/devvending/bindShop',
    method: 'post',
    data
  })
}

export function unBindShop(data) {
  return request({
    url: '/devvending/unBindShop',
    method: 'post',
    data
  })
}

export default {
  initGetList: initGetList,
  getList: getList,
  getListByShop: getListByShop,
  getListBySbShop: getListBySbShop,
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
  updateApp: updateApp,
  openPickupDoor: openPickupDoor,
  bindShop: bindShop,
  unBindShop: unBindShop
}
