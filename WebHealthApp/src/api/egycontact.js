import request from '@/utils/request'

export function save(data) {
  return request({
    url: '/egyContact/save',
    method: 'post',
    data
  })
}

export function getDetails(params) {
  return request({
    url: '/egyContact/getDetails',
    method: 'get',
    params
  })
}

export default {
  getDetails: getDetails,
  save: save
}
