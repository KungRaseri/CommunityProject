<template>
  <div class="profile">
    <div class="row">
      <div class="col-md-12">
        <vuestic-alert v-if="alert.active" :type="alert.type.name" :withCloseBtn="alert.withCloseBtn">
          <span :class="'badge badge-pill badge-' + alert.type.badge">{{alert.type.label}}</span>
          {{alert.message}}
          <i class="fa fa-close alert-close"></i>
        </vuestic-alert>
      </div>
    </div>
    <vuestic-profile-card :name="user.twitchUsername || 'Unknown'" 
                      photoSource="http://via.placeholder.com/350x350"
                      :social="{twitter: 'twitter.com', facebook: 'facebook.com', instagram: 'instagram.com'}">
    </vuestic-profile-card>
  </div>
</template>

<script>
import VuesticWidget from "../vuestic-components/vuestic-widget/VuesticWidget";
import VuesticAlert from "../vuestic-components/vuestic-alert/VuesticAlert";
import { mapGetters } from "vuex";

export default {
  name: "profile",
  components: {
    VuesticWidget,
    VuesticAlert
  },
  data() {
    return {
      alert: {
        active: false,
        message: "",
        withCloseBtn: true,
        type: {
          name: "",
          label: "",
          badge: ""
        }
      }
    };
  },
  computed: {
    ...mapGetters(["user"])
  }
};
</script>

<style lang="scss" scoped>
@import "../../sass/_variables.scss";
</style>
