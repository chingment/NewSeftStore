<template>
  <div id="device_list">

    <div class="circle-status-bar">
      <div class="circle-item"> <span class="icon-status icon-status-1" /> <span class="name">关闭</span></div>
      <div class="circle-item"> <span class="icon-status icon-status-2" /> <span class="name">正常</span></div>
      <div class="circle-item"> <span class="icon-status icon-status-4" /> <span class="name">维护</span></div>
      <div class="circle-item"> <span class="icon-status icon-status-3" /> <span class="name">异常</span></div>
    </div>
    <div class="filter-container">

      <el-row :gutter="24">
        <el-col :xs="24" :sm="12" :lg="8" :xl="span" style="margin-bottom:20px">
          <el-input v-model="listQuery.id" clearable style="width: 100%" placeholder="设备编码" class="filter-item" />
        </el-col>
        <el-col :xs="24" :sm="12" :lg="8" :xl="span" style="margin-bottom:20px;display:flex;">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="onFilter">
            查询
          </el-button>
        </el-col>
      </el-row>
    </div>
    <el-row v-loading="loading" :gutter="24">
      <el-col v-for="item in listData" v-show="deviceCount!==0" :key="item.id" :xs="24" :sm="12" :lg="8" :xl="span" class="my-col">
        <el-card class="box-card">
          <div slot="header" class="it-header clearfix">
            <div class="left">
              <div class="circle-item"> <span :class="'icon-status icon-status-'+item.status.value" /> <span class="name">{{ item.code }} <span style="font-size:12px;"> ({{ item.status.text }})</span></span></div>
            </div>
            <div class="right">
              <el-button type="text" @click="onManage(item)">管理</el-button>
            </div>
          </div>
          <div class="storeName" style="font-size:12px;white-space: nowrap">{{ item.shopName }} [{{ item.lastRequestTime }}]</div>
          <div class="it-component">
            <div class="img"> <img :src="item.mainImgUrl" alt=""> </div>
            <div class="describe">
              <ul>
                <li><el-button type="text" style="padding:0px;color:#67c23a" @click="onStock(item)">库存查看</el-button></li>
                <li><el-button type="text" style="padding:0px;color:#f38b3f" @click="onControlCenter(item)">控制中心</el-button></li>
              </ul>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col v-show="deviceCount===0" :xs="24" :sm="12" :lg="8" :xl="6" style="margin-bottom:20px">
        <el-card class="box-card">
          <div slot="header" class="it-header clearfix">
            <div class="left" />
            <el-button type="text">暂无设备，请联系您的客户经理绑定！</el-button>
          </div>
          <div class="it-component">
            <div style="margin:auto;height:120px !important;width:120px !important; line-height:125px;" class="el-upload el-upload--picture-card"><i data-v-62e19c49="" class="el-icon-plus" /></div>
          </div>
        </el-card>
      </el-col>

    </el-row>
    <div v-show="listData.length<=0&&deviceCount>0" class="list-empty">
      <span>暂无数据</span>
    </div>

  </div>
</template>

<script>

import { getList, initGetList } from '@/api/devvending'

export default {
  name: 'DeviceVending',
  data() {
    return {
      loading: true,
      listKey: 'listQuery',
      listQuery: {
        page: 1,
        limit: 1000,
        id: '',
        type: 'vending'
      },
      deviceCount: 0,
      listData: [],
      span: 6,
      isDesktop: this.$store.getters.isDesktop
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
          this.deviceCount = d.deviceCount

          if (d.deviceCount > 0) {
            this.onGetList()
          }
        }
        this.loading = false
      })
    },
    onGetList() {
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
    onFilter() {
      this.listQuery.page = 1
      this.onGetList()
    },
    onManage(item) {
      this.$router.push({
        name: 'MerchDeviceManage',
        path: '/device/vending/manage',
        params: {
          id: item.id,
          tab: 'tabBaseInfo'
        }
      })
    },
    onStock(item) {
      this.$router.push({
        name: 'MerchDeviceManage',
        path: '/device/vending/manage',
        params: {
          id: item.id,
          tab: 'tabStock'
        }
      })
    },
    onControlCenter(item) {
      this.$router.push({
        name: 'MerchDeviceManage',
        path: '/device/vending/manage',
        params: {
          id: item.id,
          tab: 'tabControlCenter'
        }
      })
    }
  }
}
</script>

<style lang="scss" scoped>

#device_list{

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
