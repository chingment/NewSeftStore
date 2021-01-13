import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/shop/getlist',
    method: 'get',
    params
  })
}

export function getDetails(params) {
  return request({
    url: '/shop/getDetails',
    method: 'get',
    params
  })
}

export function add(data) {
  return request({
    url: '/shop/add',
    method: 'post',
    data
  })
}

export function edit(data) {
  return request({
    url: '/shop/add',
    method: 'post',
    data
  })
}

export default {
  getList: getList,
  getDetails: getDetails,
  add: add,
  edit: edit
}
