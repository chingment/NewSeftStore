import request from '@/utils/request'

export function getDetails(params) {
  return request({
    url: '/dayreport/getDetails',
    method: 'get',
    params
  })
}

export default {
  getDetails: getDetails
}