import Vue from 'vue'
import Vuex from 'vuex'
Vue.use(Vuex)

const store = new Vuex.Store({
  state: {
    isLoading: false// loading全局开关
  },
  mutations: {
    changeisLoading(state, data) {
      state.isLoading = data
    }
  }
})

export default store
