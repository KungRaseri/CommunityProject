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
  isAuthenticated: !!localStorage.getItem('token'),
  token: localStorage.getItem('token') || {},
  user: localStorage.getItem('user') || {}
}

const mutations = {
  [types.LOGIN](state) {
  },
  [types.LOGIN_SUCCESS](state, value) {
    state.isAuthenticated = true;
    state.user = value.user;
    state.token = value.token;
  },
  [types.LOGOUT](state) {
    state.isAuthenticated = false;
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
        localStorage.setItem("user", value.user);
        localStorage.setItem("token", value.token);
        commit(types.LOGIN_SUCCESS, value);
        resolve();
      }, 1000);
    });
  },
  logout({
    commit
  }) {
    localStorage.removeItem("user");
    localStorage.removeItem("token");
    commit(types.LOGOUT);
  }
}

export default {
  state,
  mutations,
  actions
}
