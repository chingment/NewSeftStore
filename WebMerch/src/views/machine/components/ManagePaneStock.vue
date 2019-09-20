<template>
  <div id="machine_stock">
    <div class="filter-container">
      <el-input v-model="listQuery.name" placeholder="名称" va style="width: 200px;" class="filter-item" @keyup.enter.native="handleFilter" />
      <el-button class="filter-item" style="margin-left: 10px;" type="primary" icon="el-icon-search" @click="handleFilter">
        查询
      </el-button>
    </div>
    <el-row v-loading="loading" :gutter="20">
      <el-col v-for="(productSku,index) in listData" :key="index" :span="6" :xs="24" style="margin-bottom:20px">
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
import { initManageStock, manageStockGetStockList } from '@/api/machine'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination
import { getUrlParam } from '@/utils/commonUtil'
export default {
  name: 'ManagePaneStock',
  components: { Pagination },
  data() {
    return {
      loading: false,
      listTotal: 0,
      listQuery: {
        page: 1,
        limit: 10,
        name: undefined,
        machineId: undefined
      },
      listData: []
    }
  },
  watch: {
    '$route'(to, from) {
      this.init()
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      var id = getUrlParam('id')
      this.loading = true
      this.listQuery.machineId = id
      initManageStock({ id: id }).then(res => {
        if (res.result === 1) {
          var d = res.data
        }
        this.loading = false
      })

      this.getListData(this.listQuery)
    },
    getListData(listQuery) {
      console.log('getListData')
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: listQuery })
      manageStockGetStockList(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
          this.listTotal = d.total
        }
        this.loading = false
      })
    },
    handleFilter() {
      this.listQuery.page = 1
      this.getListData()
    }
  }
}
</script>

<style lang="scss" scoped>

#machine_stock{
  padding: 20px;
  padding-top: 0px;
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

          .el-button{
     font-size: 12px ;
     padding: 6px 12px;
   }
  }
}
</style>
