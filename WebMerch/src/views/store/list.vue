<template>
  <div id="store_list">
    <div class="filter-container">
      <el-row :gutter="24">
        <el-col :xs="24" :sm="12" :lg="8" :xl="6" style="margin-bottom:20px">
          <el-input v-model="listQuery.name" clearable style="width: 100%" placeholder="店铺名称" class="filter-item" />
        </el-col>
        <el-col :xs="24" :sm="12" :lg="6" :xl="6" style="margin-bottom:20px">
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
              <el-button type="text" @click="onManage(item)">管理</el-button>
            </div>
          </div>
          <div class="it-component">
            <div class="img"> <img :src="item.mainImgUrl" alt=""> </div>
            <div class="describe">
              <ul>
                <li><el-button type="text" style="padding:0px;color:#67c23a" @click="onKind(item)">商品分类</el-button></li>
                <li><el-button type="text" style="padding:0px;color:#f38b3f" @click="onShop(item)">门店信息</el-button></li>
              </ul>
            </div>
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
    onManage(item) {
      this.$router.push({
        name: 'MerchStoreEdit',
        path: '/store/manage',
        params: {
          id: item.id,
          tab: 'tabBaseInfo'
        }
      })
    },
    onKind(item) {
      this.$router.push({
        name: 'MerchStoreEdit',
        path: '/store/manage',
        params: {
          id: item.id,
          tab: 'tabKind'
        }
      })
    },
    onShop(item) {
      this.$router.push({
        name: 'MerchStoreEdit',
        path: '/store/manage',
        params: {
          id: item.id,
          tab: 'tabShop'
        }
      })
    }
  }
}
</script>

<style lang="scss" scoped>

#store_list{
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
