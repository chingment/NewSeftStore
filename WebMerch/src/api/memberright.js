import request from '@/utils/request'

export function getLevelSts(params) {
  return request({
    url: '/memberright/getLevelSts',
    method: 'get',
    params
  })
}

export function initManage(params) {
  return request({
    url: '/memberright/initManage',
    method: 'get',
    params
  })
}

export function initManageBaseInfo(params) {
  return request({
    url: '/memberright/initManageBaseInfo',
    method: 'get',
    params
  })
}

export function getFeeSts(params) {
  return request({
    url: '/memberright/getFeeSts',
    method: 'get',
    params
  })
}

export default {
  getLevelSts: getLevelSts,
  initManage: initManage,
  initManageBaseInfo: initManageBaseInfo,
  getFeeSts: getFeeSts
}
