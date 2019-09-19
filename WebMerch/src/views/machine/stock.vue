<template>
  <div id="machine_stock">
    <div class="cur-machine">
      <span class="title">当前机器:</span><span class="name">{{ curMachine.name }}</span>

      <el-dropdown trigger="click" @command="handleChangeMachine">
        <span class="el-dropdown-link">
          切换<i class="el-icon-arrow-down el-icon--right" />
        </span>
        <el-dropdown-menu slot="dropdown">
          <el-dropdown-item v-for="machine in machines" :key="machine.id" :command="machine.id"> {{ machine.name }}</el-dropdown-item>
        </el-dropdown-menu>
      </el-dropdown>
    </div>

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
import { initStock, getStockList } from '@/api/machine'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination
import { getUrlParam } from '@/utils/commonUtil'
export default {
  name: 'StoreList',
  components: { Pagination },
  data() {
    return {
      curMachine: undefined,
      machines: [],
      loading: false,
      listTotal: 0,
      listQuery: {
        page: 1,
        limit: 10,
        name: undefined,
        machineId: undefined
      },
      listData: [],
      activeNames: ''
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
      initStock({ id: id }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.curMachine = d.curMachine
          this.machines = d.machines
        }
        this.loading = false
      })

      this.getListData(this.listQuery)
    },
    getListData(listQuery) {
      console.log('getListData')
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: listQuery })
      getStockList(this.listQuery).then(res => {
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
    },
    handleChangeMachine(command) {
      this.$router.push({
        path: '/machine/stock?id=' + command
      })
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

  .cur-machine{
  font-size: 14px;
  line-height: 60px;
   .title{
    color: #5e6d82;
   }
   .name{
    padding: 0 10px;
   }
  }
}
</style>
