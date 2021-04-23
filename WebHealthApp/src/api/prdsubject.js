import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/prdsubject/getlist',
    method: 'get',
    params
  })
}

export function initAdd(params) {
  return request({
    url: '/prdsubject/initAdd',
    method: 'get',
    params
  })
}

export function add(data) {
  return request({
    url: '/prdsubject/add',
    method: 'post',
    data
  })
}

export function initEdit(params) {
  return request({
    url: '/prdsubject/initEdit',
    method: 'get',
    params
  })
}

export function edit(data) {
  return request({
    url: '/prdsubject/edit',
    method: 'post',
    data
  })
}

export function sort(data) {
  return request({
    url: '/prdsubject/sort',
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
