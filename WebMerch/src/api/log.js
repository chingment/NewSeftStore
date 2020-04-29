import request from '@/utils/request'

export function initListByOperate(params) {
  return request({
    url: '/log/initListByOperate',
    method: 'get',
    params
  })
}

export function getListByOperate(params) {
  return request({
    url: '/log/getListByOperate',
    method: 'get',
    params
  })
}

export function initListByStock(params) {
  return request({
    url: '/log/initListByStock',
    method: 'get',
    params
  })
}

export function getListByStock(params) {
  return request({
    url: '/log/getListByStock',
    method: 'get',
    params
  })
}

export function getListByRelStock(params) {
  return request({
    url: '/log/getListByRelStock',
    method: 'get',
    params
  })
}

export default {
  initListByOperate: initListByOperate,
  getListByOperate: getListByOperate,
  initListByStock: initListByStock,
  getListByStock: getListByStock,
  getListByRelStock: getListByRelStock
}
