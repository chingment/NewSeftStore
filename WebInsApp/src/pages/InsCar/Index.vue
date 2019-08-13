<template>
  <div id="app_wrapper">
    <lumos-header
      :title="header.title"
      :rightButton="header.rightButton"
      v-on:rightButtonVonBadgeChange="rightButtonVonBadgeChange"
    ></lumos-header>


   <div class="block-companyrules pd" >
    <div class="titlebar">
      <div class="titlebar-left">
        <span class="icon">
          <img src="@/assets/images/home/titlebar_icon.png">
        </span>
        <span class="title">保险公司</span>
      </div>
      <div class="titlebar-right">
      </div>
    </div>

    <ul class="list-companyrules" v-if="companyRules.length>0" >
      <template v-for="(companyRule,index) in this.companyRules">
        <li class="item" :key="index">
          <div class="item-left">
            <span class="icon">
              <img :src="companyRule.companyImgUrl">
            </span>
            <span class="name">{{ companyRule.companyName }}</span>
          </div>
          <div class="item-right">
            <span class="rate">{{ companyRule.commissionRate }}</span>
          </div>
        </li>
      </template>
    </ul>
     <div class="empty-companyrules" v-else>
      暂无数据
    </div>
  </div>
   <div class="space"></div>

  <div class="block-serarch pd" >
    <div class="lnavgrid" >

     <div class="field">
     <plateNumber @getPlateLicense="getPlateLicense" ></plateNumber>
    
     <div class="item" >
      <div class="item-lefticon hid"></div>
      <div class="item-content" >  
      <div class="title" > 投保城市  </div>
      <div class="note" @click="citySelectOpen()" > {{ city.localCity.cityName }} </div>
      </div>  
      <div class="item-righticon" >
           <img 
            src="@/assets/images/icon_right.png"
            alt="">
      </div>      
     </div>



     
     </div>   
 
      

    </div>

 

   
      <div class="manr">

       <span @click="goMsCarInfo"  >人工报价</span>
       <img src="@/assets/images/icon_right.png" alt="">


      </div>

  </div>
<div class="block-searchplatenorecords pd">
    <div class="titlebar">
      <div class="titlebar-left">
        <span class="icon">
          <img src="@/assets/images/home/titlebar_icon.png">
        </span>
        <span class="title">历史记录</span>
      </div>
      <div class="titlebar-right"></div>
    </div>
   <div class="list-searchplatenorecords" v-if="searchPlateNoRecords.length>0">
      <template v-for="(searchPlateNoRecord,index) in this.searchPlateNoRecords">
        <div class="item" :key="index">
            <span>{{ searchPlateNoRecord.plateNo }}</span>
        </div>
      </template>
   </div>
  <div class="empty-searchplatenorecords" v-else>
      暂无记录
    </div>
   </div>

  
         <!-- <a @click="goLink" >测试点击</a>  -->

    <!-- {{ this.$store.getters.getUId }} -->


        <lumos-cityselect
            :is-show.sync='city.isShow'
            :on-choose='city.onChoose'
            :city-data='city.cityData'
            :local-city='city.localCity'
            :star-city='city.starCity'
		      	:close="citySelectClose"
            ></lumos-cityselect>

  </div>
</template>

<style  lang="less" scoped>
.block-companyrules,
.block-searchplatenorecords {
  background-color: #fff;
  padding-top: .8rem;
  padding-bottom: .8rem
}
.block-serarch {
  background-color: #fff;
}

.titlebar {
  display: flex;
  align-items: center;
  background-color: #fff;
}

.titlebar .titlebar-left {
  flex: 1;
  justify-content: flex-start;
  display: flex;
  align-items: center;
}

.titlebar .titlebar-left .icon {
  width: 0.3rem;
  height: 1.6rem;
}

.titlebar .titlebar-left .title {
  font-weight: 600;
  font-size: 1.2rem;
  margin-left: 0.3rem;
}

.titlebar .titlebar-right {
  flex: 1;
  justify-content: flex-end;
  display: flex;
}

.list-companyrules {
}
.list-companyrules > .item {
  display: flex;
  align-content: center;
  border-bottom: 1px solid #f8f8f8;
  padding: 0.5rem 0;
}

.list-companyrules > .item:last-child {
  border-bottom-width: 0px;
}

.list-companyrules > .item .item-left {
  flex: 1;
  display: flex;
  align-content: center;
  align-items: center;
}

.list-companyrules > .item .item-left .icon {
  width: 5rem;
  height: 3rem;
  display: inline-block;
}
.list-companyrules > .item .item-right {
  flex: 1;
  display: flex;
  justify-content: flex-end;
  align-items: center;
}

.list-companyrules .name {
  font-size: 1.1rem;
  padding-left: 0.2rem;
}

.list-companyrules .rate {
  color: #006dee;
  font-size: 1.2rem;
  font-weight: 800;
}

.empty-companyrules {
  line-height: 2.3rem;
  text-align: left;
}

.list-searchplatenorecords {
  display: block;
  text-align: left;
  margin-top: 0.8rem;
}
.list-searchplatenorecords > .item {
  background-color: #f8f8f8;
  padding: 0.5rem 0.5rem;
  margin: 0.2rem;
  border-radius: 6px;
  display: inline-block;
  color: #b7b7b7;
}

.empty-searchplatenorecords {
  line-height: 2.3rem;
  text-align: left;
}

.manr{
  line-height: 2rem;
}

.manr span {
  color: #006dee;
  font-size: 0.8rem;
}
.manr img {
  width: 0.3rem;
  height: 0.6rem;
}
</style>


<script>
export default {
  data() {
    return {
      header: {
        title: {
          text: "车险询价"
        },
        leftButton: {
          iconPath: require("@/assets/images/home/icon_order.png"),
          vonBadge: {
            type: "number",
            text: "10"
          }
        },
        rightButton: {
          iconPath: require("@/assets/images/home/icon_order.png"),
          vonBadge: {
            type: "number",
            text: "10"
          }
        }
      },
      companyRules: [],
      searchPlateNoRecords: [],
      city: {
        isShow: false,
        cityData: [],
        onChoose: null,
        starCity: [],
        localCity: {
          cityId: 440,
          cityName: "广州",
          citySpell: "GUANGZHOU",
          cityFirstLetter: "G"
        }
      }
    };
  },
  methods: {
    getPlateLicense(data) {
      console.log("组件传出的data", data);
    },
    goLink() {
      this.$loading.show();

      // document.body.scrollTop = '300px';
      //this.header.title.text = "ssss"
      //this.header.rightButton.vonBadge.text = "12";

      // this.$nextTick(() => {
      //      document.body.scrollTop =1000
      // });
      
      //this.$store.dispatch('setUId', 'test');

      // this.$router.push({
      //   path: "/Hello",
      //   name: "Hello"
      // });
    },
    rightButtonVonBadgeChange() {
      console.log("dsads");
    },
    getData() {
      this.$http
        .get("/InsCar/GetIndexPageData")
        .then(res => {
          console.log(res);
          var d = res.data;
          this.companyRules = d.companyRules;
          this.searchPlateNoRecords = d.searchPlateNoRecords;
        });
    },
    getCityInfo: function() {
      this.city.cityData = [
        {
          cityId: 440,
          cityName: "广州",
          citySpell: "GUANGZHOU",
          cityFirstLetter: "G"
        },
        {
          cityId: 441,
          cityName: "深圳",
          citySpell: "SHENZHEN",
          cityFirstLetter: "S"
        }
      ];
    },
    citySelectOpen: function() {
      this.city.isShow = true;
    },
    citySelectClose() {
      this.city.isShow = false;
    },
    goAsCarInfo() {
      var plateNo = "粤A8K96A";
      let _this = this;
      this.$http
        .get("/InsCar/SearchPlateNoInfo", { plateNo: plateNo })
        .then(res => {
          if (res.result == 1) {
            var d = res.data;

            _this.$router.push({
              name:'InsCarAsCarInfo',
              path: "/InsCar/As/CarInfo",
              params: {
                carPlateNoInfo: d
              }
            });

          } else {
            this.$confirm({
              title: "提示",
              msg: "搜索不到车辆信息，是否需要人工报价？",
              yesBtnText: "人工报价",
              yesClick: () => {
                _this.$router.push({
                  path: "/InsCar/Ms/CarInfo"
                });
              }
            });
          }
        })
        
      //this.$toast("提示");
      // console.log("goAsCarInfo");
      // this.$router.push({
      //   path: "/InsCar/As/CarInfo"
      // });
    },
    goMsCarInfo() {
      console.log("goMsCarInfo");
      this.$router.push({
        path: "/InsCar/Ms/CarInfo"
      });
    }
  },
  mounted: function() {
    let _this = this;
    _this.getData();
    _this.getCityInfo();
    _this.city.onChoose = function(res) {
      _this.city.isShow = false;
      _this.city.localCity = res;
    };
  }
};
</script>