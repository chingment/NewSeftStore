import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/product/getlist',
    method: 'get',
    params
  })
}

export function initAdd(params) {
  return request({
    url: '/product/initAdd',
    method: 'get',
    params
  })
}

export function add(data) {
  return request({
    url: '/product/add',
    method: 'post',
    data
  })
}

export function initEdit(params) {
  return request({
    url: '/product/initEdit',
    method: 'get',
    params
  })
}

export function edit(data) {
  return request({
    url: '/product/edit',
    method: 'post',
    data
  })
}

export function getOnSaleStores(params) {
  return request({
    url: '/product/getOnSaleStores',
    method: 'get',
    params
  })
}

export function editSalePriceOnStore(data) {
  return request({
    url: '/product/editSalePriceOnStore',
    method: 'post',
    data
  })
}

export function searchSku(params) {
  return request({
    url: '/product/searchSku',
    method: 'get',
    params
  })
}

export function searchSpu(params) {
  return request({
    url: '/product/searchSpu',
    method: 'get',
    params
  })
}

export function getSpecs(params) {
  return request({
    url: '/product/getSpecs',
    method: 'get',
    params
  })
}

export default {
  getList: getList,
  initAdd: initAdd,
  add: add,
  initEdit: initEdit,
  edit: edit,
  getOnSaleStores: getOnSaleStores,
  editSalePriceOnStore: editSalePriceOnStore,
  searchSpu: searchSpu,
  searchSku: searchSku,
  getSpecs: getSpecs
}
