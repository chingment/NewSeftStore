<template>
  <transition name="slide">
    <div class="inscarmodelsearch animated" v-if="isShow">
      <div class="search-header">
        <div class="search-iptbox">
          <input type="text" placeholder="输入车辆型号或车架号" v-model="key" />
        </div>
        <a href="javascript:void(0)" @click="_search">搜索</a>
        <a href="javascript:void(0)" @click="_close">取消</a>
      </div>
      <div
        class="space"
        style="height: 1.8rem;line-height: 1.8rem;font-size: .8rem;"
      >没有您的车型？试试品牌型号搜索</div>
      <div class="list-searchmodels" v-if="models.length>0">
        <template v-for="(model,index) in this.models">
          <div class="item" :key="index" @click="_itemChoose(model)">
            <span>{{ model.modelName }} 排量 {{ model.exhaust }} {{ model.marketYear }}款 {{ model.seat }}座 （参考价 {{ model.purchasePrice }}）</span>
          </div>
        </template>
      </div>
      <div class="empty-searchmodels" v-else>暂无记录</div>
    </div>
  </transition>
</template>
<script>
export default {
  data() {
    return {
      type: "",
      key: "",
      models: [],
      defaultTrigger: true
    };
  },
  props: {
    isShow: {
      type: Boolean,
      default: false,
      required: false
    },
    close: {
      type: Function
    },
    onChoose: {
      type: Function
    }
  },
  methods: {
    onChange() {
      this.text.name = "我是由父级组件触发改变了内容";
    },
    _search() {
      var _this = this;
      var type = _this.type;
      var key = _this.key;
      _this.$http
        .get("/InsCar/SearchModelInfo", { type: type, key: key })
        .then(res => {
          console.log(res);
          var d = res.data;
          _this.models = d.models;
        });
    },
    _close() {
      this.close();
      //this.isShow = false;
    },
    _itemChoose(item) {
      console.log(item);
       this.onChoose && this.onChoose(item);
    }
  },
  mounted: function() {
    let _this = this;
    _this.type = "1";
    _this.key = "TV";
    _this._search();
  }
};
</script>
<style lang="less" scoped>
* {
  font-weight: 300 !important;
}
::-webkit-scrollbar {
  display: none;
}
.inscarmodelsearch {
  -webkit-transition: all 0.2s ease-out;
  transition: all 0.2s ease-out;

  position: fixed;
  top: 0;
  bottom: 0;
  left: 0;
  right: 0;
  background: #fff;
  z-index: 10000;
  color: #333;
  overflow-y: scroll;
  -webkit-overflow-scrolling: touch;
  // padding: 0.1rem 0.1rem 0 0.1rem;
  // margin: 0.1rem 0.1rem 0 0.1rem;
  box-shadow: 0 0.01rem 0.06rem rgba(0, 0, 0, 0.2);
}

.list-searchmodels {
  background-color: #fff;
  padding: 0 1rem;
  line-height: 1.5rem;
  justify-content: flex-start;
  text-align: left;
  > .item {
    border-top: 1px solid #f8f8f8;
    padding: 0.3rem 0rem;
    display: flex;
  }
}

.search-header {
  height: 2.8rem;
  display: -webkit-box;
  background: #fff;
  padding-left: 0.8rem;
  .search-iptbox {
    -webkit-box-flex: 1;
    height: 100%;
    input {
      width: 100%;
      height: 100%;
      line-height: 2.8rem;
      display: block;
      border: none;
      background: none;
      box-sizing: border-box;
      height: 2.8rem;
      font-size: 1rem;
    }
    input::-webkit-input-placeholder {
      color: #cdcdcd;
      text-align: left;
    }
  }
  a {
    width: 3rem;
    height: 100%;
    line-height: 2.8rem;
    text-align: right;
    color: #999999;
    display: block;
    text-decoration: none;
    padding-right: 0.8rem;
  }
}

.slide-enter,
.slide-leave-active {
  transform: translateX(100%) !important;
}
.fade-enter,
.fade-leave-active {
  opacity: 0;
}
</style> 
