import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/adminuser/getlist',
    method: 'get',
    params
  })
}

export function initAdd(params) {
  return request({
    url: '/adminuser/initAdd',
    method: 'get',
    params
  })
}

export function add(data) {
  return request({
    url: '/adminuser/add',
    method: 'post',
    data
  })
}

export function initEdit(params) {
  return request({
    url: '/adminuser/initEdit',
    method: 'get',
    params
  })
}

export function edit(data) {
  return request({
    url: '/adminuser/edit',
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
