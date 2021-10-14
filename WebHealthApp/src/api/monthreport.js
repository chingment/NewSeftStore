import request from '@/utils/request'

export function getMonitor(params) {
  return request({
    url: '/monthreport/getMonitor',
    method: 'get',
    params
  })
}

export function getEnergy(params) {
  return request({
    url: '/monthreport/getEnergy',
    method: 'get',
    params
  })
}

export function getAdvise(params) {
  return request({
    url: '/monthreport/GetAdvise',
    method: 'get',
    params
  })
}

export function getTagAdvise(params) {
  return request({
    url: '/monthreport/GetTagAdvise',
    method: 'get',
    params
  })
}

export function updateVisitCount(params) {
  return request({
    url: '/monthreport/updateVisitCount',
    method: 'get',
    params
  })
}

export function getSugProducts(params) {
  return request({
    url: '/monthreport/getSugProducts',
    method: 'get',
    params
  })
}

export default {
  getAdvise: getAdvise,
  getEnergy: getEnergy,
  getMonitor: getMonitor,
  getTagAdvise: getTagAdvise,
  getSugProducts: getSugProducts,
  updateVisitCount: updateVisitCount
}
