import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/user/getlist',
    method: 'get',
    params
  })
}

export function initAdd(params) {
  return request({
    url: '/user/initAdd',
    method: 'get',
    params
  })
}

export function add(data) {
  return request({
    url: '/user/add',
    method: 'post',
    data
  })
}

export function initEdit(params) {
  return request({
    url: '/user/initEdit',
    method: 'get',
    params
  })
}

export function edit(data) {
  return request({
    url: '/user/edit',
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
