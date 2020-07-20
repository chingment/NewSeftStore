import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/prdkind/getlist',
    method: 'get',
    params
  })
}

export function initAdd(params) {
  return request({
    url: '/prdkind/initAdd',
    method: 'get',
    params
  })
}

export function add(data) {
  return request({
    url: '/prdkind/add',
    method: 'post',
    data
  })
}

export function initEdit(params) {
  return request({
    url: '/prdkind/initEdit',
    method: 'get',
    params
  })
}

export function edit(data) {
  return request({
    url: '/prdkind/edit',
    method: 'post',
    data
  })
}

export function sort(data) {
  return request({
    url: '/prdkind/sort',
    method: 'post',
    data
  })
}

export default {
  getList: getList,
  initAdd: initAdd,
  add: add,
  initEdit: initEdit,
  edit: edit
}
