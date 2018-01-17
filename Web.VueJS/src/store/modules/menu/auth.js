/**
 * Created by yarik on 18.7.17.
 */
import lazyLoading from './lazyLoading'

export default {
  name: 'auth',
  meta: {
    expanded: false,
    title: 'Auth',
    iconClass: 'vuestic-icon vuestic-icon-auth',
    requiresAuth: false,
    showInSidebarEnabled: false
  },
  children: [
    {
      name: 'Login',
      path: '/auth/login',
      component: lazyLoading('auth/login/Login'),
      meta: {
        default: true,
        title: 'Login'
      }
    },
    {
      name: 'Register',
      path: '/auth/register',
      component: lazyLoading('auth/register/Register'),
      meta: {
        default: false,
        title: 'Register'
      }
    }
  ]
}
