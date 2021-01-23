<template>
     <el-page-header  @back="goBack" class="my-page-header" :content="content">
    </el-page-header>
</template>

<script>

export default {
  data() {
    return {
        content:''
    }
  },
  created() {
       this.content=this.$route.meta.title
  },
  methods: {
     goBack() {
        console.log('go back');
        this.$router.go(-1);
     }
  }
}
</script>