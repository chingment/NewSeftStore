import request from '@/utils/request'

export function getInitData(params) {
  return request({
    url: '/home/getInitData',
    method: 'get',
    params
  })
}

export function saveWorkBench(data) {
  return request({
    url: '/home/saveWorkBench',
    method: 'post',
    data
  })
}

export default {
  getInitData: getInitData,
  saveWorkBench: saveWorkBench
}
