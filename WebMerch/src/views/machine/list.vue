<template>
  <div id="machine_list" class="app-container">

    <div class="circle-status-bar">
      <span class="circle-status circle-status-2" /> <span class="name">正常</span>
      <span class="circle-status circle-status-1" /> <span class="name">关闭</span>
      <span class="circle-status circle-status-3" /> <span class="name">异常</span>
    </div>
    <div class="filter-container">
      <el-input v-model="listQuery.name" placeholder="机器名称" va style="width: 200px;" class="filter-item" @keyup.enter.native="handleFilter" />
      <el-button class="filter-item" style="margin-left: 10px;" type="primary" icon="el-icon-search" @click="handleFilter">
        查询
      </el-button>
    </div>
    <el-row v-loading="loading" :gutter="20">

      <el-col v-for="item in listData" :key="item.id" :span="6" :xs="24" style="margin-bottom:20px">
        <el-card class="box-card">
          <div slot="header" class="it-header clearfix">
            <div class="left">
              <span :class="'circle-status circle-status-'+item.status.value" /> <span class="name">{{ item.name }}</span>
            </div>
            <div class="right">
              <el-button type="text" @click="handleUpdate(item)">设置</el-button>
            </div>
          </div>
          <div class="it-component">
            <div class="img"> <img :src="item.mainImgUrl" alt=""> </div>
            <div class="describe">
              <ul>
                <li><el-button type="text" @click="handleViewStock(item)">库存管理</el-button></li>
                <li><el-button type="text" style="color:#67c23a" @click="handleViewOrder(item)">订单信息</el-button></li>
              </ul>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <div v-show="listData.length<=0" class="list-empty">
      <span>暂无数据</span>
    </div>
  </div>
</template>

<script>
import { getList } from '@/api/machine'

export default {
  name: 'MachineList',
  data() {
    return {
      loading: true,
      listQuery: {
        page: 1,
        limit: 10,
        name: undefined
      },
      listData: []

    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }
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
    handleUpdate(row) {
      this.$router.push({
        path: '/machine/manage?id=' + row.id + '&tab=tabBaseInfo'
      })
    },
    handleViewStock(row) {
      this.$router.push({
        path: '/machine/manage?id=' + row.id + '&tab=tabStock'
      })
    },
    handleViewOrder(row) {
      this.$router.push({
        path: '/machine/manage?id=' + row.id + '&tab=tabOrder'
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
    padding: 0px 5px;
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
      padding: 5px;
      font-size: 12px;

      ul{
        padding: 0px;
        margin: 0px;
        list-style: none;
         li{
        height: 26px;
        line-height: 26px;
      }
      }
    }

  }
}
</style>
