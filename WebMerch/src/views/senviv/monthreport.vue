<template>
  <div>
    <pane-stage-report cache-query rpt-type="per_month" />
  </div>
</template>

<script>

import PaneStageReport from './components/PaneStageReport.vue'
export default {
  name: 'SenvivMonthReport',
  components: { PaneStageReport },
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

