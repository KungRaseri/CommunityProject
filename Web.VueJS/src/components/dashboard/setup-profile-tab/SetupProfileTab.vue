<template>
  <div class="setup-profile-tab dashboard-tab">
    <vuestic-wizard
      :steps="steps"
      wizard-layout="horizontal"
      :wizard-type="wizardType">
      <div slot="page1" class="form-wizard-tab-content">
        <h4>Type your Twitch username</h4>
        <p>This will us recognize who you are on the website, soon™ we'll have you log in with your Twitch account.</p>
        <div class="form-group with-icon-right" :class="{'has-error': errors.has('username'), 'valid': isFormFieldValid('username')}">
          <div class="input-group">
            <input
              type="text"
              name="username"
              v-model="username"
              v-validate="'required'"
              required="required"/>
            <i class="fa fa-exclamation-triangle error-icon icon-right input-icon"></i>
            <i class="fa fa-check valid-icon icon-right input-icon"></i>
            <label class="control-label">Username</label><i class="bar"></i>
            <small v-show="errors.has('username')" class="help text-danger">{{ errors.first('username') }}</small>
          </div>
        </div>
      </div>
      <!-- <div slot="page2" class="form-wizard-tab-content">
        <h4>Select your country</h4>
        <p>Zebras communicate with facial expressions and sounds. They make loud braying or barking sounds and
          soft snorting sounds. The position of their ears, how wide open their eyes are, and whether they show
          their teeth all send a signal. For example, ears flat back means trouble, or "you better follow orders!"</p>
        <vuestic-simple-select
          label="Select country"
          v-model="selectedCountry"
          name="country"
          :required="true"
          ref="selectedCountrySelect"
          v-bind:options="countriesList">
        </vuestic-simple-select>
      </div>
      <div slot="page3" class="form-wizard-tab-content">
        <h4>Confirm selection</h4>
        <p>
          Zebras communicate with facial expressions and sounds. They make loud braying or barking sounds and
          soft snorting sounds. The position of their ears, how wide open their eyes are, and whether they show
          their teeth all send a signal. For example, ears flat back means trouble, or "you better follow orders!"
        </p>
      </div> -->
      <div slot="wizardCompleted" class="form-wizard-tab-content wizard-completed-tab">
        <h4>Thank you!</h4>
        <p>
          You entered {{username}}. Is this correct?
        </p>
        <input>
      </div>
    </vuestic-wizard>
  </div>
</template>

<script>
import VuesticWizard from "components/vuestic-components/vuestic-wizard/VuesticWizard";
import VuesticSimpleSelect from "components/vuestic-components/vuestic-simple-select/VuesticSimpleSelect";
// import CountriesList from './CountriesList'

export default {
  name: "setup-profile-tab",
  components: {
    VuesticWizard,
    VuesticSimpleSelect
  },
  props: {
    wizardType: {
      default: "rich"
    }
  },
  data() {
    return {
      steps: [
        {
          label: "Step 1. Twitch Username",
          slot: "page1",
          onNext: () => {
            this.validateFormField("username");
          },
          isValid: () => {
            return this.isFormFieldValid("username");
          }
        }
      ],
      username: ""
      // selectedCountry: '',
      // countriesList: CountriesList
    };
  },
  methods: {
    isFormFieldValid(field) {
      let isValid = false;
      if (this.formFields[field]) {
        isValid =
          this.formFields[field].validated && this.formFields[field].valid;
      }
      return isValid;
    },
    validateFormField(fieldName) {
      this.$validator.validate(fieldName, this[fieldName]);
    }
  }
};
</script>

<style lang="scss" scoped>
@import "../../../sass/_variables.scss";
@import "../../../../node_modules/bootstrap/scss/variables";
@import "../../../../node_modules/bootstrap/scss/mixins/breakpoints";

.form-group {
  min-width: 200px;
  max-width: 360px;
  width: 80%;
}

.wizard-completed-tab {
  @include media-breakpoint-up(md) {
    margin-top: -$tab-content-pt;
  }
}
</style>
