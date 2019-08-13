<template>
  <div class="lumos-header">
    <div class="header-left">
      <div class="button">
        <span class="button-icon" v-show="aleftButton.iconPath!=''">
          <img :src="aleftButton.iconPath">
          <div class="button-vonbadge">
            <span :class="aleftButton.vonBadge.type">{{ aleftButton.vonBadge.text }}</span>
          </div>
        </span>
        <span class="button-text" v-show="aleftButton.iconPath!=''">{{ aleftButton.text }}</span>
      </div>
    </div>
    <div class="header-title">
      <div class="title">
        <span class="title-text">{{ atitle.text }}</span>
      </div>
    </div>
    <div class="header-right">
      <div class="button">
        <span class="button-icon" v-show="arightButton.iconPath!=''">
          <img :src="arightButton.iconPath">

          <div class="button-vonbadge">
            <span :class="arightButton.vonBadge.type">{{ arightButton.vonBadge.text }}</span>
          </div>
        </span>
        <span class="button-text" v-show="arightButton.iconPath!=''">{{ arightButton.text }}</span>
      </div>
    </div>
  </div>
</template>
 
<style lang="less" scoped>
.lumos-header {
  background: #006dee;
  height: 2.6rem;
  color: #fff;
  display: flex;
  align-items: center;
}

.lumos-header .header-left,
.lumos-header .header-right {
  -webkit-box-flex: 0.5;
  -ms-flex: 0.5;
  flex: 0.5;
  display: flex;
  align-items: center;
  position: relative;
}

.lumos-header > .header-title {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  font-size: inherit;
  font-weight: 600;
  -webkit-box-flex: 1;
  -ms-flex: 1;
  flex: 1;
  font-size: 1.1rem;
}

.lumos-header .header-left {
  justify-content: flex-start;
  padding-left: 10px;
}
.lumos-header .header-right {
  justify-content: flex-end;
  padding-right: 10px;
}

.lumos-header .header-right .button {
  display: flex;
}

.lumos-header .header-right .button .button-icon {
  position: relative;
  width: 1.2rem;
  height: 1.2rem;
}

.lumos-header .header-right .button .button-icon img {
  height: 100%;
  width: 100%;
}

.lumos-header .header-right .button .button-icon .button-vonbadge {
  position: absolute;
  display: inline-block;
  padding: 2px 3px;
  z-index: 12;
  border-radius: 15px;
  background-color: #ea5a49;
  text-align: center;
  position: absolute;
  top: -6px;
  left: 50%;
  font-size: 0.5rem;
}

.lumos-header .header-right .button .button-text {
  margin-left: 0.3rem;
}
</style>

<script>
export default {
  name: "lumos-header",
  data() {
    return {
      atitle: this.title,
      aleftButton: this.leftButton,
      arightButton: this.rightButton
    };
  },
  props: {
    title: {
      type: Object,
      default: {
        text: ""
      }
    },
    leftButton: {
      type: Object,
      default: function() {
        return {
          iconPath: "",
          text: "",
          vonBadge: {
            type: "circle",
            text: ""
          }
        };
      }
    },
    rightButton: {
      type: Object,
      default: function() {
        return {
          iconPath: "",
          text: "",
          vonBadge: {
            type: "circle",
            text: ""
          }
        };
      }
    }
  },
  watch: {
    "arightButton.vonBadge.text": {
      handler(newVal, oldVal) {
        console.log(
          "header.arightButton.vonBadge.text is change:" +
            oldVal +
            "->" +
            newVal
        );
        this.$emit("rightButtonVonBadgeChange");
      },
      deep: true,
      immediate: true
    }
  }
};
</script>
