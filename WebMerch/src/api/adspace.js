import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/adspace/getlist',
    method: 'get',
    params
  })
}

export function getReleaseList(params) {
  return request({
    url: '/adspace/getReleaselist',
    method: 'get',
    params
  })
}

export function initRelease(params) {
  return request({
    url: '/adspace/initRelease',
    method: 'get',
    params
  })
}

export function release(data) {
  return request({
    url: '/adspace/release',
    method: 'post',
    data
  })
}

export default {
  getList: getList,
  release: release,
  getReleaseList: getReleaseList
}
