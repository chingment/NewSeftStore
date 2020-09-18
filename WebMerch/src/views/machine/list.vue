<template>
  <div id="machine_list" class="app-container">

    <div class="circle-status-bar">
      <div class="circle-item"> <span class="icon-status icon-status-1" /> <span class="name">关闭</span></div>
      <div class="circle-item"> <span class="icon-status icon-status-2" /> <span class="name">正常</span></div>
      <div class="circle-item"> <span class="icon-status icon-status-4" /> <span class="name">维护</span></div>
      <div class="circle-item"> <span class="icon-status icon-status-3" /> <span class="name">异常</span></div>
    </div>
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-input v-model="listQuery.id" clearable style="width: 100%" placeholder="机器编号" va class="filter-item" @keyup.enter.native="handleFilter" @clear="handleFilter" />
        </el-col>
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter">
            查询
          </el-button>
        </el-col>
      </el-row>
    </div>
    <el-row v-loading="loading" :gutter="20">

      <el-col v-for="item in listData" v-show="machineCount!==0" :key="item.id" :span="6" :xs="24" style="margin-bottom:20px">
        <el-card class="box-card">
          <div slot="header" class="it-header clearfix">
            <div class="left">

              <div class="circle-item"> <span :class="'icon-status icon-status-'+item.status.value" /> <span class="name">{{ item.name }} <span style="font-size:12px;"> ({{ item.status.text }})</span></span></div>

            </div>
            <div class="right">
              <el-button type="text" @click="handleManage(item)">管理</el-button>
            </div>
          </div>
          <div class="storeName" style="font-size:12px;">{{ item.storeName }} </div>
          <div class="it-component">

            <div class="img"> <img :src="item.mainImgUrl" alt=""> </div>
            <div class="describe">
              <ul>
                <li><el-button type="text" style="padding:0px;" @click="handleManageStock(item)">库存管理</el-button></li>
                <li><el-button type="text" style="padding:0px;color:#67c23a" @click="handleManageOrder(item)">订单信息</el-button></li>
                <li><el-button type="text" style="padding:0px;color:#f38b3f" @click="handleManageControlCenter(item)">控制中心</el-button></li>
              </ul>
            </div>
          </div>
        </el-card>
      </el-col>

      <el-col v-show="machineCount===0" :span="6" :xs="24" style="margin-bottom:20px">
        <el-card class="box-card">
          <div slot="header" class="it-header clearfix">
            <div class="left" />
            <el-button type="text">暂无机器，请联系您的客户经理绑定！</el-button>
          </div>
          <div class="it-component">

            <div style="margin:auto;height:120px !important;width:120px !important; line-height:125px;" class="el-upload el-upload--picture-card"><i data-v-62e19c49="" class="el-icon-plus" /></div>

          </div>
        </el-card>
      </el-col>

    </el-row>

    <div v-show="listData.length<=0&&machineCount>0" class="list-empty">
      <span>暂无数据</span>
    </div>
  </div>
</template>

<script>
import { getList, initGetList } from '@/api/machine'

export default {
  name: 'MachineList',
  data() {
    return {
      loading: true,
      listQuery: {
        page: 1,
        limit: 10,
        id: undefined
      },
      machineCount: 0,
      listData: []

    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }

    this.init()
  },
  methods: {
    init() {
      this.loading = true

      initGetList().then(res => {
        if (res.result === 1) {
          var d = res.data
          this.machineCount = d.machineCount

          if (d.machineCount > 0) {
            this.getListData()
          }
        }
        this.loading = false
      })
    },
    getListData() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      getList(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
        }
        this.loading = false
      })
    },
    handleFilter() {
      this.listQuery.page = 1
      this.getListData()
    },
    handleManage(row) {
      this.$router.push({
        path: '/machine/manage?id=' + row.id + '&tab=tabBaseInfo'
      })
    },
    handleManageStock(row) {
      this.$router.push({
        path: '/machine/manage?id=' + row.id + '&tab=tabStock'
      })
    },
    handleManageOrder(row) {
      this.$router.push({
        path: '/machine/manage?id=' + row.id + '&tab=tabOrder'
      })
    },
    handleManageControlCenter(row) {
      this.$router.push({
        path: '/machine/manage?id=' + row.id + '&tab=tabControlCenter'
      })
    }
  }
}
</script>

<style lang="scss" scoped>

#machine_list{
  padding: 20px;

  .it-header{
    display: flex;
    justify-content: flex-start;
    align-items: center;
    position: relative;
    height:20px ;
    .left{
      flex: 1;
      justify-content: flex-start;
      align-items: center;
      display: block;
      height: 100%;
    overflow: hidden;
text-overflow:ellipsis;
white-space: nowrap;
    .name{
padding: 0 5px;
    display: inline-block;
    flex: 1;
    overflow: hidden;
    white-space: nowrap;
    text-overflow: ellipsis
    }
    }
    .right{
      width: 100px;
      display: flex;
      justify-content: flex-end;
      align-items: center;
    }

  }
  .it-component{
    min-height: 100px;
    display: flex;
    .img{
      width: 120px;
      height: 120px;

      img{
        width: 100%;
        height: 100%;
      }
    }

    .describe{
      flex: 1;
      padding: 0px;
      font-size: 12px;

      ul{
        padding: 0px;
        margin: 0px;
        list-style: none;
         li{
           width: 100%;
           text-align: right;
        height: 26px;
        line-height: 26px;
      }
      }
    }

  }
}
</style>
