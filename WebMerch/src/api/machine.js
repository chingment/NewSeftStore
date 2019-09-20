import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/machine/getlist',
    method: 'get',
    params
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

export function manageStockGetStockList(params) {
  return request({
    url: '/machine/manageStockGetStockList',
    method: 'get',
    params
  })
}

export default {
  getList: getList,
  initManage: initManage,
  initManageBaseInfo: initManageBaseInfo,
  initManageStock: initManageStock,
  manageStockGetStockList: manageStockGetStockList
}
