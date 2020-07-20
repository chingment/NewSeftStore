import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/merchprdkind/getlist',
    method: 'get',
    params
  })
}

export function initAdd(params) {
  return request({
    url: '/merchprdkind/initAdd',
    method: 'get',
    params
  })
}

export function add(data) {
  return request({
    url: '/merchprdkind/add',
    method: 'post',
    data
  })
}

export function initEdit(params) {
  return request({
    url: '/merchprdkind/initEdit',
    method: 'get',
    params
  })
}

export function edit(data) {
  return request({
    url: '/merchprdkind/edit',
    method: 'post',
    data
  })
}

export function sort(data) {
  return request({
    url: '/merchprdkind/sort',
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
