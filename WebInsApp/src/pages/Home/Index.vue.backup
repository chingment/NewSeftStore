<template>
  <div class="lumos-page">
    <!-- <button @click="clickParent">点击</button> -->
    <div class="lumos-tabbody">
    <router-view/>
    </div>
     <lumos-tabbar :tabs="tabs" ref="mychild"  ></lumos-tabbar> 
  </div>
</template>
<script>
export default {
  data() {
    return {
      tabs: [
        {
          name: "InsCar",
          text: "车险",
          pagePath: "/InsCar",
          iconPath: require("@/assets/images/home/tabbar_icon_insCar.png"),
          selectedIconPath: require("@/assets/images/home/tabbar_icon_insCar_fill.png"),
          vonBadge: {
            type: "circle",
            text: ""
          },
          selected: false
        },
        {
          name: "InsMarket",
          text: "意外险",
          pagePath: "/InsMarket",
          iconPath: require("@/assets/images/home/tabbar_icon_insMarket.png"),
          selectedIconPath: require("@/assets/images/home/tabbar_icon_insMarket_fill.png"),
          vonBadge: {
            type: "circle",
            text: ""
          },
          selected: false
        },
        {
          name: "InsClaim",
          text: "理赔",
          pagePath: "/InsClaim",
          iconPath: require("@/assets/images/home/tabbar_icon_insClaim.png"),
          selectedIconPath: require("@/assets/images/home/tabbar_icon_insClaim_fill.png"),
          vonBadge: {
            type: "number",
            text: ""
          },
          selected: false
        },
        {
          name: "My",
          text: "我的",
          pagePath: "/My",
          iconPath: require("@/assets/images/home/tabbar_icon_my.png"),
          selectedIconPath: require("@/assets/images/home/tabbar_icon_my_fill.png"),
          vonBadge: {
            type: "circle",
            text: ""
          },
          selected: false
        }
      ]
    };
  },
  methods: {
    clickParent() {
      this.$refs.mychild.setVonbadgeText(1, "嘿嘿嘿");
    }
  }
  
};
</script>



