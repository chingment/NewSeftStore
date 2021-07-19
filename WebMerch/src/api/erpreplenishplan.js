import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/erpreplenishplan/getlist',
    method: 'get',
    params
  })
}

export function initNewPlan(params) {
  return request({
    url: '/erpreplenishplan/initNew',
    method: 'get',
    params
  })
}

export function newPlan(data) {
  return request({
    url: '/erpreplenishplan/new',
    method: 'post',
    data
  })
}

export default {
  getList: getList,
  initNewPlan: initNewPlan,
  newPlan: newPlan
}
