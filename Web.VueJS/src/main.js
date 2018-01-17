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
import {
  isAuthenticated
} from './store/getters';
import axios from 'axios';

// var passport = require('passport');
// var LocalStrategy = require('passport-local').LocalStrategy

// passport.use(new LocalStrategy(
//   function (email, password, done) {
//     var data = new FormData();

//     data.append("email", email);
//     data.append("password", password);

//     axios
//       .create({
//         baseURL: "http://localhost:53600/api/"
//       })
//       .post(`auth/token`, data)
//       .then(response => {
//         var token = response.data.value;
//         if (token) {
//           // asdfasdf
//           this.setIsAuthenticated(true);
//           this.setToken(token);
//           this.$router.push({
//             path: "/dashboard"
//           });
//           var user = {};
//           // get user from api
//           return done(null, user);
//         }
//         return done(null, false);
//       })
//       .catch(e => {
//         return done(null, false);
//       });
//   }
// ));

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
    if (!isAuthenticated) {
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
  next()
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
