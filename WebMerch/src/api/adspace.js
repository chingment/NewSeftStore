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

export function deleteAdContent(params) {
  return request({
    url: '/adspace/deleteAdContent',
    method: 'get',
    params
  })
}

export function getAdContentBelongs(params) {
  return request({
    url: '/adspace/getAdContentBelongs',
    method: 'get',
    params
  })
}

export function setAdContentBelongStatus(data) {
  return request({
    url: '/adspace/setAdContentBelongStatus',
    method: 'post',
    data
  })
}

getAdContentBelongs
export default {
  getList: getList,
  release: release,
  getReleaseList: getReleaseList,
  deleteAdContent: deleteAdContent,
  getAdContentBelongs: getAdContentBelongs,
  setAdContentBelongStatus: setAdContentBelongStatus
}
