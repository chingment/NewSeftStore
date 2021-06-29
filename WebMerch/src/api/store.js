import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/store/getlist',
    method: 'get',
    params
  })
}

export function initManage(params) {
  return request({
    url: '/store/initManage',
    method: 'get',
    params
  })
}

export function initManageBaseInfo(params) {
  return request({
    url: '/store/initManageBaseInfo',
    method: 'get',
    params
  })
}

export function getDevices(params) {
  return request({
    url: '/store/getDevices',
    method: 'get',
    params
  })
}

export function getKinds(params) {
  return request({
    url: '/store/getKinds',
    method: 'get',
    params
  })
}

export function saveKind(data) {
  return request({
    url: '/store/saveKind',
    method: 'post',
    data
  })
}

export function removeKind(data) {
  return request({
    url: '/store/removeKind',
    method: 'post',
    data
  })
}

export function saveKindSpu(data) {
  return request({
    url: '/store/saveKindSpu',
    method: 'post',
    data
  })
}

export function getKindSpus(params) {
  return request({
    url: '/store/getKindSpus',
    method: 'get',
    params
  })
}

export function removeKindSpu(data) {
  return request({
    url: '/store/removeKindSpu',
    method: 'post',
    data
  })
}

export function getKindSpu(params) {
  return request({
    url: '/store/getKindSpu',
    method: 'get',
    params
  })
}

export function initManageShop(params) {
  return request({
    url: '/store/initManageShop',
    method: 'get',
    params
  })
}

export function getShops(params) {
  return request({
    url: '/store/getShops',
    method: 'get',
    params
  })
}

export function addShop(data) {
  return request({
    url: '/store/addShop',
    method: 'post',
    data
  })
}

export function removeShop(data) {
  return request({
    url: '/store/removeShop',
    method: 'post',
    data
  })
}

export default {
  getList: getList,
  initManage: initManage,
  initManageBaseInfo: initManageBaseInfo,
  getKinds: getKinds,
  saveKind: saveKind,
  removeKind: removeKind,
  saveKindSpu: saveKindSpu,
  getKindSpus: getKindSpus,
  removeKindSpu: removeKindSpu,
  getKindSpu: getKindSpu,
  getShops: getShops,
  addShop: addShop,
  removeShop: removeShop,
  getDevices: getDevices
}
