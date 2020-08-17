<template>
  <div id="user_list" class="app-container">

    <div v-show="!isHandle">
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
        <el-table-column label="操作" align="center" width="100" class-name="small-padding fixed-width">
          <template slot-scope="{row}">
            <el-button v-if="row.method.value==2" type="primary" size="mini" @click="dialogOpenByRefundHandle(row)">
              人工处理
            </el-button>
            <el-tag v-if="row.method.value==1" type="warning">自动处理</el-tag>
          </template>
        </el-table-column>
      </el-table>
      <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />

    </div>

    <div v-show="isHandle">
      <div v-loading="loadingByRefundApply">
        <div class="row-title clearfix">
          <div class="pull-left"> <h5>基本信息</h5>
          </div>
          <div class="pull-right">
            <el-button icon="el-icon-refresh" circle @click="refreshDetails(details.id)" />
          </div>
        </div>
        <el-form class="form-container" style="display:flex;max-width:800px;">
          <el-col :span="24">

            <div class="postInfo-container">
              <el-row>
                <el-col :span="24">
                  <el-form-item label-width="80px" label="订单编号:" class="postInfo-container-item">
                    {{ details.id }}
                  </el-form-item>
                </el-col>
              </el-row>
              <el-row>
                <el-col :span="24">
                  <el-form-item label-width="80px" label="店铺名称:" class="postInfo-container-item">
                    {{ details.storeName }}
                  </el-form-item>
                </el-col>
              </el-row>
              <el-row>
                <el-col :span="12">
                  <el-form-item label-width="80px" label="下单用户:" class="postInfo-container-item">
                    {{ details.clientUserName }}
                  </el-form-item>
                </el-col>

                <el-col :span="12">
                  <el-form-item label-width="80px" label="下单方式:" class="postInfo-container-item">
                    {{ details.sourceName }}
                  </el-form-item>
                </el-col>
              </el-row>
              <el-row>

                <el-col :span="12">
                  <el-form-item label-width="80px" label="下单时间:" class="postInfo-container-item">
                    {{ details.submittedTime }}
                  </el-form-item>
                </el-col>

                <el-col :span="12">
                  <el-form-item label-width="80px" label="原金额:" class="postInfo-container-item">
                    {{ details.originalAmount }}
                  </el-form-item>
                </el-col>

              </el-row>

              <el-row>
                <el-col :span="12">
                  <el-form-item label-width="80px" label="优惠金额:" class="postInfo-container-item">
                    {{ details.discountAmount }}
                  </el-form-item>
                </el-col>

                <el-col :span="12">
                  <el-form-item label-width="80px" label="支付金额:" class="postInfo-container-item">
                    {{ details.chargeAmount }}
                  </el-form-item>
                </el-col>
              </el-row>

              <el-row>
                <el-col :span="12">
                  <el-form-item label-width="80px" label="状态:" class="postInfo-container-item">
                    {{ details.status.text }}
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
        <div v-for="(receiveMode,index) in details.receiveModes" :key="index" style="font-size:14px">
          <div> <i class="el-icon-place" /><span> {{ receiveMode.name }} </span> <i class="el-icon-d-arrow-right" /> </div>
          <table class="table-skus" style="max-width:800px;table-layout:fixed;">
            <tr v-for="(pickupSku,sub_index) in receiveMode.items" :key="sub_index">
              <td style="width:10%">
                <img :src="pickupSku.mainImgUrl" style="width:50px;height:50px;">
              </td>
              <td style="width:20%">
                {{ pickupSku.name }}
              </td>
              <td style="width:20%">
                x {{ pickupSku.quantity }}
              </td>
              <td style="width:10%">
                {{ pickupSku.chargeAmount }}
              </td>
              <td style="width:30%;">
                {{ pickupSku.status.text }}
              </td>
            </tr>
          </table>
        </div>

        <div class="row-title clearfix">
          <div class="pull-left"> <h5>退款申请</h5>
          </div>
        </div>

        <el-form ref="form" v-loading="loading" :model="formByApply" :rules="rulesByApply" label-width="80px" style="max-width:800px;">
          <el-form-item label="退款提示">

            <span>已退款金额：<span>{{ details.refundedAmount }}</span>，正在申请退款金额：<span>{{ details.refundingAmount }}</span>，可申请退款金额：<span>{{ details.refundableAmount }}</span></span>

          </el-form-item>
          <el-form-item label="退款方式" prop="method">
            <span>{{ details.refundedAmount }}</span>
          </el-form-item>
          <el-form-item label="退款金额" prop="amount">
            <span>{{ details.refundedAmount }}</span>
          </el-form-item>
          <el-form-item label="原因" prop="reason">
            <span>{{ details.refundedAmount }}</span>
          </el-form-item>

        </el-form>

      </div>
      <div slot="footer" class="dialog-footer">
        <el-button type="primary" @click="_apply()">
          确认处理
        </el-button>
        <el-button @click="isHandle = false">
          返回
        </el-button>
      </div>
    </div>

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
      details: {
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
        details: undefined,
        isRunning: false
      },
      isHandle: false,
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
    dialogOpenByRefundHandle(row) {
      // this.loadingByRefundApply = true
      // this.formByApply.orderId = row.id
      // getOrderDetails({ orderId: row.id }).then(res => {
      //   if (res.result === 1) {
      //     this.details = res.data
      //   }
      //   this.loadingByRefundApply = false
      //   this.isSearch = false
      // })

      this.isHandle = true
    }
  }
}
</script>
