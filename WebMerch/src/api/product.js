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

export function getListBySale(params) {
  return request({
    url: '/product/getListBySale',
    method: 'get',
    params
  })
}

export function editSale(data) {
  return request({
    url: '/product/editSale',
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

export function del(data) {
  return request({
    url: '/product/delete',
    method: 'post',
    data
  })
}

export default {
  getList: getList,
  initAdd: initAdd,
  add: add,
  initEdit: initEdit,
  edit: edit,
  getListBySale: getListBySale,
  editSale: editSale,
  searchSpu: searchSpu,
  searchSku: searchSku,
  getSpecs: getSpecs,
  del: del
}
