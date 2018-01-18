import lazyLoading from './lazyLoading'

export default {
  name: 'Dashboard',
  meta: {
    default: false,
    title: 'Dashboard',
    iconClass: 'vuestic-icon vuestic-icon-dashboard',
    requiresAuth: true,
    showInSidebarEnabled: false
  },
  children: [
    {
      name: 'Dashboard',
      path: '/dashboard',
      component: lazyLoading('dashboard/Dashboard'),
      meta: {
        default: false,
        title: 'Dashboard'
      }
    },
    {
      name: 'Profile',
      path: '/dashboard/profile',
      component: lazyLoading('dashboard/Profile'),
      meta: {
        default: false,
        title: 'Profile'
      }
    }
  ]
}
