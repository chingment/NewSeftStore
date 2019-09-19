import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/machine/getlist',
    method: 'get',
    params
  })
}

export function initStock(params) {
  return request({
    url: '/machine/initStock',
    method: 'get',
    params
  })
}

export function getStockList(params) {
  return request({
    url: '/machine/getStockList',
    method: 'get',
    params
  })
}

export default {
  getList: getList,
  initStock: initStock,
  getStockList: getStockList
}
