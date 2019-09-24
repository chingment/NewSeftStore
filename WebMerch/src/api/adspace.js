import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/adspace/getlist',
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

export default {
  getList: getList,
  initRelease: initRelease
}
