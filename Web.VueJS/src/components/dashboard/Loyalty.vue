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
    <data-table 
      :tableFields="tableFields"
      :itemsPerPage="itemsPerPage"
      :sortFunctions="sortFunctions"
      :apiMode="false"
      :apiUrl="''"
      :paginationPath="paginationPath"
      :data="topPoints.users"
      :dataTotal="topPoints._total">
    </data-table>

    <data-table 
      :tableFields="tableFields"
      :itemsPerPage="itemsPerPage"
      :sortFunctions="sortFunctions"
      :apiMode="false"
      :apiUrl="''"
      :paginationPath="paginationPath"
      :data="topAlltimePoints.users"
      :dataTotal="topAlltimePoints._total">      
     </data-table>
  </div>
</template>

<script>
import VuesticWidget from "../vuestic-components/vuestic-widget/VuesticWidget";
import VuesticAlert from "../vuestic-components/vuestic-alert/VuesticAlert";
import DataTable from "../vuestic-components/vuestic-datatable/VuesticDataTable.vue";

export default {
  name: "loyalty",
  components: {
    VuesticWidget,
    VuesticAlert,
    DataTable
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
      },
      topPoints: {},
      topAlltimePoints: {},
      tableFields: [
        {
          name: "username", // Object property name in your data e.g. (data[0].name)
          sortField: "username" // Object property name in your data which will be used for sorting
        },
        {
          name: "points",
          sortField: "points",
          title: "booties"
        }
      ],
      itemsPerPage: [
        {
          value: 5
        },
        {
          value: 10
        },
        {
          value: 50
        }
      ],
      sortFunctions: {
        // use custom sorting functions for prefered fields
        username: function(item1, item2) {
          return item1 >= item2 ? 1 : -1;
        },
        points: function(item1, item2) {
          return item1 >= item2 ? 1 : -1;
        }
      },
      paginationPath: "pagination"
    };
  },
  beforeMount() {
    this.$api.StreamElements.GetTopPoints().then(response => {
      this.topPoints = response.data.value;
    });
    this.$api.StreamElements.GetTopAlltimePoints().then(response => {
      this.topAlltimePoints = response.data.value;
    });
  }
};
</script>

<style lang="scss" scoped>
@import "../../sass/_variables.scss";
</style>
