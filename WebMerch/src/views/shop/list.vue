<template>
  <div id="shop_list">
    <div class="filter-container">
      <el-row :gutter="24">
        <el-col :xs="24" :sm="12" :lg="8" :xl="6" style="margin-bottom:20px">
          <el-input v-model="listQuery.name" clearable style="width: 100%" placeholder="门店名称" class="filter-item" />
        </el-col>
        <el-col :xs="24" :sm="12" :lg="8" :xl="6" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="onFilter">
            查询
          </el-button>
        </el-col>
      </el-row>
    </div>
    <el-row v-loading="loading" :gutter="24">

      <el-col v-for="item in listData" :key="item.id" :xs="24" :sm="12" :lg="8" :xl="6" class="my-col">
        <el-card class="box-card">
          <div slot="header" class="it-header clearfix">
            <div class="left">

              <div class="circle-item"> <span class="name">{{ item.name }}</span> </div>

            </div>
            <div class="right">
              <el-button type="text" @click="onEdit(item)">编辑</el-button>
            </div>
          </div>
          <div class="it-component">
            <div class="img"> <img :src="item.mainImgUrl" alt=""> </div>
            <div class="describe" />
          </div>
        </el-card>
      </el-col>

      <el-col :xs="24" :sm="12" :lg="8" :xl="6" style="margin-bottom:20px">
        <el-card class="box-card">
          <div slot="header" class="it-header clearfix">
            <div class="left" />
            <el-button type="text" @click="onAdd">新建</el-button>
          </div>
          <div class="it-component">
            <div style="margin:auto;height:120px !important;width:120px !important; line-height:125px;" class="el-upload el-upload--picture-card" @click="onAdd"><i data-v-62e19c49="" class="el-icon-plus" /></div>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script>
import { getList } from '@/api/shop'

export default {
  name: 'ShopList',
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
    this.onGetList()
  },
  methods: {
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
    onAdd() {
      this.$router.push({
        path: '/shop/add'
      })
    },
    onEdit(item) {
      this.$router.push({
        path: '/shop/edit?id=' + item.id
      })
    }
  }
}
</script>

<style lang="scss" scoped>

#shop_list{
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
