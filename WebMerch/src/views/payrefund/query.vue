<template>
  <div id="user_list" class="app-container">
    <div class="filter-container">

      <el-row :gutter="20">
        <el-col :span="4" :xs="24" style="margin-bottom:20px">
          <el-input v-model="listQuery.payrefundId" clearable style="width: 100%" placeholder="退款单号" class="filter-item" @keyup.enter.native="handleFilter" @clear="handleFilter" />
        </el-col>
        <el-col :span="4" :xs="24" style="margin-bottom:20px">
          <el-input v-model="listQuery.paytransId" clearable style="width: 100%" placeholder="交易号" class="filter-item" @keyup.enter.native="handleFilter" @clear="handleFilter" />
        </el-col>
        <el-col :span="4" :xs="24" style="margin-bottom:20px">
          <el-input v-model="listQuery.orderId" clearable style="width: 100%" placeholder="订单号" class="filter-item" @keyup.enter.native="handleFilter" @clear="handleFilter" />
        </el-col>
        <el-col :span="4" :xs="24" style="margin-bottom:20px">
          <el-input v-model="listQuery.payPartnerOrderId" clearable style="width: 100%" placeholder="支付商交易号" class="filter-item" @keyup.enter.native="handleFilter" @clear="handleFilter" />
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
      <el-table-column label="退款单号" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.id }}</span>
        </template>
      </el-table-column>
      <el-table-column label="店铺" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.storeName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="订单号" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.orderId }}</span>
        </template>
      </el-table-column>
      <el-table-column label="交易号" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.payTransId }}</span>
        </template>
      </el-table-column>
      <el-table-column label="支付商交易号" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.payPartnerOrderId }}</span>
        </template>
      </el-table-column>
      <el-table-column label="退款金额" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.amount }}</span>
        </template>
      </el-table-column>
      <el-table-column label="退款方式" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.method.text }}</span>
        </template>
      </el-table-column>
      <el-table-column label="申请时间" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.applyTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="状态" align="left" min-width="5%">
        <template slot-scope="scope">
          <span>{{ scope.row.status.text }}</span>
        </template>
      </el-table-column>
      <el-table-column label="原因" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.applyRemark }}</span>
        </template>
      </el-table-column>
      <!-- <el-table-column label="操作" align="center" width="80" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button type="primary" size="mini" @click="handleDetails(row)">
            查看
          </el-button>
        </template>
      </el-table-column> -->
    </el-table>

    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />
  </div>
</template>

<script>
import { getList } from '@/api/payrefund'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination

export default {
  name: 'PaytransList',
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
        payrefundId: undefined,
        paytransId: undefined,
        orderId: undefined,
        payPartnerOrderId: undefined
      },
      isDesktop: this.$store.getters.isDesktop
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
          this.listTotal = d.total
        }
        this.loading = false
      })
    },
    handleFilter() {
      this.listQuery.page = 1
      this.getListData()
    },
    handleDetails(row) {
      this.$router.push({
        path: '/clientuser/details?id=' + row.id
      })
    }
  }
}
</script>
