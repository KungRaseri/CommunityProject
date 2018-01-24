<template>
  <div class="dashboard">
    <div class="row">
      <div class="col-md-12">
        <vuestic-alert v-if="alert.active" :type="alert.type.name" :withCloseBtn="alert.withCloseBtn">
          <span :class="'badge badge-pill badge-' + alert.type.badge">{{alert.type.label}}</span>
          {{alert.message}}
          <i class="fa fa-close alert-close"></i>
        </vuestic-alert>
      </div>
    </div>

    <dashboard-info-widgets></dashboard-info-widgets>

    <vuestic-widget class="no-padding no-v-padding" v-if="!user.twitchUsername">
      <vuestic-tabs :names="['Setup Profile']" ref="tabs">
        <!-- <div slot="Data Visualization">
          <data-visualisation-tab></data-visualisation-tab>
        </div>
        <div slot="Users & Members">
          <users-members-tab></users-members-tab>
        </div> -->
        <div slot="Setup Profile">
          <setup-profile-tab></setup-profile-tab>
        </div>
        <!-- <div slot="Features">
          <features-tab></features-tab>
        </div> -->
      </vuestic-tabs>
    </vuestic-widget>

    <dashboard-bottom-widgets></dashboard-bottom-widgets>

  </div>
</template>

<script>
import VuesticWidget from "../vuestic-components/vuestic-widget/VuesticWidget";
import VuesticAlert from "../vuestic-components/vuestic-alert/VuesticAlert";
import DashboardInfoWidgets from "./DashboardInfoWidgets";
import VuesticTabs from "../vuestic-components/vuestic-tabs/VuesticTabs.vue";
// import UsersMembersTab from "./users-and-members-tab/UsersMembersTab.vue";
import SetupProfileTab from "./setup-profile-tab/SetupProfileTab.vue";
// import FeaturesTab from "./features-tab/FeaturesTab.vue";
// import DataVisualisationTab from "./data-visualisation-tab/DataVisualisation.vue";
import DashboardBottomWidgets from "./DashboardBottomWidgets.vue";
import { mapGetters } from "vuex";

export default {
  name: "dashboard",
  components: {
    // DataVisualisationTab,
    VuesticWidget,
    VuesticAlert,
    DashboardInfoWidgets,
    VuesticTabs,
    // UsersMembersTab,
    SetupProfileTab,
    // FeaturesTab,
    DashboardBottomWidgets
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
