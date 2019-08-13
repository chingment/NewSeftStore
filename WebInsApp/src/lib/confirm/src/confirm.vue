<template>
  <div class="lumos-confirm" v-show="isShow">
    <div class="wraper">
      <div class="confirm-title">{{title}}</div>
      <p class="confirm-content">
        <span class="msg" v-html="msg"></span>
      </p>
      <div class="confirm-operation" v-if="type == 'alert'">
        <div class="confirm-btn" @click.stop="alertClick">
          <span class="my-btn-text">{{alertBtnText}}</span>
        </div>
      </div>
      <div class="confirm-operation" v-if="type == 'confirm'">
        <div @touchstart.prevent="noClick" class="cancel-btn br1">
          <span class="btn-text">{{noBtnText}}</span>
        </div>
        <div @touchstart.prevent="_yesClick" class="confirm-btn">
          <span class="btn-text">{{yesBtnText}}</span>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  name: "lumos-confirm",
  props: {
    title: {
      type: String,
      default: "提示"
    },
    msg: {
      type: String,
      default: ""
    },
    type: {
      type: String,
      default: "alert"
    },
    alertBtnText: {
      type: String,
      default: "我知道了"
    },
    yesBtnText: {
      type: String,
      default: "确定"
    },
    noBtnText: {
      type: String,
      default: "取消"
    }
  },
  data() {
    return {
      title: this.title,
      msg: this.msg,
      type: this.type
    };
  },
  props: {
    isShow: false,
    yesClick: {
      type: Function
    }
  },
  methods: {
    confirm() {
      let _this = this;
      this.isShow = true;
      return new Promise(function(resolve, reject) {
        _this.promiseStatus = { resolve, reject };
      });
    },
    noClick() {
      this.isShow = false;
      this.promiseStatus && this.promiseStatus.reject();
    },
    _yesClick() {
      this.isShow = false;
      this.yesClick();
    },
    alertClick() {
      this.isShow = false;
      this.promiseStatus && this.promiseStatus.resolve();
    }
  }
};
</script>

<style lang="less" scoped>
.lumos-confirm {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  z-index: 998;
  /* 这里防止当用户长按屏幕，出现的黑色背景色块，以及 iPhone 横平时字体的缩放问题 */
  -webkit-text-size-adjust: 100%;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  > .wraper {
    position: absolute;
    top: 28%;
    left: 0;
    right: 0;
    width: 280px;
    margin: 0 auto;
    background-color: #fff;
    border-radius: 5px;
    z-index: 999;
    user-select: none;

    .confirm-title {
      padding-top: 0.5rem;
      text-align: center;
      font-size: 1.1rem;
      font-weight: 500;
      color: #333;
    }

    .confirm-content {
      padding: 0 15px;
      padding-top: 20px;
      margin-bottom: 32px;
      text-align: center;
      font-size: 16px;
      color: #666;
      line-height: 1.5;
    }

    .confirm-operation {
      display: flex;
      border-top: 1px solid #eee;
      text-align: center;
      justify-content: center;
      align-items: center;
      line-height: 2.5rem;

      .cancel-btn,
      .confirm-btn {
        flex: 1;
      }

      .cancel-btn {
        color: #b7b7b7;
      }

      .confirm-btn {
        color: #ffb000;
      }

      .btn-text {
        text-align: center;
        font-size: 16px;
        margin: 14px 0;
        padding: 6px 0;
      }

      /* 其他修饰样式 */
      .br1 {
        border-right: 1px solid #eee;
      }
    }
  }
}
</style>

