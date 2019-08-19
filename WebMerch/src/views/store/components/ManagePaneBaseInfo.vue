<template>
  <div>
    AAA
  </div>
</template>
<script>

export default {
  name: 'ManageBaseInfo',
  props: {
    storeId: {
      type: String,
      default: 'CN'
    }
  },
  data() {
    return {
      loading: false,
      sellChannels: [],
      listQuery: {
        page: 1,
        limit: 10,
        name: undefined
      }
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {

    }
  }
}
</script>
