import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/shop/getlist',
    method: 'get',
    params
  })
}

export function getListBySbStore(params) {
  return request({
    url: '/shop/getListBySbStore',
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

export function save(data) {
  return request({
    url: '/shop/save',
    method: 'post',
    data
  })
}

export default {
  getList: getList,
  getListBySbStore: getListBySbStore,
  getDetails: getDetails,
  save: save
}
