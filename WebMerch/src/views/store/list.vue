<template>
  <div class="store-container">
    <div class="filter-container">
      <el-input v-model="listQuery.name" placeholder="名称" va style="width: 200px;" class="filter-item" @keyup.enter.native="handleFilter" />
      <el-button class="filter-item" style="margin-left: 10px;" type="primary" icon="el-icon-search" @click="handleFilter">
        查询
      </el-button>
      <el-button class="filter-item" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleCreate">
        新建
      </el-button>
    </div>
    <el-row v-loading="listLoading" :gutter="20">

      <el-col v-for="item in listData" :key="item.id" :span="6" :xs="24" style="margin-bottom:20px">
        <el-card class="box-card">
          <div slot="header" class="header-item clearfix">
            <span>{{ item.name }}</span>
            <el-button style="float: right; padding: 3px 0" type="text" @click="handleUpdate(item)">管理</el-button>
          </div>
          <div class="component-item">
            <div class="it-img"> <img :src="item.mainImgUrl" alt=""> </div>
            <div class="it-describe" />
          </div>
        </el-card>
      </el-col>

    </el-row>
  </div>
</template>

<script>
import { fetchList } from '@/api/store'

export default {
  name: 'StoreList',
  data() {
    return {
      listQuery: {
        page: 1,
        limit: 10,
        name: undefined
      },
      listData: [],
      listLoading: true
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
      this.listLoading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      fetchList(this.listQuery).then(res => {
        var d = res.data
        this.listData = d
        this.listLoading = false
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
    handleUpdate(row) {
      this.$router.push({
        path: '/store/edit?storeId=' + row.id
      })
    }
  }
}
</script>

<style lang="scss" scoped>

.store-container{
  padding: 20px;

  .header-item{
    .it-login{
      float: right;
    }
  }
  .component-item{
    min-height: 100px;
    display: flex;
    .it-img{
      width: 120px;
      height: 120px;

      img{
        width: 100%;
        height: 100%;
      }
    }

    .it-describe{
      flex: 1;
      padding: 5px;
      font-size: 12px;
    }
  }
}
</style>
