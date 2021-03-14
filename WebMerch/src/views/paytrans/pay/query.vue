<template>
  <div id="pay_list">
    <div class="filter-container">

      <el-form ref="form" label-width="120px">
        <el-form-item label="交易号">
          <el-input v-model="listQuery.payTransId" clearable style="max-width: 300px;" @keyup.enter.native="handleFilter" />
        </el-form-item>
        <el-form-item label="订单号">
          <el-input v-model="listQuery.orderId" clearable style="max-width: 300px;" @keyup.enter.native="handleFilter" />
        </el-form-item>
        <el-form-item label="支付商交易号">
          <el-input v-model="listQuery.payPartnerPayTransId" clearable style="max-width: 300px;" @keyup.enter.native="handleFilter" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="el-icon-search" @click="handleFilter">查询</el-button>
        </el-form-item>
      </el-form>

    </div>
    <el-table
      :key="listKey"
      v-loading="loading"
      :data="listData"
      fit
      highlight-current-row
      style="width: 100%;"
    >
      <el-table-column label="交易号"   fixed align="left"  width="220">
        <template slot-scope="scope">
          <span>{{ scope.row.id }}</span>
        </template>
      </el-table-column>
      <el-table-column label="店铺" align="left"  width="150">
        <template slot-scope="scope">
          <span>{{ scope.row.storeName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="相关订单号" align="left"  width="150">
        <template slot-scope="scope">
          <span>{{ scope.row.orderIds }}</span>
        </template>
      </el-table-column>
      <el-table-column label="支付商" align="left" width="150">
        <template slot-scope="scope">
          <span>{{ scope.row.payPartner.text }}</span>
        </template>
      </el-table-column>
      <el-table-column label="支付商交易号" align="left"  width="150">
        <template slot-scope="scope">
          <span>{{ scope.row.payPartnerPayTransId }}</span>
        </template>
      </el-table-column>
      <el-table-column label="支付金额" align="left" width="150">
        <template slot-scope="scope">
          <span>{{ scope.row.chargeAmount }}</span>
        </template>
      </el-table-column>
      <el-table-column label="支付方式" align="left"  width="150">
        <template slot-scope="scope">
          <span>{{ scope.row.payWay.text }}</span>
        </template>
      </el-table-column>
      <el-table-column label="交易状态" align="left"  width="150">
        <template slot-scope="scope">
          <span>{{ scope.row.payStatus.text }}</span>
        </template>
      </el-table-column>
      <el-table-column label="备注" align="left"  width="150">
        <template slot-scope="scope">
          <span>{{ scope.row.description }}</span>
        </template>
      </el-table-column>
      <el-table-column label="应用程序" align="left"  width="150">
        <template slot-scope="scope">
          <span>{{ scope.row.appId }}</span>
        </template>
      </el-table-column>
      <el-table-column label="交易时间" align="left" width="150">
        <template slot-scope="scope">
          <span>{{ scope.row.submittedTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" width="120" fixed="right" class-name="small-padding fixed-width">
        <template slot-scope="scope">
          <el-dropdown @command="handleOperate">
            <el-button type="text" size="small">
              操作<i class="el-icon-arrow-down el-icon--right" />
            </el-button>
            <el-dropdown-menu slot="dropdown">
              <el-dropdown-item :command="'payRefund-'+scope.row.id">退款</el-dropdown-item>

            </el-dropdown-menu>
          </el-dropdown>

        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />
  </div>
</template>

<script>
import { getList } from '@/api/paytrans'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination

export default {
  name: 'PayTransPayQuery',
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
        userName: undefined
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
    handleOperate(command) {
      // this.$message('click on item ' + command)
      var arr = command.split('-')
      if (arr[0] === 'payRefund') {
        this.$router.push({
          path: '/paytrans/refund/apply?payTransId=' + arr[1]
        })
      }
    }
  }
}
</script>
