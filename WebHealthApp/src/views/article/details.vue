<template>
  <div id="article_details">

    <div class="title">
      {{ article.title }}
    </div>

    <div class="content" v-html="article.content" />

  </div>
</template>

<script>
import { details } from '@/api/article'
export default {
  data() {
    return {
      loading: false,
      articleId: '',
      svUserId: '',
      article: {
        title: '',
        content: ''
      },
      appInfo: {}
    }
  },
  created() {
    this.articleId = typeof this.$route.query.id === 'undefined' ? null : this.$route.query.id
    this.svUserId = typeof this.$route.query.svuid === 'undefined' ? null : this.$route.query.svuid
    this.onInit()
  },
  methods: {
    onInit() {
      this.loading = true
      details({ id: this.articleId, svuid: this.svUserId }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.article = d.article
        }
        this.loading = false
      })
    }
  }
}
</script>

<style lang="scss" scope>

#article_details{
        padding: 20px 16px 9px;
.title{
    font-size: 22px;
    line-height: 1.4;
    margin-bottom: 14px;
}

.content{
     overflow: hidden;
    color: #333;
    font-size: 17px;
    word-wrap: break-word;
    -webkit-hyphens: auto;
    -ms-hyphens: auto;
    hyphens: auto;
    text-align: justify;
    position: relative;
    z-index: 0;
    line-height: 26px;
}
}

</style>
