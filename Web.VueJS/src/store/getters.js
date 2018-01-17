const menuItems = state => state.menu.items
const sidebarOpened = state => state.app.sidebar.opened
const toggleWithoutAnimation = state => state.app.sidebar.withoutAnimation
const config = state => state.app.config
const palette = state => state.app.config.palette
const isLoading = state => state.app.isLoading
const isAuthenticated = state => state.app.isAuthenticated
const token = state => state.app.token
const user = state => state.app.user

export {
  menuItems,
  toggleWithoutAnimation,
  sidebarOpened,
  config,
  palette,
  isLoading,
  isAuthenticated,
  token,
  user
}
