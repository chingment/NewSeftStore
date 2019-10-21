<template>
  <div id="home_container">

    <el-row :gutter="8">
      <el-col :xs="{span: 24}" :sm="{span: 24}" :md="{span: 24}" :lg="{span: 12}" :xl="{span: 12}" style="padding-right:8px;margin-bottom:30px;">

        <el-card class="box-card">
          <div slot="header" class="clearfix">
            <span>今日总览</span>
          </div>
          <div class="today-sum" style="height:200px;">
            <div class="it">
              <div class="t1"><span class="m1"> 0</span><br> <span class="m3">今日订单</span></div>
            </div>
            <div class="it">
              <div class="t1"><span class="m2"> 0</span><br> <span class="m3">今日营业额</span></div>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :xs="{span: 24}" :sm="{span: 12}" :md="{span: 12}" :lg="{span: 6}" :xl="{span: 6}" style="margin-bottom:30px;">

        <el-card class="box-card">
          <div slot="header" class="clearfix">
            <span>今日店铺销售额</span>

            <el-button style="float: right; padding: 0px 0" type="text">更多</el-button>
          </div>
          <div style="height:200px;" />
        </el-card>

      </el-col>
      <el-col :xs="{span: 24}" :sm="{span: 12}" :md="{span: 12}" :lg="{span: 6}" :xl="{span: 6}" style="margin-bottom:30px;">

        <el-card class="box-card">
          <div slot="header" class="clearfix">
            <span>最近7天销售额</span>
          </div>
          <div style="height:200px;" />
        </el-card>

      </el-col>
    </el-row>

    <el-row :gutter="8">
      <el-col :xs="{span: 24}" :sm="{span: 24}" :md="{span: 24}" :lg="{span: 12}" :xl="{span: 12}" style="padding-right:8px; margin-bottom:30px;">

        <el-card class="box-card">
          <div slot="header" class="clearfix">
            <span>店铺销售排行榜</span>
          </div>
          <div style="min-height:300px;">
            <el-button type="text" @click="goNewStore(item)">暂无店铺，马上新建，GO GO GO</el-button>
          </div>
        </el-card>
      </el-col>
      <el-col :xs="{span: 24}" :sm="{span: 24}" :md="{span: 24}" :lg="{span: 12}" :xl="{span: 12}" style="margin-bottom:30px; ">

        <el-card class="box-card">
          <div slot="header" class="clearfix">
            <span>热销商品排行榜</span>
            <el-button style="float: right; padding: 0px 0" type="text">更多</el-button>
          </div>
          <div style="min-height:300px;">
            <span>暂无销售数据</span>
          </div>
        </el-card>

      </el-col>

    </el-row>

  </div>
</template>

<script>

export default {

  data() {
    return {
    }
  },
  created() {
  },
  methods: {
    goNewStore: function(e) {
      this.$router.push({
        path: '/store/add'
      })
    }
  }
}

</script>

<style lang="scss" scoped>
#home_container{
  padding: 20px;
}

.today-sum{
  display: flex;
  .it{
    flex: 1;
    justify-content: center;
    align-items: center;
    align-content: center;
    display: flex;

    .t1{
      text-align: center;

      .m1{
       font-size: 42px;
      }

       .m2{
       font-size: 42px;
       color: #cf9236;
      }
    }
  }

}
</style>
