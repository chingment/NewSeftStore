<template>
  <div class="sug-product" style="margin-top:20px">
    <div class="sug-tag">------ 健康商品 ------</div>
    <div class="its-skus prd-list-1">
      <div v-for="(sku,index) in skus" :key="index" :class="'it '+'it-'+index%2 ">
        <div class="imgurl">
          <img class="img" :src="sku.mainImgUrl" style="width: 100%;height:100%">
        </div>
        <div class="name">{{ sku.name }}</div>
        <wx-open-launch-weapp id="launch-btn" username="gh_1561f3c9d366" :path="'pages/productdetails/productdetails.html?skuId='+sku.id+'&shopMethod=1'">
          <script type="text/wxtag-template">
            <button class="open-launch-weapp-btn" style="background-color:transparent;border: none;color:#ffb101;font-size:14px;">点击查看</button>
          </script>
        </wx-open-launch-weapp>

      </div>
    </div>
  </div>

</template>

<script>

import { getSugProducts } from '@/api/monthreport'

export default {
  name: 'SugProduct',
  props: {
    rptId: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      skus: [],
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    console.log('rptId:' + this.rptId)
    this.onGetSugProducts()
  },
  methods: {
    onGetSugProducts() {
      getSugProducts({ rptId: this.rptId }).then(res => {
        console.log(res)
        if (res.result === 1) {
          var d = res.data
          this.skus = d.items
        }
      })
    }
  }
}
</script>

<style>

.sug-tag{
    text-align: center;
    color: #9d9d9d;
}

.prd-list-1 {
  position: relative;
  overflow: hidden;
}

.prd-list-1 .it {
  position: relative;
  float: left;
  width: 50%;
  box-sizing: border-box;
  text-align: center;
  padding: 14px 0;
  overflow: hidden;
}

.prd-list-1 .it-0 {
  padding-left: 20px;
  padding-right: 10px;
}

.prd-list-1 .it-1 {
  padding-left: 10px;
  padding-right: 20px;
}

.prd-list-1 .imgurl {
  text-align: center;
  height: 100px;
  width: 100px;
  margin: auto;
  position: relative;
  overflow: hidden;
}

.prd-list-1 .name {
  width: 100%;
  font-size: 16px;
  text-align: left;
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  word-break: break-all;
  -webkit-box-orient: vertical;
  -webkit-line-clamp: 1;
  color: #000;
  margin:10px 0;
}

.prd-list-1 .briefInfo {
  width: 100%;
  height: 42px;
  line-height: 42px;
  font-size: 24px;
  text-align: left;
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  word-break: break-all;
  -webkit-box-orient: vertical;
  -webkit-line-clamp: 1;
  margin:10px 0;
}

.prd-list-1 .area2 {
  display: flex;
}

.prd-list-1 .price {
  flex: 1;
  text-align: left;
  margin-right:10px;

}

.prd-list-1 .saleprice {
  height: 62px;
  line-height: 62px;
  font-size: 42px;
  display: inline-block;
  color: #ffb101;
}

.prd-list-1 .showprice {
  margin-left: 10px;
  height: 62px;
  line-height: 62px;
  font-size: 32px;
  display: inline-block;
  text-decoration: line-through;
}

.prd-list-1 .operate {
  flex: 1;
  text-align: right;
  align-items: center;
  justify-content: flex-end;
  height: 62px;
  display: flex;
}

</style>
