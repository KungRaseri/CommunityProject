import * as types from '../mutation-types'
import {
  setTimeout
} from 'timers';

const state = {
  sidebar: {
    opened: false,
    withoutAnimation: false
  },
  config: {
    googleMaps: {
      apiKey: 'AIzaSyBNAqPrTQoz9P4NBlDDyfxrnKiafkaL8iQ'
    },
    windowMatchSizeLg: '(min-width: 992px)',
    palette: {
      primary: '#4ae387',
      danger: '#e34a4a',
      info: '#4ab2e3',
      success: '#db76df',
      warning: '#f7cc36',
      white: '#fff',
      black: '#000',
      fontColor: '#34495e',
      transparent: 'transparent',
      lighterGray: '#ddd'
    }
  },
  isLoading: true,
  isAuthenticated: (JSON.parse(localStorage.getItem('token'))) ? JSON.parse(localStorage.getItem('token')).expiration > Date.now() : false,
  token: JSON.parse(localStorage.getItem('token')) || {},
  user: JSON.parse(localStorage.getItem('user')) || {}
}

const mutations = {
  [types.LOGIN](state) {},
  [types.LOGIN_SUCCESS](state, value) {
    state.isAuthenticated = true;
    state.user = value.user;
    state.token = value.token;
  },
  [types.LOGOUT](state) {
    state.isAuthenticated = false;
    state.user = {};
    state.token = {};
  },
  [types.CLOSE_MENU](state) {
    if (document.documentElement.clientWidth < 992) {
      state.sidebar.opened = false;
    }
  },
  [types.TOGGLE_SIDEBAR](state, opened) {
    state.sidebar.opened = opened;
  },
  [types.TOGGLE_WITHOUT_ANIMATION](state, value) {
    state.sidebar.withoutAnimation = value;
  },
  setLoading(state, isLoading) {
    state.isLoading = isLoading;
  }
}

const actions = {
  closeMenu({
    commit
  }) {
    commit(types.CLOSE_MENU)
  },
  toggleSidebar({
    commit
  }, opened) {
    commit(types.TOGGLE_SIDEBAR, opened)
  },
  isToggleWithoutAnimation({
    commit
  }, value) {
    commit(types.TOGGLE_WITHOUT_ANIMATION, value)
  },
  login({
    commit
  }, value) {
    commit(types.LOGIN);
    return new Promise(resolve => {
      setTimeout(() => {
        localStorage.setItem("user", JSON.stringify(value.user));
        localStorage.setItem("token", JSON.stringify(value.token));
        commit(types.LOGIN_SUCCESS, value);
        resolve();
      }, 1000);
    });
  },
  logout({
    commit
  }) {
    localStorage.clear();
    commit(types.LOGOUT);
  },
  verifyTokenExpiration({
    commit
  }) {
    return new Promise(resolve => {
      var token = JSON.parse(localStorage.getItem('token'));
      var now = new Date();
      var utc = Date.UTC(now.getFullYear(), now.getMonth(), now.getDate(), now.getHours(), now.getMinutes(), now.getSeconds(), now.getMilliseconds());
      if (token) {
        console.log('expiration', Date.parse(token.expiration));
        console.log('utc', utc);
        console.log('verify', Date.parse(token.expiration) < utc);
        if (Date.parse(token.expiration) < utc) {
          commit(types.LOGOUT);
          resolve(false);
        } else {
          resolve(true);
        }
      } else {
        resolve(false);
      }
    });
  }
}

export default {
  state,
  mutations,
  actions
}
