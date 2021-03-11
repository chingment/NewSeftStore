<template>
  <el-page-header class="my-page-header" :content="content" @back="goBack" />
</template>

<script>

export default {
  data() {
    return {
      content: ''
    }
  },
  created() {
    this.content = this.$route.meta.title
  },
  methods: {
    goBack() {
      this.$router.go(-1)
    }
  }
}
</script>
