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

export function getTasks(params) {
  return request({
    url: '/senviv/getTasks',
    method: 'get',
    params
  })
}

export function getDayReports(params) {
  return request({
    url: '/senviv/getDayReports',
    method: 'get',
    params
  })
}

export function getDayReportDetail(params) {
  return request({
    url: '/senviv/getDayReportDetail',
    method: 'get',
    params
  })
}

export function getStageReports(params) {
  return request({
    url: '/senviv/getStageReports',
    method: 'get',
    params
  })
}

export function getStageReportDetail(params) {
  return request({
    url: '/senviv/getStageReportDetail',
    method: 'get',
    params
  })
}

export function saveStageReportSug(data) {
  return request({
    url: '/senviv/saveStageReportSug',
    method: 'post',
    data
  })
}

export function getStageReportSug(params) {
  return request({
    url: '/senviv/getStageReportSug',
    method: 'get',
    params
  })
}

export function getTagExplains(params) {
  return request({
    url: '/senviv/getTagExplains',
    method: 'get',
    params
  })
}

export function saveTagExplain(data) {
  return request({
    url: '/senviv/saveTagExplain',
    method: 'post',
    data
  })
}

export function saveVisitRecordByTelePhone(data) {
  return request({
    url: '/senviv/saveVisitRecordByTelePhone',
    method: 'post',
    data
  })
}

export function saveVisitRecordByPapush(data) {
  return request({
    url: '/senviv/saveVisitRecordByPapush',
    method: 'post',
    data
  })
}

export function getVisitRecords(params) {
  return request({
    url: '/senviv/getVisitRecords',
    method: 'get',
    params
  })
}

export default {
  getVisitRecords: getVisitRecords,
  getUsers: getUsers,
  getUserDetail: getUserDetail,
  getTasks: getTasks,
  getDayReports: getDayReports,
  getDayReportDetail: getDayReportDetail,
  getStageReports: getStageReports,
  getStageReportDetail: getStageReportDetail,
  saveStageReportSug: saveStageReportSug,
  getStageReportSug: getStageReportSug,
  getTagExplains: getTagExplains,
  saveTagExplain: saveTagExplain,
  saveVisitRecordByTelePhone: saveVisitRecordByTelePhone,
  saveVisitRecordByPapush: saveVisitRecordByPapush
}
