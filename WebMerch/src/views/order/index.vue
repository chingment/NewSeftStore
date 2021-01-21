<template>
  <div id="productsku_list" class="app-container my-container1">
    <div class="filter-container">
      <el-container>
        <el-aside width="200px">
          <el-menu router :default-active="navActive">
            <el-menu-item-group>
              <template slot="title">订单中心</template>
              <el-menu-item index="/order/list">全部订单</el-menu-item>
              <el-menu-item index="/order/listbymt">机器自提</el-menu-item>
              <el-menu-item index="/order/listbyst">店铺自提</el-menu-item>
              <el-menu-item index="/order/listbyet">配送到手</el-menu-item>
            </el-menu-item-group>
          </el-menu>
        </el-aside>
        <el-container>
          <el-main>
            <router-view  />
          </el-main>
        </el-container>
      </el-container>

    </div>
  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
export default {
  name: 'OrderCenter',
  data() {
    return {
      navActive: '/order/list',
      isDesktop: this.$store.getters.isDesktop,
    }
  },
  created() {
 this.navActive=this.$route.path
 console.log('this.$route.params.path:'+this.$route.path)
  },
  methods: {

  }
}
</script>


<style lang="scss" scoped>

.my-container1{
  background: #fbfbfb;
  padding: 0px;
  .el-menu{
    background: #fbfbfb;
    border-right: solid 0px #e6e6e6;
    margin-right: 10px;
  }

  .el-aside{
    background: #fbfbfb;
  }

  .el-container{

  }
  .el-main{
    background: #ffffff;
    min-height: calc(100vh - 100px);
    box-shadow: 0 2px 12px 0 rgba(0,0,0,.1);
  }
}
</style>
