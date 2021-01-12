<template>
  <div id="user_list" class="app-container">
    <div class="filter-container">

      <el-row :gutter="16">
        <el-col :span="8" :xs="24" style="margin-bottom:20px">
          <el-input v-model="listQuery.name" clearable style="width: 100%" placeholder="门店" class="filter-item" @keyup.enter.native="handleFilter" @clear="handleFilter" />
        </el-col>
        <el-col :span="4" :xs="24" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter">
            查询
          </el-button>
        </el-col>
      </el-row>

    </div>
    <el-table
      :key="listKey"
      v-loading="loading"
      :data="listData"
      fit
      highlight-current-row
      style="width: 100%;"
    >
      <el-table-column v-if="isDesktop" label="序号" prop="id" align="left" width="80">
        <template slot-scope="scope">
          <span>{{ scope.$index+1 }} </span>
        </template>
      </el-table-column>
      <el-table-column label="门店" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column label="地址" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.address }}</span>
        </template>
      </el-table-column>  
      <el-table-column label="操作" align="center" width="120" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
    
      <template v-if="opcode==='select'" >
         <el-button v-if="row.isCanSelect" type="primary" size="mini" @click="handleSelect(row)">
            选择
          </el-button>

          <span v-else>{{row.opTips}}</span>

      </template>
        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />
  </div>
</template>

<script>
import { getList } from '@/api/shop'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination

export default {
  name: 'ShopList',
  props: {
    opcode: {
      type: String,
      default: 'list'
    },
    storeid: {
      type: String,
      default: ''
    }
  },
  components: { Pagination },
  data() {
    return {
      loading: false,
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        page: 1,
        limit: 10,
        opCode:'',
        storeId:'',
        name: undefined
      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }

    this.listQuery.opCode=this.opcode
    this.listQuery.storeId=this.storeid
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
          this.listTotal = d.total
        }
        this.loading = false
      })
    },
    handleFilter() {
      this.listQuery.page = 1
      this.getListData()
    },
    handleOperate(command) {
      // this.$message('click on item ' + command)
      var arr = command.split('-')
      if (arr[0] === 'payRefund') {
        this.$router.push({
          path: '/payrefund/apply?payTransId=' + arr[1]
        })
      }
    }
  }
}
</script>
