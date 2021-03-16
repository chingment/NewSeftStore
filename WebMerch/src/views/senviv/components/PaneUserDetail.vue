<template>
  <div id="senviv_user_detail" class="app-container my-container1" style="height:600px">

    <el-container style="padding:10px 0px">
      <el-aside style="width:300px;margin-right:10px">

        <el-card class="box-card box-card-product" :body-style="{ padding: '0px' }">
          <div class="it-header clearfix">
            <div class="left">
              <div class="l1">
                <el-avatar size="medium" />
              </div>
              <div class="l2">
                <span class="name">xx</span>
              </div>
            </div>
          </div>
          <div class="it-component">
            <div class="t1"><span class="sex">x</span> <span class="age">x岁</span> <span class="height">身高：x</span><span class="weight">体重：x</span></div>
            <div />
          </div>
        </el-card>

        <el-menu router :default-active="navActive">
          <el-menu-item-group>
            <el-menu-item index="/senviv/users">个人主页</el-menu-item>
            <el-menu-item index="/senviv/users">基本信息</el-menu-item>
            <el-menu-item index="/senviv/users">健康评价</el-menu-item>
            <el-menu-item index="/senviv/users">健康监测</el-menu-item>
            <el-menu-item index="/senviv/users">基线评估</el-menu-item>
          </el-menu-item-group>
        </el-menu>

      </el-aside>
      <el-container>
        <el-main class="">

          dsadsad

        </el-main>
      </el-container>
    </el-container>

  </div>
</template>

<style lang="scss" scoped>

.el-card__header{
     padding: 5px 5px !important;
 }

#senviv_user_detail
{

.el-col-5 {
  width: 20%;
}

.box-card-product {

  .it-header {
    display: flex;
    padding: 5px 20px;
    border-bottom: 1px solid #EBEEF5;

    .left {
      flex: 1;
      display: flex;
  overflow: hidden;
     .l1{
           display: flex;
    justify-content: center;
    align-items: center;
     }

     .l2{
       display: block;
      text-overflow: ellipsis;
    overflow: hidden;
    white-space: nowrap;
    line-height: 50px;
    padding-left:8px;

     }
    }
    .right {
      width: 50px;
      display: flex;
      justify-content: flex-end;
      align-items: center;
    }
  }
  .it-component {
    height: 160px;
    overflow: hidden;
    text-overflow: ellipsis;
    padding: 5px 20px;

  .t1{
    padding:  5px 0px;
    span{
    font-size: 14px;
    margin-left: 5px;
    }
  }
  }
}

}
</style>
