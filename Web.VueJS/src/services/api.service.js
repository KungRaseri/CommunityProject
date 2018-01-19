import axios from 'axios';

axios.defaults.baseURL = "http://localhost:53600/api/";

axios.interceptors.request.use((config) => {
  var token = JSON.parse(localStorage.getItem('token'));
  if (token) {
    config.headers.Authorization = `Bearer ${token.value}`;
  }
  return config;
}, (error) => {
  return Promise.reject(error);
});

const ApiService = {
  Auth: {
    Login(credentials) {
      return new Promise((resolve, reject) => {
        var data = new FormData();

        data.append("email", credentials.email);
        data.append("password", credentials.password);

        axios
          .post(`auth/token`, data)
          .then(response => {
            resolve(response);
          }).catch(response => {
            reject(response);
          })
      });
    }
  },
  User: {
    Get() {
      return new Promise((resolve, reject) => {
        axios
          .get('user')
          .then(response => {
            resolve(response);
          }).catch(response => {
            reject(response);
          })
      });
    },
    Post(user) {
      return new Promise((resolve, reject) => {
        var data = new FormData();

        data.append(user);

        axios
          .post('user', user)
          .then(response => {
            resolve(response);
          })
          .catch(response => {
            reject(response);
          })
      })
    }
  }
};

export default ApiService;
