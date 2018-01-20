<template>
  <div class="signup">
    <h2>Create New Account</h2>
    <form method="post" name="signup" @submit.prevent="register">
      <div class="form-group">
        <div class="input-group">
          <input v-model="email" type="text" id="email" required="required"/>
          <label class="control-label" for="email">Email</label><i class="bar"></i>
        </div>
      </div>
      <div class="form-group">
        <div class="input-group">
          <input v-model="password" type="password" id="password" required="required"/>
          <label class="control-label" for="password">Password</label><i class="bar"></i>
        </div>
      </div>
      <!-- <div class="abc-checkbox abc-checkbox-primary">
        <input id="checkbox1" type="checkbox" checked>
        <label for="checkbox1">
          <span class="abc-label-text">I agree to <router-link to="">Terms of Use.</router-link></span>
        </label>
      </div> -->
      <div class="d-flex flex-column flex-lg-row align-items-center justify-content-between down-container">
        <button class="btn btn-primary" type="submit">
          Register
        </button>
        <router-link class='link' :to="{name: 'Login'}">Already joined?</router-link>
      </div>
    </form>
  </div>
</template>

<script>
import { mapGetters, mapActions } from "vuex";

export default {
  name: "register",
  data() {
    return {
      email: "",
      password: ""
    };
  },
  computed: {
    ...mapGetters(["isAuthenticated"])
  },
  methods: {
    ...mapActions(["login"]),
    register() {
      var data = new FormData();

      data.append("email", this.email);
      data.append("password", this.password);

      this.$api.Auth.Register(data)
        .then(response => {
          var user = response.data.value;
          if (user) {
            this.$api.Auth.Login(data).then(response => {
              var resValue = response.data.value;
              if (resValue.token) {
                this.$store.dispatch("login", resValue).then(() => {
                  this.$router.push({ name: "Dashboard" });
                });
              } else {
                this.$store.dispatch("logout").then(() => {
                  this.$router.push({ name: "Login" });
                });
              }
            });
            return;
          }
          this.$store.dispatch("logout").then(() => {
            this.$router.push({ name: "Login" });
          });
        })
        .catch(e => {
          console.log(e);
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

.signup {
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
    margin-top: 2.6875rem;
  }
}
</style>
