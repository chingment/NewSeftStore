import request from '@/utils/request'

export function lyingIn(params) {
  return request({
    url: '/imitate/lyingin',
    method: 'get',
    params
  })
}

export default {
  lyingIn: lyingIn
}
