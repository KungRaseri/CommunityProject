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
        <router-link class='link' :to="{name: 'Register'}">Create account</router-link>
      </div>
    </form>
  </div>
</template>

<script>
import { mapGetters } from "vuex";

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
    login() {
      this.$api.Auth.Login({ email: this.email, password: this.password })
        .then(response => {
          var resValue = response.data.value;
          if (resValue.token) {
            this.$store.dispatch("login", resValue).then(() => {
              if (this.$route.query) {
                var redirect = this.$route.query.redirect;
                if (redirect) {
                  this.$router.push({ path: redirect });
                }
                return;
              }
              this.$router.push({ name: "Dashboard" });
            });
          } else {
            this.$store.dispatch("logout").then(() => {
              this.$router.push({ name: "Login" });
            });
          }
        })
        .catch(e => {
          // report error to sentry
          console.log("error", e);
        });
    }
  },
  beforeMount() {
    if (this.isAuthenticated) {
      this.$router.push({ name: "Dashboard" });
      return;
    }
    this.$store.dispatch("logout");
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
