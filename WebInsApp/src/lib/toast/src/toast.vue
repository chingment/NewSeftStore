<template>
  <div class="lumos-toast" v-show="isShow">
    <span>{{ text }}</span>
  </div>
</template>


<style>
.lumos-toast {
  position: fixed;
  left: 50%;
  top: 50%;
  background: rgba(0, 0, 0, 0.35);
  padding: 10px;
  border-radius: 5px;
  transform: translate(-50%, -50%);
  color: #fff;
}
</style>


<script>
export default {
  name: "lumos-toast",
  data() {
    return {
    };
  },
  props: {
    isShow: {
      type: Boolean,
      default: false
    },
    text: {
      type: String,
      default: ""
    }
  }
};
</script>