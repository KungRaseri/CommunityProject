// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import BootstrapVue from 'bootstrap-vue'
import VeeValidate from 'vee-validate'
import App from './App'
import store from './store'
import router from './router'
import {
  sync
} from 'vuex-router-sync'
import VuesticPlugin from 'src/components/vuestic-components/vuestic-components-plugin'
import axios from 'axios';

Vue.prototype.$ax = axios
  .create({
    baseURL: "http://localhost:53600/api/",
    headers: [
      "Access-Control-Allow-Origin: *"
    ]
  });
Vue.use(VuesticPlugin)
Vue.use(BootstrapVue)

// NOTE: workaround for VeeValidate + vuetable-2
Vue.use(VeeValidate, {
  fieldsBagName: 'formFields'
})

sync(store, router)

let mediaHandler = () => {
  if (window.matchMedia(store.getters.config.windowMatchSizeLg).matches) {
    store.dispatch('toggleSidebar', true)
  } else {
    store.dispatch('toggleSidebar', false)
  }
}

router.beforeEach((to, from, next) => {
  store.commit('setLoading', true)
  if (to.matched.some(route => route.meta.requiresAuth)) {
    console.log('test', 'is this working', '--- ' + to.meta.requiresAuth, store.getters.isAuthenticated);
    if (!store.getters.isAuthenticated) {
      next({
        name: 'Login',
        query: {
          redirect: to.fullPath
        }
      });
    } else {
      next();
    }
  } else {
    next();
  }
})

router.afterEach((to, from) => {
  mediaHandler()
  store.commit('setLoading', false)
})

/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  store,
  template: '<App/>',
  components: {
    App
  }
})
