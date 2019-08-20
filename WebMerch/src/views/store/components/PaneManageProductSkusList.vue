<template>
  <div class="skus-container">
    <el-row v-loading="loading" :gutter="20">
      <el-col v-for="productSku in listData" :key="productSku.id" :span="6" :xs="24" style="margin-bottom:20px">
        <el-card class="box-card">
          <div class="above">
            <div class="above-des">
              <div class="des1">
                <div class="name">{{ productSku.name }}</div>
                <div class="price"> <span class="saleprice">{{ productSku.salePrice }}</span> </div>
              </div>
              <div class="des2">
                <span class="sellQuantity">{{ productSku.sellQuantity }}</span> /
                <span class="lockQuantity">{{ productSku.lockQuantity }}</span> /
                <span class="sumQuantity">{{ productSku.sumQuantity }}</span>
              </div>
            </div>
            <div class="above-img"> <img :src="productSku.mainImgUrl" alt=""> </div>
          </div>
          <div class="below">
            <div class="below-left">
              <el-button type="success">置满</el-button>
              <el-button type="warning">沽清</el-button>
            </div>
            <div class="below-right">
              <el-button type="primary">编辑</el-button>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>
    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />
  </div>
</template>

<script>
import { getStoreProductSkuList } from '@/api/store'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination
export default {
  name: 'StoreList',
  components: { Pagination },
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
      listTotal: 0,
      listQuery: {
        page: 1,
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
          this.listTotal = d.total
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

  .above{
    height: 110px;
    display: flex;

    .above-des{
     flex: 1;
      .des1{
        height: 80px;

        .name{
          line-height: 21px;
          font-size: 16px;
          color: #909399;
        }

        .saleprice{
          color: #f56c6c;
          line-height: 18px;
        }
      }
      .des2{
        height: 30px;

        .sellQuantity{
          color: #67c23a;
        }
        .lockQuantity{
color: #f56c6c;
        }
        .sumQuantity{
color: #e6a23c;
        }
      }
     }
    .above-img{
     img{
      width: 110px;
      height: 110px;
    }
     }
  }

  .below{
       display: flex;

       .below-left{
          flex: 2;
          text-align: left;
       }

       .below-right{
  flex: 1;
  text-align: right;
       }
  }

   .el-button{
     font-size: 12px ;
     padding: 6px 12px;
   }

}
</style>
