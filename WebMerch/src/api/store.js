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

export function getProductList(params) {
  return request({
    url: '/store/getProductList',
    method: 'get',
    params
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
  getProductList: getProductList
}
