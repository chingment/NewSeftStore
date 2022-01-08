import axios from 'axios'

import store from '@/store'
import { getToken } from '@/utils/auth'

axios.defaults.retry = 4
axios.defaults.retryDelay = 1000

// create an axios instance
const service = axios.create({
  baseURL: process.env.VUE_APP_BASE_API, // url = base url + request url
  // withCredentials: true, // send cookies when cross-domain requests
  timeout: 10000 // request timeout
})

// request interceptor
service.interceptors.request.use(
  config => {
    store.state.isLoading = true
    var token = getToken()
    console.log('token:' + token)
    config.headers['X-Token'] = token
    return config
  },
  error => {
    console.log(error) // for debug
    return Promise.reject(error)
  }
)

// response interceptor
service.interceptors.response.use(
  /**
   * If you want to get http information such as headers or status
   * Please return  response => response
  */

  /**
   * Determine the request status by custom code
   * Here is just an example
   * You can also judge the status by HTTP Status Code
   */
  response => {
    store.state.isLoading = false
    const res = response.data
    // console.log(JSON.stringify(res))
    // if the custom code is not 20000, it is judged as an error.
    if (res.result === 1 || res.result === 2) {
      if (res.result === 2) {
        if (res.code === 5000) {

        }
      }

      return res
    } else {
      return Promise.reject(new Error(res.message || 'Error'))
    }
  },
  error => {
    // console.log('err' + error) // for debug
    // Message({
    //   message: error.message,
    //   type: 'error',
    //   duration: 5 * 1000
    // })
    // return Promise.reject(error)

    var config = error.config

    // 如果config不存在或未设置重试选项，请拒绝
    if (!config || !config.retry) return Promise.reject('Error')
    // 设置变量跟踪重试次数
    config.__retryCount = config.__retryCount || 0
    // 检查是否已经达到最大重试总次数
    if (config.__retryCount >= config.retry) {
      // 抛出错误信息
      return Promise.reject(error)
    }
    // 增加请求重试次数
    config.__retryCount += 1
    // 创建新的异步请求
    var backoff = new Promise(function(resolve) {
      setTimeout(function() {
        resolve()
      }, config.retryDelay || 1)
    })
    // 返回axios信息，重新请求
    return backoff.then(function() {
      return axios(config)
    })
  }
)

export default service
