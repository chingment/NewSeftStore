import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/clientuser/getlist',
    method: 'get',
    params
  })
}

export function initDetails(params) {
  return request({
    url: '/clientuser/initDetails',
    method: 'get',
    params
  })
}

export function initDetailsBaseInfo(params) {
  return request({
    url: '/clientuser/initDetailsBaseInfo',
    method: 'get',
    params
  })
}

export function edit(data) {
  return request({
    url: '/clientuser/edit',
    method: 'post',
    data
  })
}

export function getAvatars(data) {
  return request({
    url: '/clientuser/getAvatars',
    method: 'post',
    data
  })
}

export default {
  getList: getList,
  edit: edit,
  initDetails: initDetails,
  initDetailsBaseInfo: initDetailsBaseInfo,
  getAvatars: getAvatars
}
