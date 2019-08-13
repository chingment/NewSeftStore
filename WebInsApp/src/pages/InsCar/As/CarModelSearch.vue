<template>
  <div>
   <div class="search-header">
        <div class="search-iptbox">
            <input type="text" placeholder="输入车辆型号或车架号" >
        </div>
          <a href="javascript:void(0)">搜索</a>
  </div>
  <div class="space" style="height: 1.8rem;line-height: 1.8rem;font-size: .8rem;" >没有您的车型？试试品牌型号搜索</div>
    <div class="list-searchmodels" v-if="models.length>0">
      <template v-for="(model,index) in this.models">
        <div class="item" :key="index">
            <span>{{ model.modelName }} 排量 {{ model.exhaust }} {{ model.marketYear }}款 {{ model.seat }}座 （参考价 {{ model.purchasePrice }}）</span>
        </div>
      </template>
    </div>
  <div class="empty-searchmodels" v-else>
      暂无记录
    </div>
  </div>
</template>
<script>
export default {
  data() {
    return {
      models: []
    };
  },
  methods: {
    onChange() {
      this.text.name = "我是由父级组件触发改变了内容";
    },
    search(type, key) {
      var _this = this;
      _this.$http
        .get("/InsCar/SearchModelInfo", { type: type, key: key })
        .then(res => {
          console.log(res);
          var d = res.data;
          _this.models = d.models;
        });
    }
  },
  mounted: function() {
    this.search("1", "TV");
  }
};
</script>
<style lang="less" scoped>
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
</style>
