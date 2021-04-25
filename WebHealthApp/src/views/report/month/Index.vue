<template>
  <div style="background:#f7f7f7">
    <div style="margin-bottom:59px">
      <router-view />
    </div>
    <div class="tabbar">
      <div v-for="(item ,index) in tabBar" :key="index" class="tabbar_item">
        <a @click="switchToTab(item.pagePath)">
          <div class="t1"><img class="icon" :src="$route.path === item.pagePath ? item.selectedIconPath : item.iconPath" alt=""></div>
          <div class="t2"><span class="title">{{ item.text }}</span></div>
        </a>
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
          pagePath: '/report/month/monitor',
          iconPath: require('@/assets/images/tabbar/monitor_off.png'),
          selectedIconPath: require('@/assets/images/tabbar/monitor_on.png'),
          text: '检测'
        },
        {
          pagePath: '/report/month/energy',
          iconPath: require('@/assets/images/tabbar/energy_off.png'),
          selectedIconPath: require('@/assets/images/tabbar/energy_on.png'),
          text: '机能'
        },
        {
          pagePath: '/report/month/advise',
          iconPath: require('@/assets/images/tabbar/advise_off.png'),
          selectedIconPath: require('@/assets/images/tabbar/advise_on.png'),
          text: '建议'
        }
      ]
    }
  },
  methods: {
    switchToTab(path) {
      console.log(this.$route.query)
      this.$router.replace(path + '?rptId=' + this.$route.query.rptId)
    }
  }
}
</script>

<style lang="scss" scoped>
.tabbar {
  text-align: center;
  width: 100%;
  height: 59px;
  position: fixed;
  bottom: 0;
  display: flex;
  justify-content: space-around;
  align-items: center;
  border-top: 1px solid gainsboro;
  padding-top: 10px;
  background: #fff;

a {
  text-decoration: none;
  color: black;
  outline: none;
  appearance:none;
 -webkit-tap-highlight-color: transparent;
}

.t1{
  height: 24px;

  .icon{
  width: 24px;
  height: 24px;
}
}

.t2{
    height: 28px;
    line-height: 28px;
    font-size: 14px;
}
}
</style>
