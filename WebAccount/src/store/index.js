import Vue from 'vue'
import Vuex from 'vuex'
import getters from './getters'
import app from './modules/app'
import settings from './modules/settings'
import own from './modules/own'

Vue.use(Vuex)

const store = new Vuex.Store({
  modules: {
    app,
    settings,
    own
  },
  getters
})

export default store
