import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/prdproduct/getlist',
    method: 'get',
    params
  })
}

export function initAdd(params) {
  return request({
    url: '/prdproduct/initAdd',
    method: 'get',
    params
  })
}

export function add(data) {
  return request({
    url: '/prdproduct/add',
    method: 'post',
    data
  })
}

export function initEdit(params) {
  return request({
    url: '/prdproduct/initEdit',
    method: 'get',
    params
  })
}

export function edit(data) {
  return request({
    url: '/prdproduct/edit',
    method: 'post',
    data
  })
}

export function getOnSaleStores(params) {
  return request({
    url: '/prdproduct/getOnSaleStores',
    method: 'get',
    params
  })
}

export function editSalePriceOnStore(data) {
  return request({
    url: '/prdproduct/EditSalePriceOnStore',
    method: 'post',
    data
  })
}

export function search(params) {
  return request({
    url: '/prdproduct/search',
    method: 'get',
    params
  })
}

export function getSpecs(params) {
  return request({
    url: '/prdproduct/getSpecs',
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
  search: search,
  getSpecs: getSpecs
}
