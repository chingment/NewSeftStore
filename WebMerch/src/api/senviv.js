import request from '@/utils/request'

export function getUsers(params) {
  return request({
    url: '/senviv/getUsers',
    method: 'get',
    params
  })
}

export function getUserDetail(params) {
  return request({
    url: '/senviv/getUserDetail',
    method: 'get',
    params
  })
}

export default {
  getUsers: getUsers,
  getUserDetail: getUserDetail
}
