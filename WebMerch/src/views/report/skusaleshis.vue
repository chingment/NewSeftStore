<template>
  <div id="report_list">

    <div class="filter-container">
      <div />
      <el-form ref="form" label-width="120px" class="query-box">
        <el-form-item label="店铺" style="max-width: 600px;">
          <el-select v-model="listQuery.storeIds" multiple placeholder="选择" style="width: 100%">
            <el-option
              v-for="item in optionsStores"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="交易日期" style="max-width: 400px;">
          <el-date-picker
            v-model="listQuery.tradeDateTimeArea"
            type="daterange"
            range-separator="-"
            value-format="yyyy-MM-dd"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="取货状态">
          <el-radio-group v-model="listQuery.pickupStatus">
            <el-radio-button v-for="item in options_PickupStatus" :key="item.value" :label="item.value">{{ item.label }}</el-radio-button>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="提货方式">
          <el-radio-group v-model="listQuery.receiveMode">
            <el-radio-button v-for="item in options_ReceiveModes" :key="item.value" :label="item.value">{{ item.label }}</el-radio-button>
          </el-radio-group>
        </el-form-item>

        <el-form-item>
         <el-button type="primary" icon="el-icon-search" @click="onFilter">查询</el-button> 
          <el-button :loading="downloadLoading" type="primary" style="margin-left:10px" icon="el-icon-document" @click="onDownload">
            导出
          </el-button>
        </el-form-item>
      </el-form>
    </div>
    <el-table
      :key="listKey"
      v-loading="loading"
      border
      :data="listData"
      fit
      highlight-current-row
    >
      <el-table-column label="店铺" align="left" width="100">
        <template slot-scope="scope">
          <span>{{ scope.row.storeName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="门店" align="left" width="200">
        <template slot-scope="scope">
          <span>{{ scope.row.shopName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="设备编码" align="left" width="150">
        <template slot-scope="scope">
          <span>{{ scope.row.deviceCode }}</span>
        </template>
      </el-table-column>
      <el-table-column label="提货方式" align="left" width="100">
        <template slot-scope="scope">
          <span>{{ scope.row.receiveMode }}</span>
        </template>
      </el-table-column>
      <el-table-column label="订单号" align="left" width="220">
        <template slot-scope="scope">
          <span>{{ scope.row.orderId }}</span>
        </template>
      </el-table-column>
      <el-table-column label="商品名称" align="left" width="200">
        <template slot-scope="scope">
          <span>{{ scope.row.skuName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="商品编码" align="left" width="100">
        <template slot-scope="scope">
          <span>{{ scope.row.skuCumCode }}</span>
        </template>
      </el-table-column>
      <el-table-column label="商品规格" align="left" width="200">
        <template slot-scope="scope">
          <span>{{ scope.row.skuSpecDes }}</span>
        </template>
      </el-table-column>
      <el-table-column label="支付方式" align="left" width="100">
        <template slot-scope="scope">
          <span>{{ scope.row.payWay }}</span>
        </template>
      </el-table-column>
      <el-table-column label="支付状态" align="left" width="100">
        <template slot-scope="scope">
          <span>{{ scope.row.payStatus }}</span>
        </template>
      </el-table-column>
       <el-table-column label="支付时间" align="left" width="160">
        <template slot-scope="scope">
          <span>{{ scope.row.payedTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="取货状态" align="left" width="100">
        <template slot-scope="scope">
          <span>{{ scope.row.pickupStatus }}</span>
        </template>
      </el-table-column>
      <el-table-column label="单价" align="left" width="100">
        <template slot-scope="scope">
          <span>{{ scope.row.salePrice }}</span>
        </template>
      </el-table-column>
      <el-table-column label="数量" align="left"  width="100">
        <template slot-scope="scope">
          <span>{{ scope.row.quantity }}</span>
        </template>
      </el-table-column>
      <el-table-column label="支付金额" align="left"  width="100">
        <template slot-scope="scope">
          <span>{{ scope.row.chargeAmount }}</span>
        </template>
      </el-table-column>
      <el-table-column label="退款数量" align="left" width="100">
        <template slot-scope="scope">
          <span>{{ scope.row.refundedQuantity }}</span>
        </template>
      </el-table-column>
      <el-table-column label="退款金额" align="left"  width="100">
        <template slot-scope="scope">
          <span>{{ scope.row.refundedAmount }}</span>
        </template>
      </el-table-column>
      <el-table-column label="结算数量" align="left" width="100">
        <template slot-scope="scope">
          <span>{{ scope.row.tradeQuantity }}</span>
        </template>
      </el-table-column>
      <el-table-column label="结算金额" align="left"  width="100">
        <template slot-scope="scope">
          <span>{{ scope.row.tradeAmount }}</span>
        </template>
      </el-table-column>
    </el-table>
    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="onGetList" />

    <el-alert
      title="提示：以商品单位维度来统计销售报表， 不统计测试模式"
      type="remark-gray"
      :closable="false"
    />

  </div>
</template>

<script>
import axios from 'axios'
import fileDownload from 'js-file-download'
import { skuSalesHisInit, skuSalesHisGet, checkRightExport } from '@/api/report'
import { parseTime } from '@/utils'
import { getToken } from '@/utils/auth'
import Pagination from '@/components/Pagination'
export default {
  name: 'ReportSkuSalesDateHis',
  components: { Pagination },
  props: {
  },
  data() {
    return {
      loading: false,
      downloadLoading: false,
      filename: '商品销售报表',
      autoWidth: true,
      bookType: 'xlsx',
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        page: 1,
        limit: 10,
        storeIds: [],
        tradeDateTimeArea: [],
        pickupStatus: '',
        receiveMode: '0'
      },
      options_PickupStatus: [{
        value: '',
        label: '全部'
      }, {
        value: '1',
        label: '待取货'
      }, {
        value: '2',
        label: '未取货'
      }, {
        value: '3',
        label: '已取货'
      }],
      options_ReceiveModes: [{
        value: '0',
        label: '全部'
      }, {
        value: '4',
        label: '设备自提'
      }, {
        value: '2',
        label: '店铺自取'
      }, {
        value: '1',
        label: '配送商品'
      }],
      optionsStores: [],
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }
    this.init()
  },
  methods: {
    init() {
      this.loading=true
      skuSalesHisInit().then(res => {
        if (res.result === 1) {
          var d = res.data
          this.optionsStores = d.optionsStores
        }
        this.loading = false
      })
    },
    onGetList() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      skuSalesHisGet(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
          this.listTotal = d.total
        } else {
          this.$message({
            message: res.message,
            type: 'error'
          })
        }
        this.loading = false
      })
    },
    onFilter() {
      if (this.listQuery.tradeDateTimeArea.length === 0) {
        this.$message('请选择日期范围')
        return
      }
      this.onGetList()
    },
    onDownload() {
      if (this.listQuery.tradeDateTimeArea.length === 0) {
        this.$message('请选择日期范围')
        return
      }

      var filename = this.filename

      if (this.listQuery.tradeDateTimeArea[0] === this.listQuery.tradeDateTimeArea[1]) {
        filename = filename + '(' + this.listQuery.tradeDateTimeArea[0] + ')'
      } else {
        filename = filename + '(' + this.listQuery.tradeDateTimeArea[0] + '~' + this.listQuery.tradeDateTimeArea[1] + ')'
      }

     this.downloadLoading = true
      checkRightExport({ fileName: filename }).then(res => {
        if (res.result === 1) {
          const data = this.listQuery
          axios({
            url: `http://api.merch.17fanju.com/api/report/SkuSalesHisExport`,
            method: 'post',
            data,
            'responseType': 'arraybuffer',
            headers: {
              'X-Token': getToken()
            }
          }).then(res => {
            fileDownload(res.data, filename + '.xls')
                        this.downloadLoading = false
          })
        } else {
          this.downloadLoading = false
          this.$message({
            message: res.message,
            type: 'error'
          })
        }
      })
    }
  }
}
</script>
<style lang="scss" scoped>

.table-skus{
  margin-left: 20px;
  td{
    border: 0px  !important;
  }
}
</style>
