import * as types from '../mutation-types'

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
  isAuthenticated: false,
  token: {},
  user: {}
}

const mutations = {
  [types.CLOSE_MENU](state) {
    if (document.documentElement.clientWidth < 992) {
      state.sidebar.opened = false
    }
  },
  [types.TOGGLE_SIDEBAR](state, opened) {
    state.sidebar.opened = opened
  },
  [types.TOGGLE_WITHOUT_ANIMATION](state, value) {
    state.sidebar.withoutAnimation = value
  },
  setLoading(state, isLoading) {
    state.isLoading = isLoading
  },
  [types.SET_IS_AUTHENTICATED](state, value) {
    state.isAuthenticated = value
  },
  [types.SET_TOKEN](state, value) {
    state.token = value
  },
  [types.SET_USER](state, value) {
    state.user = value
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
  setIsAuthenticated({
    commit
  }, value) {
    commit(types.SET_IS_AUTHENTICATED, value)
  },
  setToken({
    commit
  }, value) {
    commit(types.SET_TOKEN, value)
  },
  setUser({
    commit
  }, value) {
    commit(types.SET_USER, value)
  }
}

export default {
  state,
  mutations,
  actions
}
