<template>
  <div class="lumos-loading" v-show="isShow">
    <div class="loading-img">
      <img src="@/assets/images/loading.gif" style="width:32px;display: inline-block">
    </div>
    <div class="loading-tips">
      <span></span>
    </div>
  </div>
</template>


<style>
.lumos-loading {
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
}
</style>


<script>
export default {
  name: "lumos-loading",
  data() {
    return {};
  },
  props: {
    isShow: false
  }
};
</script>