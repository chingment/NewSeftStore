<template>
  <div id="app_wrapper">
    <lumos-swiper></lumos-swiper>
  
   <template v-for="(lNavGrid,pIndex) in this.lNavGrids">

    <div class="lnavgrid pd"  :key="pIndex" >
      <div class="title">
        <div class="title-left">
          <span class="icon">
            
          </span>
          <span class="title">{{ lNavGrid.title }}</span>
        </div>
        <div class="title-right"></div>
      </div>

      <div class="field" >


   <template v-for="(item,cIndex) in lNavGrid.items">

     <div class="item"  :key="cIndex"  @click="lNavGridItemClick(item)" >
      <div class="item-lefticon hid"></div>
      <div class="item-content" >  
      <div class="title" > {{ item.title}} </div>
      <div class="note lumos-hid" ></div>
      </div>  
      <div class="item-righticon" >
           <img  src="@/assets/images/icon_right.png" alt="">
      </div>      
     </div>

   </template>

      </div>
     
    </div>


   </template>

  </div>
</template>
<script>
export default {
  data() {
    return {
      lNavGrids: []
    };
  },
  methods: {
    getData() {
      var mId = this.$store.getters.getUserInfo.mId;
      var uId = this.$store.getters.getUserInfo.uId;
      this.$http
        .get("/Home/GetIndexPageData", { mId: mId, uId: uId })
        .then(res => {
          var d = res.data;

          this.lNavGrids = d.lNavGrids;
        });
    },
    lNavGridItemClick(item) {
      switch (item.opType) {
        case "HURL":
          window.location.href = item.opContent;
          break;
        case "PURL":
          this.$router.push({ path: item.opContent });
          break;
      }
    }
  },
  mounted: function() {
    let _this = this;
    _this.getData();
  }
};
</script>

<style  lang="less" scoped>
.lnavgrid {
  > .title {
    padding: 0.2rem 0;

    .title-left {
      .icon {
        height: 1.3rem;
      }

      .title {
        font-size: 1.2rem;
      }
    }
  }
}
</style>