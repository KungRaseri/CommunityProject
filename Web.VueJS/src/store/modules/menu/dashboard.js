import lazyLoading from './lazyLoading'

export default {
  name: 'Dashboard',
  meta: {
    default: false,
    expanded: false,
    title: 'Dashboard',
    iconClass: 'vuestic-icon vuestic-icon-dashboard',
    showInSidebarEnabled: true
  },
  children: [
    {
      name: 'Dashboard',
      path: '/dashboard',
      component: lazyLoading('dashboard/Dashboard'),
      meta: {
        default: false,
        title: 'Dashboard',
        requiresAuth: true
      }
    },
    {
      name: 'Profile',
      path: '/dashboard/profile',
      component: lazyLoading('dashboard/Profile'),
      meta: {
        default: false,
        title: 'Profile',
        requiresAuth: true
      }
    },
    {
      name: 'Loyalty',
      path: '/dashboard/loyalty',
      component: lazyLoading('dashboard/Loyalty'),
      meta: {
        default: false,
        title: 'Loyalty',
        requiresAuth: true
      }
    }

  ]
}
