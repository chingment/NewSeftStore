import request from '@/utils/request'

export function fetchList(params) {
  return request({
    url: '/agentmenu/getlist',
    method: 'get',
    params
  })
}

export function initAddMenu(params) {
  return request({
    url: '/agentmenu/initAdd',
    method: 'get',
    params
  })
}

export function addMenu(data) {
  return request({
    url: '/agentmenu/add',
    method: 'post',
    data
  })
}

export function initEditMenu(params) {
  return request({
    url: '/agentmenu/initEdit',
    method: 'get',
    params
  })
}

export function editMenu(data) {
  return request({
    url: '/agentmenu/edit',
    method: 'post',
    data
  })
}
