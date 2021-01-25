import request from '@/utils/request'

export function getSpaces(params) {
  return request({
    url: '/ad/getSpaces',
    method: 'get',
    params
  })
}

export function initRelease(params) {
  return request({
    url: '/ad/initRelease',
    method: 'get',
    params
  })
}

export function release(data) {
  return request({
    url: '/ad/release',
    method: 'post',
    data
  })
}

export function getContents(params) {
  return request({
    url: '/ad/getContents',
    method: 'get',
    params
  })
}

export function setContentStatus(data) {
  return request({
    url: '/ad/setContentStatus',
    method: 'post',
    data
  })
}

export function getContentBelongs(params) {
  return request({
    url: '/ad/getContentBelongs',
    method: 'get',
    params
  })
}

export function setContentBelongStatus(data) {
  return request({
    url: '/ad/setContentBelongStatus',
    method: 'post',
    data
  })
}

export function editContentBelong(data) {
  return request({
    url: '/ad/editContentBelong',
    method: 'post',
    data
  })
}

export function addContentBelong(data) {
  return request({
    url: '/ad/addContentBelong',
    method: 'post',
    data
  })
}


export default {
  getSpaces: getSpaces,
  release: release,
  getContents: getContents,
  setContentStatus: setContentStatus,
  getContentBelongs: getContentBelongs,
  setContentBelongStatus: setContentBelongStatus,
  editContentBelong: editContentBelong,
  addContentBelong:addContentBelong
}
