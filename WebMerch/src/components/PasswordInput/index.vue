<template>
  <div :id="`ids_${id}`" class="m-password-input">
    <div v-for="(v, i) in 6" :key="i" class="box-input">
      <div v-show="pwdList[i]" class="u-dot" @click="changePwd">•</div>
      <input
        :key="i"
        v-model="pwdList[i]"
        type="tel"
        readonly
        onfocus="this.removeAttribute('readonly');"
        maxlength="1"
        autocomplete="new-password"
        class="u-input"
        @input="changeInput"
        @click="changePwd"
        @keyup="keyUp($event)"
        @keydown="oldPwdList = pwdList.length"
      >
    </div>
  </div>
</template>

<script>
export default {
  props: {
    id: {
      type: Number,
      default: 1 // 当一个页面有多个密码输入框时，用id来区分
    }
  },
  data() {
    return {
      pwdList: [],
      pwdListReal: [],
      oldPwdList: [],
      isDelete: false,
      ipt: ''
    }
  },
  mounted() {
    this.ipt = document.querySelectorAll(`#ids_${this.id} .u-input`)
  },
  methods: {
    keyUp(ev) {
      let index = this.pwdList.length
      if (!index) return
      if (ev.keyCode === 8) {
        this.isDelete = true
        if (this.oldPwdList === this.pwdList.length) {
          if (index === this.pwdList.length) {
            this.pwdList.pop()
          }
          index--
        } else {
          index > 0 && index--
        }
        this.ipt[index].focus()
      } else if (this.isDelete && index === this.pwdList.length && /^\d$/.test(ev.key)) {
        this.isDelete = false
        this.pwdList.pop()
        this.pwdList.push(ev.key)
        this.ipt[this.pwdList.length] && this.ipt[this.pwdList.length].focus()
      }
      this.$emit('get-pwd', this.pwdList.join(''))
    },
    changePwd() {
      let index = this.pwdList.length
      index === 6 && index--
      this.ipt[index].focus()
    },
    changeInput() {
      let index = this.pwdList.length
      const val = this.pwdList[index - 1]
      if (!/[0-9]/.test(val)) {
        this.pwdList.pop()
        return
      }
      if (!val) {
        this.pwdList.pop()
        index--
        if (index > 0) this.ipt[index - 1].focus()
      } else {
        if (index < 6) this.ipt[index].focus()
      }
    }
  }
}
</script>

<style lang="scss" scoped>
.m-password-input {
  display: flex;
  border-radius: 5px;
  padding: 10px 0;
  border: 1px solid #cccccc;
  position: relative;
  margin-left: 1px;
  .box-input {
    display: inline-block;
    position: relative;
    border-right: 1px solid #cccccc;
    &:last-child{
      border-right:0;
    }
    .u-dot {
      position: absolute;
      top: 0;
      right: 0;
      bottom: 0;
      left: 0;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 40px;
    }
    .u-input {
      text-align: center;
      font-size: 30px;
      float: left;
      width: 40px;
      height: 20px !important;
      color: transparent;
      caret-color: #333333;
      outline: unset;
      border: none;
      border-right: 1px solid #cccccc;
      &:last-child{
        border-right:0;
      }
    }
  }
}
</style>
