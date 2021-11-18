<template>
  <div id="refund_query">

    <div v-show="isShowList">
      <div class="filter-container">

        <el-form ref="form" label-width="120px" class="query-box">
          <el-form-item label="退款单号">
            <el-input v-model="listQuery.payrefundId" clearable style="max-width: 300px;" />
          </el-form-item>
          <el-form-item label="商户单号">
            <el-input v-model="listQuery.payTransId" clearable style="max-width: 300px;" />
          </el-form-item>
          <el-form-item label="订单号">
            <el-input v-model="listQuery.orderId" clearable style="max-width: 300px;" />
          </el-form-item>
          <el-form-item label="交易单号">
            <el-input v-model="listQuery.payPartnerPayTransId" clearable style="max-width: 300px;" />
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
        <el-table-column label="退款单号" fixed align="left" width="220">
          <template slot-scope="scope">
            <span>{{ scope.row.id }}</span>
          </template>
        </el-table-column>
        <el-table-column label="店铺" align="left" width="150">
          <template slot-scope="scope">
            <span>{{ scope.row.storeName }}</span>
          </template>
        </el-table-column>
        <el-table-column label="订单号" align="left" width="220">
          <template slot-scope="scope">
            <span>{{ scope.row.orderId }}</span>
          </template>
        </el-table-column>
        <el-table-column label="商户单号" align="left" width="220">
          <template slot-scope="scope">
            <span>{{ scope.row.payTransId }}</span>
          </template>
        </el-table-column>
        <el-table-column label="交易单号" align="left" width="260">
          <template slot-scope="scope">
            <span>{{ scope.row.payPartnerPayTransId }}</span>
          </template>
        </el-table-column>
        <el-table-column label="退款金额" align="left" width="100">
          <template slot-scope="scope">
            <span>{{ scope.row.applyAmount }}</span>
          </template>
        </el-table-column>
        <el-table-column label="退款方式" align="left" width="100">
          <template slot-scope="scope">
            <span>{{ scope.row.applyMethod.text }}</span>
          </template>
        </el-table-column>
        <el-table-column label="申请时间" align="left" width="160">
          <template slot-scope="scope">
            <span>{{ scope.row.applyTime }}</span>
          </template>
        </el-table-column>
        <el-table-column label="原因" align="left" width="160">
          <template slot-scope="scope">
            <span>{{ scope.row.applyRemark }}</span>
          </template>
        </el-table-column>
        <el-table-column label="备注" align="left" width="160">
          <template slot-scope="scope">
            <span>{{ scope.row.refundedRemark }}</span>
          </template>
        </el-table-column>
        <el-table-column label="状态" align="center" fixed="right" width="80">
          <template slot-scope="scope">
            <el-tag :type="getStatusColor(scope.row.status.value)">{{ scope.row.status.text }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" align="center" fixed="right" width="80" class-name="small-padding fixed-width">
          <template slot-scope="{row}">
            <el-button type="text" size="mini" @click="onDialogOpenByDetails(row)">
              查看
            </el-button>
          </template>
        </el-table-column>
      </el-table>
      <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="onGetList" />
    </div>
    <div v-show="isShowDetials">
      <div v-loading="loadingByDetials">
        <el-page-header class="my-page-header" content="退款查询" @back="onGoBack" />
        <div class="row-title clearfix">
          <div class="pull-left"> <h5>基本信息</h5>
          </div>
          <div class="pull-right" />
        </div>
        <el-form class="form-container" style="display:flex;max-width:800px;">
          <el-col :span="24">

            <div class="postInfo-container">
              <el-row>
                <el-col :span="24">
                  <el-form-item label-width="110px" label="订单编号:" class="postInfo-container-item">
                    {{ details.order.id }}
                  </el-form-item>
                </el-col>
              </el-row>
              <el-row>
                <el-col :span="24">
                  <el-form-item label-width="110px" label="店铺名称:" class="postInfo-container-item">
                    {{ details.order.storeName }}
                  </el-form-item>
                </el-col>
              </el-row>
              <el-row>
                <el-col :span="12">
                  <el-form-item label-width="110px" label="下单用户:" class="postInfo-container-item">
                    {{ details.order.clientUserName }}
                  </el-form-item>
                </el-col>

                <el-col :span="12">
                  <el-form-item label-width="110px" label="下单方式:" class="postInfo-container-item">
                    {{ details.order.sourceName }}
                  </el-form-item>
                </el-col>
              </el-row>
              <el-row>

                <el-col :span="12">
                  <el-form-item label-width="110px" label="下单时间:" class="postInfo-container-item">
                    {{ details.order.submittedTime }}
                  </el-form-item>
                </el-col>

                <el-col :span="12">
                  <el-form-item label-width="110px" label="原金额:" class="postInfo-container-item">
                    {{ details.order.originalAmount }}
                  </el-form-item>
                </el-col>

              </el-row>

              <el-row>
                <el-col :span="12">
                  <el-form-item label-width="110px" label="优惠金额:" class="postInfo-container-item">
                    {{ details.order.discountAmount }}
                  </el-form-item>
                </el-col>

                <el-col :span="12">
                  <el-form-item label-width="110px" label="支付金额:" class="postInfo-container-item">
                    {{ details.order.chargeAmount }}
                  </el-form-item>
                </el-col>
              </el-row>

              <el-row>
                <el-col :span="12">
                  <el-form-item label-width="110px" label="状态:" class="postInfo-container-item">
                    {{ details.order.status.text }}
                  </el-form-item>
                </el-col>
                <el-col :span="12">
                  <el-form-item label-width="110px" label="设备编码:" class="postInfo-container-item">
                    {{ details.order.deviceCumCode }}
                  </el-form-item>
                </el-col>
              </el-row>
              <el-row>
                <el-col :span="12">
                  <el-form-item label-width="110px" label="支付方式:" class="postInfo-container-item">
                    {{ details.order.payWay.text }}
                  </el-form-item>
                </el-col>
                <el-col :span="12" />
              </el-row>
            </div>
          </el-col>
        </el-form>
        <div class="row-title clearfix">
          <div class="pull-left"> <h5>商品信息</h5>
          </div>
        </div>

        <div style="font-size:14px">

          <el-table
            :key="listKey"
            :data="details.order.skus"
            fit
            highlight-current-row
            style="width: 800px"
          >
            <el-table-column label="" align="left" width="100">
              <template slot-scope="scope">
                <img :src="scope.row.mainImgUrl" style="width:50px;height:50px;">
              </template>
            </el-table-column>
            <el-table-column label="商品名称" align="left" width="100">
              <template slot-scope="scope">
                <span>{{ scope.row.name }}</span>
              </template>
            </el-table-column>
            <el-table-column label="交易数量" align="left" width="100">
              <template slot-scope="scope">
                <span>{{ scope.row.quantity }}</span>
              </template>
            </el-table-column>
            <el-table-column label="交易金额" align="left" width="100">
              <template slot-scope="scope">
                <span>{{ scope.row.chargeAmount }}</span>
              </template>
            </el-table-column>
            <el-table-column label="取货状态" align="left" width="100">
              <template slot-scope="scope">
                <span>{{ scope.row.pickupStatus.text }}</span>
              </template>
            </el-table-column>
            <el-table-column label="已退数量" align="left" width="100">
              <template slot-scope="scope">
                <span>{{ scope.row.refundedQuantity }}</span>
              </template>
            </el-table-column>
            <el-table-column label="已退金额" align="left" width="100">
              <template slot-scope="scope">
                <span>{{ scope.row.refundedAmount }}</span>
              </template>
            </el-table-column>
          </el-table>

        </div>

        <div class="row-title clearfix">
          <div class="pull-left"> <h5>退款申请</h5>
          </div>
        </div>

        <el-form label-width="110px" style="max-width:800px;">
          <el-form-item label="退款单号:">
            {{ details.payRefundId }}
          </el-form-item>
          <el-form-item label="订单号:">
            {{ details.orderId }}
          </el-form-item>
          <el-form-item label="商户单号:">
            {{ details.payTransId }}
          </el-form-item>
          <el-form-item label="交易单号:">
            {{ details.payPartnerPayTransId }}
          </el-form-item>
          <el-form-item label="退款商品:">

            <el-table
              :key="listKey"
              :data="details.skus"
              fit
              highlight-current-row
              style="width: 690px"
            >
              <el-table-column label="" align="left" width="300">
                <template slot-scope="scope">
                  <img :src="scope.row.mainImgUrl" style="width:50px;height:50px;">
                </template>
              </el-table-column>
              <el-table-column label="商品名称" align="left" width="190">
                <template slot-scope="scope">
                  <span>{{ scope.row.name }}</span>
                </template>
              </el-table-column>
              <el-table-column label="数量" align="left" width="100">
                <template slot-scope="scope">
                  <span>{{ scope.row.refundedQuantity }}</span>
                </template>
              </el-table-column>
              <el-table-column label="金额" align="left" width="100">
                <template slot-scope="scope">
                  <span>{{ scope.row.refundedAmount }}</span>
                </template>
              </el-table-column>
            </el-table>

          </el-form-item>
          <el-form-item label="申请金额:">
            {{ details.applyAmount }}
          </el-form-item>
          <el-form-item label="申请金额:">
            {{ details.applyAmount }}
          </el-form-item>
          <el-form-item label="退款方式:">
            {{ details.applyMethod.text }}
          </el-form-item>
          <el-form-item label="原因:">
            {{ details.applyRemark }}
          </el-form-item>
          <el-form-item label="状态:">
            <el-tag :type="getStatusColor(details.status.value)">{{ details.status.text }}</el-tag>
          </el-form-item>
          <el-form-item label="处理备注:">
            {{ details.handleRemark }}
          </el-form-item>
          <el-form-item label="退款备注:">
            {{ details.refundedRemark }}
          </el-form-item>
          <el-form-item label="" prop="">
            <el-button @click="onGoBack">
              返回
            </el-button>
          </el-form-item>
        </el-form>

      </div>

    </div>

  </div>
</template>

<script>
import { getList, getDetails } from '@/api/payrefund'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination

export default {
  name: 'PayTransRefundQuery',
  components: { Pagination },
  data() {
    return {
      isShowDetials: false,
      isShowList: true,
      loadingByDetials: false,
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
        payPartnerPayTransId: undefined
      },
      details: {
        applyMethod: { text: '' },
        skus: [],
        status: { text: '', value: 0 },
        order: {
          id: '',
          sn: '',
          storeName: '',
          clientUserName: '',
          sourceName: '',
          quantity: '',
          originalAmount: '',
          discountAmount: '',
          chargeAmount: '',
          submittedTime: '',
          status: { text: '' },
          exHandleRemark: '',
          isRunning: false,
          skus: [],
          payWay: { text: '' },
          isTimeoutPayed: false
        }
      },
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
    getStatusColor(status) {
      switch (status) {
        case 1:
          return ''
        case 2:
          return ''
        case 3:
          return 'success'
        case 4:
        case 5:
          return 'danger'
      }
      return ''
    },
    onDialogOpenByDetails(row) {
      this.isShowDetials = true
      this.isShowList = false
      this.loadingByDetials = true
      getDetails({ payRefundId: row.id }).then(res => {
        if (res.result === 1) {
          this.details = res.data
        }
        this.loadingByDetials = false
      })
    },
    onGoBack() {
      this.isShowList = true
      this.isShowDetials = false
    }
  }
}
</script>
