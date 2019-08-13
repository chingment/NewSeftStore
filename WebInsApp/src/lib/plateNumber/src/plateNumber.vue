<template>
  <div id="plateNumber" class="item pdt0"   v-clickoutside="handleClose">
    <div class="wrap">
      <div class="radio-box">
        <label class="flex-items-center">
          <img v-if="formData.commonCard == 1"
            src="@/assets/images/icon_chose_s@2x.png"
            alt="">
          <img v-else
            src="@/assets/images/icon_chose_n@2x.png"
            alt="">
          <input type="radio"
            v-model="formData.commonCard"
            value="1" />普通车牌
        </label>
        <label class="flex-items-center">
          <img v-if="formData.commonCard == 2"
            src="@/assets/images/icon_chose_s@2x.png"
            alt="">
          <img v-else
            src="@/assets/images/icon_chose_n@2x.png"
            alt="">
          <input type="radio"
            v-model="formData.commonCard"
            value="2" />新能源车牌
        </label>
      </div>

      <div class="plate-box" >
    
     <div class="title-box" >
       <span>车牌号</span>
     </div>

      <div class="num-box">
        <div class="num0" @click="clickFirstWrap()">
            <span>{{formData.num0}}</span>
        </div>
        <div class="num1 mgr2" @click="clickKeyWordWrap(1)"><span>{{formData.num1}}</span></div>
        <em class="spot"></em>
        <div class="num1" @click="clickKeyWordWrap(2)"><span>{{formData.num2}}</span></div>
        <div class="num1" @click="clickKeyWordWrap(3)"><span>{{formData.num3}}</span></div>
        <div class="num1" @click="clickKeyWordWrap(4)"><span>{{formData.num4}}</span></div>
        <div class="num1" @click="clickKeyWordWrap(5)"><span>{{formData.num5}}</span></div>
        <div class="num1" @click="clickKeyWordWrap(6)"><span>{{formData.num6}}</span></div>
        <div v-if="formData.commonCard == '2'" class="num1" @click="clickKeyWordWrap(7)"><span>{{formData.num7}}</span></div>
      </div>

      </div>

      <!-- <div class="submit-box">
        <button @click="submitFn()">确认</button>
      </div> -->
    </div>

    <div class="first-word-wrap"  v-show="show" ref="firstwordwrap" 
      v-if="firstWrapStatus">
      <div class="first-word"
        @click="selectFirstWord($event)">
        <div class="word">
          <span>京</span>
        </div>
        <div class="word">
          <span>湘</span>
        </div>
        <div class="word">
          <span>津</span>
        </div>
        <div class="word">
          <span>鄂</span>
        </div>
        <div class="word">
          <span>沪</span>
        </div>
        <div class="word">
          <span>粤</span>
        </div>
        <div class="word">
          <span>渝</span>
        </div>
        <div class="word">
          <span>琼</span>
        </div>
      </div>
      <div class="first-word"
        @click="selectFirstWord($event)">
        <div class="word">
          <span>翼</span>
        </div>
        <div class="word">
          <span>川</span>
        </div>
        <div class="word">
          <span>晋</span>
        </div>
        <div class="word">
          <span>贵</span>
        </div>
        <div class="word">
          <span>辽</span>
        </div>
        <div class="word">
          <span>云</span>
        </div>
        <div class="word">
          <span>吉</span>
        </div>
        <div class="word">
          <span>陕</span>
        </div>
      </div>
      <div class="first-word" 
        @click="selectFirstWord($event)">
        <div class="word">
          <span>黑</span>
        </div>
        <div class="word">
          <span>甘</span>
        </div>
        <div class="word">
          <span>苏</span>
        </div>
        <div class="word">
          <span>青</span>
        </div>
        <div class="word">
          <span>浙</span>
        </div>
        <div class="word">
          <span>皖</span>
        </div>
        <div class="word">
          <span>藏</span>
        </div>
        <div class="word">
          <span>闽</span>
        </div>
      </div>
      <div class="first-word"
        @click="selectFirstWord($event)">
        <div class="word">
          <span>蒙</span>
        </div>
        <div class="word">
          <span>赣</span>
        </div>
        <div class="word">
          <span>桂</span>
        </div>
        <div class="word">
          <span>鲁</span>
        </div>
        <div class="word">
          <span>宁</span>
        </div>
        <div class="word">
          <span>豫</span>
        </div>
        <div class="word">
          <span>新</span>
        </div>
        <div class="word bordernone">
          <!-- <img src="../assets/images/icon-switch.png" alt=""> -->
        </div>
      </div>
    </div>
    <div class="keyboard-wrap"   ref="keyboardwrap"  v-if="keyBoardStatus === true">
      <!-- <div class="number-wrap"></div>
      <div class="letter-wrap"></div>
      <div class="cn-wrap"></div> -->
      <div class="keyboard" v-if="activeKeyWordIndex !== 1">
        <span v-for="(item,index) in allKeyWord._1"
          :key="index"
          @click="clickKeyBoard(item)">{{item}}</span>
      </div>
      <div class="keyboard" v-if="activeKeyWordIndex !== 1">
        <span v-for="(item,index) in allKeyWord._2"
          :key="index"
          @click="clickKeyBoard(item)">{{item}}</span>
          <span class="bordernone"></span>
          <span class="bordernone"></span>
          <span class="bordernone"></span>
          <span class="bordernone"></span>
      </div>
      <div class="keyboard">
        <span v-for="(item,index) in allKeyWord._3"
          :key="index"
          @click="clickKeyBoard(item)">{{item}}</span>
      </div>
      <div class="keyboard">
        <span v-for="(item,index) in allKeyWord._4"
          :key="index"
          @click="clickKeyBoard(item)">{{item}}</span>
      </div>
      <div class="keyboard">
        <span v-for="(item,index) in allKeyWord._5"
          :key="index"
          @click="clickKeyBoard(item)">{{item}}</span>
      </div>
      <div class="keyboard">
        <span v-for="(item,index) in allKeyWord._6"
          :key="index"
          @click="clickKeyBoard(item)">{{item}}</span>
          <span class="bordernone"></span>
          <span class="bordernone"></span>
          <span class="bordernone"></span>
          <!-- <span class="bordernone" v-if="activeKeyWordIndex === 1"></span>
          <span class="bordernone" v-if="activeKeyWordIndex === 1"></span> -->
          <!-- <span @click="deleteWord" v-if="activeKeyWordIndex === 1">x</span> -->
      </div>
      <div class="keyboard" v-if="activeKeyWordIndex !== 1">
        <span v-for="(item,index) in allKeyWord._7"
          :key="index"
          @click="clickKeyBoard(item)">{{item}}</span>
          <span class="bordernone"></span>
          <span class="delete" @click="deleteWord"><img src="@/assets/images/icon-delete.png" alt=""></span>
      </div>
      <div class="cancel">
        <span @click="keyBoardStatus = false">完成</span>
      </div>
    </div>
  </div>
</template>
<script>
import { fail } from "assert";

const clickoutside = {
  // 初始化指令
  bind(el, binding, vnode) {
    function documentHandler(e) {
      // 这里判断点击的元素是否是本身，是本身，则返回
      if (el.contains(e.target)) {
        return false;
      }
      // 判断指令中是否绑定了函数
      if (binding.expression) {
        // 如果绑定了函数 则调用那个函数，此处binding.value就是handleClose方法
        binding.value(e);
      }
    }
    // 给当前元素绑定个私有变量，方便在unbind中可以解除事件监听
    el.__vueClickOutside__ = documentHandler;
    document.addEventListener("click", documentHandler);
  },
  update() {},
  unbind(el, binding) {
    // 解除事件监听
    document.removeEventListener("click", el.__vueClickOutside__);
    delete el.__vueClickOutside__;
  }
};

export default {
  name: "plateNumber",
  data() {
    return {
      show: true,
      formData: {
        commonCard: "1",
        num0: "",
        num1: "",
        num2: "",
        num3: "",
        num4: "",
        num5: "",
        num6: "",
        num7: ""
      },
      allKeyWord: {
        _1: [1, 2, 3, 4, 5, 6, 7],
        _2: [8, 9, 0],
        _3: ["A", "B", "C", "D", "E", "F", "G"],
        _4: ["H", "J", "K", "L", "M", "N", "O"],
        _5: ["P", "Q", "R", "S", "T", "U", "V"],
        _6: ["W", "X", "Y", "Z"],
        _7: ["港", "澳", "学", "领", "警"]
      },
      activeKeyWordIndex: 1, // 当前车牌号
      keyBoardStatus: false,
      firstWrapStatus: false, // 选择弹窗
      confirmTitle: "",
      submitConfirm: false,
      submitConfirmFalse: false,
      submitConfirmText: ""
    };
  },
  mounted() {},
  directives: { clickoutside },
  methods: {
    handleClose(e) {
      //this.show = false;
      // this.firstWrapStatus=false;
      // if(this.keyBoardStatus)
      // {
      //   this.keyBoardStatus=false;
      // }
    },
    clickFirstWrap() {
      // 点击第一个输入框
      this.firstClickStatus = true;
      this.firstWrapStatus = true;
      this.keyBoardStatus = false;
      this.formData.num0 = "";
      this.show = true;
      this.$nextTick(() => {
        document.body.scrollTop = 1000;
      });
    },
    selectFirstWord(event) {
      // 选择省份
      if (event.target.localName !== "span") {
        return;
      }
      this.formData.num0 = event.target.innerText;
      this.firstSelectStatus = true;
      this.firstWrapStatus = false;
      this.firstClickStatus = false;
      this.keyBoardStatus = true;
      this.activeKeyWordIndex = 1;
      // this.$refs.num1.focus()
      // document.getElementById('num1').focus()
    },
    clickKeyBoard(item) {
      // 点击自定义键盘
      console.log(item);
      this.formData["num" + this.activeKeyWordIndex] = item;

      if (this.formData.commonCard === "1") {
        this.activeKeyWordIndex++;
        if (this.activeKeyWordIndex > 6) {
          this.keyBoardStatus = false;
        }
      } else {
        this.activeKeyWordIndex++;
        if (this.activeKeyWordIndex > 7) {
          this.keyBoardStatus = false;
        }
      }

      //     this.$nextTick(() => {
      //   document.body.scrollTop =0
      // });
    },
    deleteWord() {
      // 退格
      // console.log(this.activeKeyWordIndex)
      // console.log(this.formData['num' + (this.activeKeyWordIndex - 1)])
      if (this.activeKeyWordIndex > 1) {
        this.formData["num" + (this.activeKeyWordIndex - 1)] = "";
        this.activeKeyWordIndex--;
      }
    },
    clickKeyWordWrap(activeKeyWordIndex) {
      this.keyBoardStatus = true;
      this.activeKeyWordIndex = activeKeyWordIndex;
      this.formData["num" + this.activeKeyWordIndex] = "";
    },
    submitFn() {
      let plateLicense;
      if (this.formData.commonCard === "1") {
        plateLicense = this.plate_license_1;
        plateLicense = this.palindrome(plateLicense);
        if (plateLicense.length < 7) {
          alert("请输入正确的车牌号");
          return;
        }
      }
      if (this.formData.commonCard === "2") {
        plateLicense = this.plate_license_2;
        plateLicense = this.palindrome(plateLicense);
        if (plateLicense.length < 8) {
          alert("请输入正确的车牌号");
          return;
        }
      }
      this.$emit("getPlateLicense", plateLicense);
      console.log(plateLicense);
      alert(plateLicense);
    },
    palindrome(str) {
      var arr = str.split("");
      arr = arr.filter(function(val) {
        return (
          val !== " " &&
          val !== "," &&
          val !== "." &&
          val !== "?" &&
          val !== ":" &&
          val !== ";" &&
          val !== "`" &&
          val !== "'" &&
          val !== "_" &&
          val !== "/" &&
          val !== "-" &&
          val !== "\\" &&
          val !== "" &&
          val !== "(" &&
          val !== ")"
        );
      });
      return arr.join("");
    },
    checkIsHasSpecialStr(str) {
      var flag = false;
      var arr = str.split("");
      arr.forEach(val => {
        if (
          val === "!" ||
          val === "}" ||
          val === "{" ||
          val === "]" ||
          val === "[" ||
          val === "&" ||
          val === "$" ||
          val === "@" ||
          val === " " ||
          val === "," ||
          val === "." ||
          val === "?" ||
          val === ":" ||
          val === ";" ||
          val === "`" ||
          val === "'" ||
          val === "_" ||
          val === "/" ||
          val === "-" ||
          val === "\\" ||
          val === "" ||
          val === "(" ||
          val === ")"
        ) {
          flag = true;
        }
      });
      return flag;
    },
    checkIsHasChineseStr(str) {
      var Reg = /.*[\u4e00-\u9fa5]+.*/;
      if (Reg.test(str)) {
        return true;
      }
      return false;
    }
  },
  computed: {
    plate_license_1() {
      return (
        this.formData.num0 +
        this.formData.num1 +
        this.formData.num2 +
        this.formData.num3 +
        this.formData.num4 +
        this.formData.num5 +
        this.formData.num6
      );
    },
    plate_license_2() {
      return (
        this.formData.num0 +
        this.formData.num1 +
        this.formData.num2 +
        this.formData.num3 +
        this.formData.num4 +
        this.formData.num5 +
        this.formData.num6 +
        this.formData.num7
      );
    }
  },

  watch: {
    firstWrapStatus: function(val, oldval) {
      //var cur_top=document.body.scrollTop;
      //console.log("cur_top:"+cur_top);

      // console.log("keyBoardStatus:"+val)

      var top = 0;
      if (val) {
        top = 300;
      }

      this.$nextTick(() => {
        document.body.scrollTop = top;
      });
    },
    keyBoardStatus: function(val, oldval) {
      var top = 0;
      if (val) {
        top = 300;
      }

      this.$nextTick(() => {
        document.body.scrollTop = top;
      });
    }
  }
};
</script>
<style lang="less" scoped>
.flex-items-center {
  display: flex;
  align-items: center;
}
.mgr2 {
  margin-right: 0.2rem;
}
.wrap {
  width: 100%;
  // height: 11.1rem;
  // margin: 0.5rem 0.6rem;
  // padding: 0.85rem 0.6rem;
  // background-color: #fff;
  // border-radius: 0.5rem;
  .radio-box {
    display: flex;
    align-items: center;
    justify-content: flex-end;
    font-size: 0.9rem;
    text-align: right;
    color: #4a4a4a;
    line-height: 2.1rem;
    input[type="radio"] {
      display: none;
    }
    label {
      padding-left: 0.6rem;
      cursor: pointer;
      img {
        width: 0.8rem;
        margin-right: 0.1rem;
      }
    }
  }
  .card-header {
    font-size: 0.75rem;
    margin: 0.2rem 0 0.5rem;
    color: #4a4a4a;
  }

  .plate-box {
    display: flex;
    align-content: center;
    justify-content: center;
  }

  .title-box {
    flex: 1;
    display: flex;
    justify-content: space-between;
    align-items: center;
    font-size: 1.1rem;
  }

  // input输入框
  .num-box {
    flex: 1;
    display: flex;
    justify-content: space-between;
    align-items: center;
    min-width: 13rem;
    max-width: 16rem;
    .spot {
      width: 0.2rem;
      height: 0.2rem;
      border-radius: 50%;
      background-color: #d8d8d8;
    }
    & > div {
      width: 1.6rem;
      height: 1.6rem;
      border: 1px solid #e4e4e4;
      margin-left: 0.2rem;
      &.first {
        position: relative;
        text-align: center;
        line-height: 1.7rem;
        font-weight: 200;
        .input-wrap {
          position: absolute;
          top: 0;
          left: 0;
          right: 0;
          bottom: 0;
          &.active {
            z-index: 100;
          }
        }
        em {
          color: #979797;
          font-size: 1.6rem;
          line-height: 1.7rem;
        }
        span {
          display: inline-block;
          width: 100%;
          height: 100%;
          // background-color: #9cbce2;
          color: #828282;
          line-height: 1.8rem;
        }
      }
      &.active {
        border: 1px solid #4a90e2;
        &:after {
          border-bottom: 0.5rem solid #4a90e2;
        }
      }
      span {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 100%;
        height: 100%;
        font-size: 1rem;
        color: #828282;
        &.first {
          background-color: #9cbce2;
          color: #fff;
          text-indent: 0.4rem;
          border-radius: 0;
        }
      }
    }
  }
  .submit-box {
    button {
      width: 100%;
      height: 2.2rem;
      border-radius: 0.25rem;
      font-size: 0.75rem;
      margin-top: 0.7rem;
      background: linear-gradient(
        320deg,
        rgba(74, 144, 226, 1) 0%,
        rgba(101, 172, 248, 1) 100%
      );
      color: #fff;
    }
  }
  .info {
    font-size: 0.5rem;
    margin-top: 0.9rem;
    color: #828282;
    text-align: left;
    img {
      width: 0.6rem;
      vertical-align: middle;
    }
  }
}
.first-word-wrap {
  // height: 9.4rem;
  z-index: 999;
  background-color: #d2d5db;
  padding: 0.6rem 0.8rem 1.1rem;
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  .first-word {
    display: flex;
    justify-content: space-between;
    margin-bottom: 0.45rem;
    .word {
      box-sizing: border-box;
      // width: 1.8rem;
      height: 1.8rem;
      // border: 1px solid #9cbce2;
      box-shadow: 0px 1px 4px rgba(0, 0, 0, 0.35);
      border-radius: 0.16rem;
      text-align: center;
      flex: 1;
      margin: 0.2rem;
      &.bordernone {
        border: none;
        box-shadow: none;
      }
      span {
        box-sizing: border-box;
        display: flex;
        align-items: center;
        justify-content: center;
        text-align: center;
        width: 100%;
        height: 100%;
        background-color: #fff;
        color: #000;
        // border: 1px solid #fff;
        border-radius: 0.125rem;
      }
      img {
        width: 1.6rem;
      }
    }
    &:nth-last-of-type(1) {
      margin-bottom: 0rem;
    }
  }
}
.keyboard-wrap {
  z-index: 9999;
  background-color: #d2d5db;
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  padding: 0.6rem 0.6rem 0.4rem;
  .keyboard {
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: 2rem;
    margin-bottom: 0.3rem;
    span {
      text-align: center;
      display: flex;
      // width: 1.8rem;
      align-items: center;
      justify-content: center;
      height: 1.8rem;
      margin: 0 0.3rem;
      box-shadow: 0px 1px 4px rgba(0, 0, 0, 0.35);
      background-color: #fff;
      border-radius: 0.125rem;
      flex: 1;
      margin: 0.2rem;
      &:active {
        background-color: #e4e4e4;
      }
      &.bordernone {
        border: none;
        box-shadow: none;
        background-color: #d2d5db;
        &:active {
          background-color: #d2d5db;
        }
      }
      &.delete {
        background-color: #465266;
        img {
          width: 1.15rem;
          height: 0.8rem;
        }
      }
    }
  }
  .cancel {
    display: flex;
    justify-content: flex-end;
    align-items: center;
    span {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 3.6rem;
      height: 1.8rem;
      background-color: #465266;
      color: #fff;
      border-radius: 0.125rem;
    }
  }
}
</style>
