<template>
  <div class="widget" :class="{'no-header': !headerText}">
    <div class="widget-header" v-if="headerText">{{headerText}}</div>
    <div class="widget-body" v-if="!isLoading">
      <slot></slot>
    </div>
    <trinity-rings-spinner v-else :animation-duration="1500" :size="33" :color="'#333'" :class="'spinner-center'" />
  </div>
</template>

<script>
import { TrinityRingsSpinner } from "epic-spinners";
export default {
  name: "vuestic-widget",
  components: {
    TrinityRingsSpinner
  },
  props: ["headerText", "isLoading"]
};
</script>

<style lang="scss">
@import "../../../sass/_variables.scss";

.spinner-center {
  margin: auto;
  margin-bottom: 10px;
}

.widget {
  background: $widget-bg;
  box-shadow: $widget-box-shadow;
  margin-bottom: $widget-mb;
  padding: 0;

  &.no-header {
    .widget-body {
      height: $widget-body-no-header-height;
    }
  }

  &.larger-padding {
    .widget-body {
      padding: $widget-larger-padding;
    }
  }

  &.no-h-padding {
    .widget-body {
      padding-left: 0;
      padding-right: 0;
    }
  }

  &.no-v-padding {
    .widget-body {
      padding-top: 0;
      padding-bottom: 0;
    }
  }

  &.brand-info {
    background-color: $brand-info;
    color: $white;
    box-shadow: $widget-info-shadow;
  }

  &.brand-danger {
    background-color: $brand-danger;
    color: $white;
    box-shadow: $widget-danger-shadow;
  }

  &.info-widget {
    border-top: $info-widget-border;

    &.brand-info {
      border-top-color: $brand-info;
    }

    &.brand-danger {
      border-top-color: $brand-danger;
    }
  }

  .widget-body {
    height: $widget-body-with-header-height;
    padding: $widget-padding;
  }

  .widget-header {
    height: $widget-header-height;
    padding: 0 $widget-padding;
    border-bottom: $widget-header-border;
    font-size: $font-size-larger;
    font-weight: 600;
    display: flex;
    align-items: center;
  }
}
</style>
