<template>
  <div class="loading_box">
    <van-loading size="36px" class="loading" color="#FFF" vertical>加载中...</van-loading>
  </div>
</template>

<script>
export default {
  props: {},
  data() {
    return {}
  },
  computed: {},
  created() {},
  mounted() {}
}
</script>

<style lang="scss"  scoped>
.loading_box {
  position: fixed;
  outline: none;
  border: none;
  color: #fff;
  text-transform: uppercase;
  letter-spacing: 1px;
  font-size: 12px;
  z-index: 50;
  line-height: 32px;
  overflow: hidden;
  border-radius: 5px;
  left: 50%;
  top: 50%;
  -webkit-transform: translate(-50%, -50%);
  width: 100px;
  text-align: center;

  .loading{
width: 100px;
    margin: auto;
    background:rgba(57,53,53,0.5);
    border-radius: 10px;
    padding: 10px;

  }
}
</style>
