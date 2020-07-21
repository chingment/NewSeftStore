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

export function initManageProduct(params) {
  return request({
    url: '/store/initManageProduct',
    method: 'get',
    params
  })
}

export function manageProductGetProductList(params) {
  return request({
    url: '/store/manageProductGetProductList',
    method: 'get',
    params
  })
}

export function initManageMachine(params) {
  return request({
    url: '/store/initManageMachine',
    method: 'get',
    params
  })
}

export function manageMachineGetMachineList(params) {
  return request({
    url: '/store/manageMachineGetMachineList',
    method: 'get',
    params
  })
}

export function addMachine(data) {
  return request({
    url: '/store/addMachine',
    method: 'post',
    data
  })
}

export function removeMachine(data) {
  return request({
    url: '/store/removeMachine',
    method: 'post',
    data
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

export default {
  getList: getList,
  initAdd: initAdd,
  add: add,
  initManageBaseInfo: initManageBaseInfo,
  edit: edit,
  initManage: initManage,
  initManageProduct: initManageProduct,
  manageProductGetProductList: manageProductGetProductList,
  initManageMachine: initManageMachine,
  manageMachineGetMachineList: manageMachineGetMachineList,
  removeMachine: removeMachine,
  addMachine: addMachine,
  getKinds: getKinds,
  saveKind: saveKind,
  removeKind: removeKind,
  saveKindSpu: saveKindSpu,
  getKindSpus: getKindSpus,
  removeKindSpu: removeKindSpu
}
