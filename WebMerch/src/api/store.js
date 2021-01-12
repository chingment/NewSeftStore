import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/store/getlist',
    method: 'get',
    params
  })
}

export function initAdd(params) {
  return request({
    url: '/store/initAdd',
    method: 'get',
    params
  })
}

export function add(data) {
  return request({
    url: '/store/add',
    method: 'post',
    data
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

export function edit(data) {
  return request({
    url: '/store/edit',
    method: 'post',
    data
  })
}

export function getMachineList(params) {
  return request({
    url: '/store/getMachineList',
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

export function initManageFront(params) {
  return request({
    url: '/store/initManageFront',
    method: 'get',
    params
  })
}

export function getFrontList(params) {
  return request({
    url: '/store/getFrontList',
    method: 'get',
    params
  })
}

export function getFront(params) {
  return request({
    url: '/store/getFront',
    method: 'get',
    params
  })
}

export function saveFront(data) {
  return request({
    url: '/store/saveFront',
    method: 'post',
    data
  })
}

export default {
  getList: getList,
  initAdd: initAdd,
  add: add,
  initManageBaseInfo: initManageBaseInfo,
  edit: edit,
  initManage: initManage,
  getMachineList: getMachineList,
  getKinds: getKinds,
  saveKind: saveKind,
  removeKind: removeKind,
  saveKindSpu: saveKindSpu,
  getKindSpus: getKindSpus,
  removeKindSpu: removeKindSpu,
  getKindSpu: getKindSpu,
  initManageFront: initManageFront,
  getFrontList: getFrontList,
  getFront: getFront,
  saveFront: saveFront
}
