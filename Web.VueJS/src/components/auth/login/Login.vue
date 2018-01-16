<template>
  <div class="login">
    <h2>Welcome!</h2>
    <form method="post" name="login" @submit.prevent="login">
      <div class="form-group">
        <div class="input-group">
          <input v-model="email" type="text" id="email" name="email" required="required"/>
          <label class="control-label" for="email">Email</label><i class="bar"></i>
        </div>
      </div>
      <div class="form-group">
        <div class="input-group">
          <input v-model="password" type="password" id="password" name="password" required="required"/>
          <label class="control-label" for="password">Password</label><i class="bar"></i>
        </div>
      </div>
      <div class="d-flex flex-column flex-lg-row align-items-center justify-content-between down-container">
        <button class="btn btn-primary" type="submit">
          Login
        </button>
        <router-link class='link' :to="{name: 'Signup'}">Create account</router-link>
      </div>
    </form>
  </div>
</template>

<script>
import axios from "axios";
import { mapActions, mapGetters } from "vuex";

export default {
  name: "login",
  data() {
    return {
      email: "",
      password: ""
    };
  },
  methods: {
    ...mapGetters(["isAuthenticated"]),
    ...mapActions(["setIsAuthenticated", "setToken"]),
    login() {
      var data = new FormData();

      data.append("email", this.email);
      data.append("password", this.password);

      axios
        .create({
          baseURL: "http://localhost:53600/api/"
        })
        .post(`auth/token`, data)
        .then(response => {
          var token = response.data.value;
          if (token) {
            // asdfasdf
            this.setIsAuthenticated(true);
            this.setToken(token);
            this.$router.push({ path: "/dashboard" });
          }
        })
        .catch(e => {
          console.log("catch: ", e);
        });
    }
  }
};
</script>

<style lang="scss">
@import "../../../sass/variables";
@import "../../../../node_modules/bootstrap/scss/mixins/breakpoints";
@import "../../../../node_modules/bootstrap/scss/variables";
.login {
  @include media-breakpoint-down(md) {
    width: 100%;
    padding-right: 2rem;
    padding-left: 2rem;
    .down-container {
      .link {
        margin-top: 2rem;
      }
    }
  }

  h2 {
    text-align: center;
  }
  width: 21.375rem;

  .down-container {
    margin-top: 3.125rem;
  }
}
</style>
