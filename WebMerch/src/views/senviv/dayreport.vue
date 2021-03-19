<template>
  <div id="senviv_user_list">
    <pane-day-report cache-query />
  </div>
</template>

<script>

import PaneDayReport from './components/PaneDayReport.vue'
export default {
  name: 'SenvivDayReport',
  components: { PaneDayReport },
  data() {
    return {
      loading: false,
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        page: 1,
        limit: 20,
        name: undefined,
        sas: '0',
        chronic: '0',
        perplexs: null
      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {

  },
  methods: {

  }
}
</script>

