<template>
  <div class="skus-container">
    <el-row v-loading="loading" :gutter="20">
      <el-col v-for="productSku in listData" :key="productSku.id" :span="4" :xs="24" style="margin-bottom:20px">
        <el-card class="box-card">
          <div slot="header" class="header-item clearfix">
            <span>{{ productSku.name }}</span>
            <el-button style="float: right; padding: 3px 0" type="text" @click="handleUpdate(productSku)">管理</el-button>
          </div>
          <div class="component-item">
            <div class="it-img"> <img :src="productSku.mainImgUrl" alt=""> </div>
            <div class="it-describe" />
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script>
import { getStoreProductSkuList } from '@/api/store'

export default {
  name: 'StoreList',
  props: {
    storeId: {
      type: String,
      default: ''
    },
    sellchannel: {
      type: Object,
      default: null
    }
  },
  data() {
    return {
      loading: true,
      listQuery: {
        page: 0,
        limit: 10,
        refType: undefined,
        refId: undefined,
        storeId: undefined
      },
      listData: [],
      activeNames: ''
    }
  },
  created() {
    this.listQuery.storeId = this.storeId
    this.listQuery.refId = this.sellchannel.refId
    this.listQuery.refType = this.sellchannel.refType
    console.log(JSON.stringify(this.sellchannel))
    this.getListData(this.listQuery)
  },
  methods: {
    getListData(listQuery) {
      console.log('getListData')
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: listQuery })
      getStoreProductSkuList(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
        }
        this.loading = false
      })
    }
  }
}
</script>

<style lang="scss" scoped>

.skus-container{
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
