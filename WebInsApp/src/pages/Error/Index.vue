<template>
  <div id="app_wrapper" >
    <div class="messagebox">
    <div class="icon"> <img src="@/assets/images/error_an.jpg"  alt=""> </div>
    <P class="title">{{ messageBox.title }}</P>
    <P class="content">{{ messageBox.content }}</P>
    </div>
  </div>
</template>
<script>
export default {
  data() {
    return {
      messageBox: {
        title: "温馨提示",
        content: "您好，您访问的页面不存在"
      }
    };
  },
  mounted: function() {
    var _this = this;
    if (typeof _this.$store.getters.getMessagesBox != "undefined") {
      _this.messageBox = _this.$store.getters.getMessagesBox;
    }
  }
};
</script>
<style  lang="less" >
body {
  background-color: #fff;
}

.messagebox {
  margin-top: 5rem;
  .icon {
    width: 50%;
    margin: auto;
  }

  .title {
    line-height: 2.4rem;
    height: 2.4rem;
    font-size: 1.1rem;
  }

  .content {
    line-height: 2.4rem;
    height: 2.4rem;
    font-size: 0.9rem;
  }
}
</style>
