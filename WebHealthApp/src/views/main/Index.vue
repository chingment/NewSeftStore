<template>
  <div style="background:#f7f7f7">
    <div style="margin-bottom:79px">
      <router-view />
    </div>
    <div class="tabbar">
      <div v-for="(item ,index) in tabBar" :key="index" class="tabbar_item">
        <router-link :to="item.pagePath">
          <img class="icon" :src="$route.path === item.pagePath ? item.selectedIconPath : item.iconPath" alt="">
          <span class="iconfont icon-icon_huabanfuben" />
          <p class="title">{{ item.text }}</p>
        </router-link>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  name: 'Tabbar',
  data() {
    return {
      tabBar: [
        {
          pagePath: '/report',
          iconPath: require('../../assets/images/tabbar/report_off.png'),
          selectedIconPath: require('../../assets/images/tabbar/report_on.png'),
          text: '检测'
        },
        {
          pagePath: '/energy',
          iconPath: require('../../assets/images/tabbar/energy_off.png'),
          selectedIconPath: require('../../assets/images/tabbar/energy_on.png'),
          text: '机能'
        },
        {
          pagePath: '/advise',
          iconPath: require('../../assets/images/tabbar/advise_off.png'),
          selectedIconPath: require('../../assets/images/tabbar/advise_on.png'),
          text: '建议'
        }
      ]
    }
  },
  methods: {
    switchToTab(path) {
      this.$router.replace(path)
    }
  }
}
</script>

<style lang="scss" scoped>
.tabbar {
  text-align: center;
  width: 100%;
  height: 79px;
  line-height: 22px;
  position: fixed;
  bottom: 0;
  display: flex;
  justify-content: space-around;
  align-items: center;
  border-top: 1px solid gainsboro;
  padding: 10px 0px;
  background: #fff;
}
a {
  text-decoration: none;
  color: black;
  outline: none;
  appearance:none;
 -webkit-tap-highlight-color: transparent;
}
.router-link-active {
  color: #FF7703;
}

.icon{
  width: 26px;
  height: 26px;
}

.title {
  font-size: 18px;
}
</style>
