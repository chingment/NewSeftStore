<template>
  <div id="pay_query_list">
    <div class="filter-container">

      <el-form ref="form" label-width="120px" class="query-box">
        <el-form-item label="商户单号">
          <el-input v-model="listQuery.payTransId" clearable style="max-width: 300px;" />
        </el-form-item>
        <el-form-item label="订单号">
          <el-input v-model="listQuery.orderId" clearable style="max-width: 300px;" />
        </el-form-item>
        <el-form-item label="交易单号">
          <el-input v-model="listQuery.payPartnerPayTransId" clearable style="max-width: 300px;" />
        </el-form-item>
        <el-form-item label="交易时间">
          <el-date-picker
            v-model="listQuery.tradeDateArea"
            type="daterange"
            range-separator="-"
            value-format="yyyy-MM-dd"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
            style="max-width: 300px;"
          />
        </el-form-item>
        <el-form-item label="交易状态">
          <el-radio-group v-model="listQuery.payStatus">
            <el-radio-button v-for="item in payStatuss" :key="item.value" :label="item.value">{{ item.label }}</el-radio-button>
          </el-radio-group>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="el-icon-search" @click="onFilter">查询</el-button>
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
      <el-table-column label="商户单号" fixed align="left" width="220">
        <template slot-scope="scope">
          <span>{{ scope.row.id }}</span>
        </template>
      </el-table-column>
      <el-table-column label="店铺" align="left" width="150">
        <template slot-scope="scope">
          <span>{{ scope.row.storeName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="交易时间" align="left" width="160">
        <template slot-scope="scope">
          <span>{{ scope.row.submittedTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="相关订单号" align="left" width="220">
        <template slot-scope="scope">
          <span>{{ scope.row.orderIds }}</span>
        </template>
      </el-table-column>
      <el-table-column label="支付商" align="left" width="100">
        <template slot-scope="scope">
          <span>{{ scope.row.payPartner.text }}</span>
        </template>
      </el-table-column>
      <el-table-column label="交易单号" align="left" width="260">
        <template slot-scope="scope">
          <span>{{ scope.row.payPartnerPayTransId }}</span>
        </template>
      </el-table-column>
      <el-table-column label="支付金额" align="left" width="100">
        <template slot-scope="scope">
          <span>{{ scope.row.chargeAmount }}</span>
        </template>
      </el-table-column>
      <el-table-column label="支付方式" align="left" width="100">
        <template slot-scope="scope">
          <span>{{ scope.row.payWay.text }}</span>
        </template>
      </el-table-column>
      <el-table-column label="备注" align="left" width="150">
        <template slot-scope="scope">
          <span>{{ scope.row.description }}</span>
        </template>
      </el-table-column>
      <el-table-column label="交易状态" align="left" fixed="right" width="100">
        <template slot-scope="scope">
          <el-tag :type="getPayStatusColor(scope.row.payStatus.value)">{{ scope.row.payStatus.text }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" width="100" fixed="right" class-name="small-padding fixed-width">
        <template slot-scope="scope">
          <el-dropdown @command="onOperate">
            <el-button type="text" size="small">
              操作<i class="el-icon-arrow-down el-icon--right" />
            </el-button>

            <el-dropdown-menu slot="dropdown">
              <el-dropdown-item :command="'payRefund-'+scope.row.id" :disabled="scope.row.payStatus.value==3?false:true">退款</el-dropdown-item>

            </el-dropdown-menu>
          </el-dropdown>

        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="onGetList" />
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
        payStatus: '0',
        tradeDateArea: [],
        userName: undefined
      },
      payStatuss: [
        { value: '0', label: '全部' },
        { value: '1', label: '待支付' },
        { value: '2', label: '支付中' },
        { value: '3', label: '已支付' },
        { value: '4', label: '已取消' },
        { value: '5', label: '已超时' }
      ],
      isDesktop: this.$store.getters.isDesktop
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
          this.listTotal = d.total
        }
        this.loading = false
      })
    },
    onFilter() {
      this.listQuery.page = 1
      this.onGetList()
    },
    onOperate(command) {
      // this.$message('click on item ' + command)
      var arr = command.split('-')
      if (arr[0] === 'payRefund') {
        this.$router.push({
          path: '/paytrans/refund/apply?payTransId=' + arr[1]
        })
      }
    },
    getPayStatusColor(status) {
      switch (status) {
        case 1:
          return ''
        case 2:
          return ''
        case 3:
          return 'success'
        case 4:
        case 5:
          return ''
      }
      return ''
    }
  }
}
</script>
