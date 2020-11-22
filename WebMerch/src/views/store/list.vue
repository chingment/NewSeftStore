<template>
  <div id="store_list" class="app-container">

    <div class="circle-status-bar">
      <div class="circle-item"> <span class="icon-status icon-status-1" /> <span class="name">关闭</span></div>
      <div class="circle-item"> <span class="icon-status icon-status-2" /> <span class="name">正常</span></div>
    </div>
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-input v-model="listQuery.name" clearable style="width: 100%" placeholder="店铺名称" va class="filter-item" @keyup.enter.native="handleFilter" @clear="handleFilter" />
        </el-col>
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter">
            查询
          </el-button>
        </el-col>
      </el-row>
    </div>
    <el-row v-loading="loading" :gutter="20">

      <el-col v-for="item in listData" :key="item.id" :span="6" :xs="24" style="margin-bottom:20px">
        <el-card class="box-card">
          <div slot="header" class="it-header clearfix">
            <div class="left">

              <div class="circle-item"> <span :class="'icon-status icon-status-'+item.status.value" /> <span class="name">{{ item.name }}</span> </div>

            </div>
            <div class="right">
              <el-button type="text" @click="handleManage(item)">管理</el-button>
            </div>
          </div>
          <div class="it-component">
            <div class="img"> <img :src="item.mainImgUrl" alt=""> </div>
            <div class="describe">
              <ul>
                <li v-if="item.sctMode.indexOf('K')>-1"><el-button type="text" style="padding:0px" @click="handleManageMachine(item)">机器管理</el-button></li>
                <li><el-button type="text" style="padding:0px;color:#67c23a" @click="handleManageOrder(item)">订单信息</el-button></li>
                <li><el-button type="text" style="padding:0px;color:#f38b3f" @click="handleManageProduct(item)">商品分类</el-button></li>
                <li><el-button type="text" style="padding:0px;color:#f38b3f;display:none" @click="handleManageExpressStock(item)">快递库存</el-button></li>
              </ul>
            </div>
          </div>
        </el-card>
      </el-col>

      <el-col v-if="mctMode.indexOf('M')>-1" :span="6" :xs="24" style="margin-bottom:20px">
        <el-card class="box-card">
          <div slot="header" class="it-header clearfix">
            <div class="left" />
            <el-button type="text" @click="handleCreate">新建</el-button>
          </div>
          <div class="it-component">

            <div style="margin:auto;height:120px !important;width:120px !important; line-height:125px;" class="el-upload el-upload--picture-card" @click="handleCreate"><i data-v-62e19c49="" class="el-icon-plus" /></div>

          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script>
import { getList } from '@/api/store'

export default {
  name: 'StoreList',
  data() {
    return {
      loading: true,
      listQuery: {
        page: 1,
        limit: 10,
        name: undefined
      },
      listData: [],
      mctMode: ''
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }
    this.mctMode = this.$store.getters.userInfo.mctMode
    this.getListData()
  },
  methods: {
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
    handleCreate() {
      this.$router.push({
        path: '/store/add'
      })
    },
    handleManage(row) {
      this.$router.push({
        path: '/store/manage?id=' + row.id + '&tab=tabBaseInfo'
      })
    },
    handleManageMachine(row) {
      this.$router.push({
        path: '/store/manage?id=' + row.id + '&tab=tabMachine'
      })
    },
    handleManageOrder(row) {
      this.$router.push({
        path: '/store/manage?id=' + row.id + '&tab=tabOrder'
      })
    },
    handleManageExpressStock(row) {
      this.$router.push({
        path: '/store/manage?id=' + row.id + '&tab=tabExpressStock'
      })
    },
    handleManageProduct(row) {
      this.$router.push({
        path: '/store/manage?id=' + row.id + '&tab=tabProduct'
      })
    }
  }
}
</script>

<style lang="scss" scoped>

#store_list{
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
      max-width: 100px;
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
