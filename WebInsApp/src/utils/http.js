
import Vue from 'vue'
import axios from "axios";
import qs from "qs";


//添加请求拦截器
axios.interceptors.request.use(
  config => {
    Vue.prototype.$loading.show()
    return config;
  },
  error => {
    
    return Promise.reject(error);
  }
);

//添加响应拦截器
axios.interceptors.response.use(
  response => {
    Vue.prototype.$loading.hide()
    return response;
  },
  error => {
    return Promise.resolve(error.response);
  }
);

axios.defaults.baseURL = "http://api.m.ins-uplink.com/api";
axios.defaults.headers.post["Content-Type"] = "application/json";
axios.defaults.headers.post["X-Requested-With"] = "XMLHttpRequest";
axios.defaults.timeout = 10000;

function checkStatus(response) {
  return new Promise((resolve, reject) => {
    if (
      response &&
      (response.status === 200 ||
        response.status === 304 ||
        response.status === 400)
    ) {
      console.log(response.data)
      resolve(response.data);
    } else {
      Vue.prototype.$toast('网络异常');
      Vue.prototype.$loading.hide();
    }
  });
}

export default {
  post(url, params) {
    return axios({
      method: "post",
      url,
      data: params
    }).then(response => {
      return checkStatus(response);
    });
  },
  get(url, params) {
    //params = qs.stringify(params);
    //console.log("params:"+params)
    return axios({
      method: "get",
      url,
      params
    }).then(response => {
      return checkStatus(response);
    });
  }
};