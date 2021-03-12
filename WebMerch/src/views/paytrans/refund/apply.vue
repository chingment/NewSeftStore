<template>
  <div id="refund_apply">

    <div v-show="isSearch">
      <el-form ref="form" label-width="120px">
        <el-form-item label="交易号">
          <el-input v-model="listQuery.payTransId" clearable style="max-width: 300px;" @keyup.enter.native="handleSearch" />
        </el-form-item>
        <el-form-item label="订单号">
          <el-input v-model="listQuery.orderId" clearable style="max-width: 300px;" @keyup.enter.native="handleSearch" />
        </el-form-item>
        <el-form-item label="支付商交易号">
          <el-input v-model="listQuery.payPartnerPayTransId" clearable style="max-width: 300px;" @keyup.enter.native="handleSearch" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查询交易</el-button>
        </el-form-item>
      </el-form>
      <el-table
        v-loading="loading"
        :data="listData"
        fit
        highlight-current-row
        style="width: 100%;"
      >
        <el-table-column label="订单号" prop="id" align="left" :width="isDesktop==true?220:80">
          <template slot-scope="scope">
            <span>{{ scope.row.id }}</span>
          </template>
        </el-table-column>
        <el-table-column label="交易号" prop="payTransId" align="left" :width="isDesktop==true?220:80">
          <template slot-scope="scope">
            <span>{{ scope.row.payTransId }}</span>
          </template>
        </el-table-column>
        <el-table-column v-if="isDesktop" label="店铺" prop="storeName" align="left" min-width="10%">
          <template slot-scope="scope">
            <span>{{ scope.row.storeName }}</span>
          </template>
        </el-table-column>
        <el-table-column v-if="isDesktop" label="下单用户" prop="clientUserName" align="left" min-width="10%">
          <template slot-scope="scope">
            <span>{{ scope.row.clientUserName }}</span>
          </template>
        </el-table-column>
        <el-table-column v-if="isDesktop" label="下单方式" prop="sourceName" align="left" min-width="10%">
          <template slot-scope="scope">
            <span>{{ scope.row.sourceName }}</span>
          </template>
        </el-table-column>
        <el-table-column v-if="isDesktop" label="取货方式" prop="sourceName" align="left" min-width="10%">
          <template slot-scope="scope">
            <span>{{ scope.row.receiveModeName }}</span>
          </template>
        </el-table-column>
        <el-table-column v-if="isDesktop" label="触发状态" prop="sourceName" align="left" min-width="10%">
          <template slot-scope="scope">
            <span>{{ scope.row.pickupTrgStatus.text }}</span>
          </template>
        </el-table-column>
        <el-table-column label="金额" prop="chargeAmount" align="left" min-width="10%">
          <template slot-scope="scope">
            <span>{{ scope.row.chargeAmount }}</span>
          </template>
        </el-table-column>
        <el-table-column label="状态" prop="status" align="left" min-width="10%">
          <template slot-scope="scope">
            <span>{{ scope.row.status.text }}</span>
          </template>
        </el-table-column>
        <el-table-column v-if="isDesktop" label="下单时间" prop="submittedTime" align="left" min-width="10%">
          <template slot-scope="scope">
            <span>{{ scope.row.submittedTime }}</span>
          </template>
        </el-table-column>
        <el-table-column label="操作" fixed="right" align="center" width="180" class-name="small-padding fixed-width">
          <template slot-scope="{row}">
            <el-button type="text" size="mini" @click="dialogOpenByRefundApply(row)">
              退款
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </div>

    <div v-show="!isSearch">
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
                  <el-form-item label-width="110px" label="订单编号:" class="postInfo-container-item">
                    {{ details.id }}
                  </el-form-item>
                </el-col>
              </el-row>
              <el-row>
                <el-col :span="24">
                  <el-form-item label-width="110px" label="店铺名称:" class="postInfo-container-item">
                    {{ details.storeName }}
                  </el-form-item>
                </el-col>
              </el-row>
              <el-row>
                <el-col :span="12">
                  <el-form-item label-width="110px" label="下单用户:" class="postInfo-container-item">
                    {{ details.clientUserName }}
                  </el-form-item>
                </el-col>

                <el-col :span="12">
                  <el-form-item label-width="110px" label="下单方式:" class="postInfo-container-item">
                    {{ details.sourceName }}
                  </el-form-item>
                </el-col>
              </el-row>
              <el-row>

                <el-col :span="12">
                  <el-form-item label-width="110px" label="下单时间:" class="postInfo-container-item">
                    {{ details.submittedTime }}
                  </el-form-item>
                </el-col>

                <el-col :span="12">
                  <el-form-item label-width="110px" label="原金额:" class="postInfo-container-item">
                    {{ details.originalAmount }}
                  </el-form-item>
                </el-col>

              </el-row>

              <el-row>
                <el-col :span="12">
                  <el-form-item label-width="110px" label="优惠金额:" class="postInfo-container-item">
                    {{ details.discountAmount }}
                  </el-form-item>
                </el-col>

                <el-col :span="12">
                  <el-form-item label-width="110px" label="支付金额:" class="postInfo-container-item">
                    {{ details.chargeAmount }}
                  </el-form-item>
                </el-col>
              </el-row>

              <el-row>
                <el-col :span="12">
                  <el-form-item label-width="110px" label="状态:" class="postInfo-container-item">
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

        <el-form ref="formByApply" :model="formByApply" :rules="rulesByApply" label-width="110px" style="max-width:800px;">
          <el-form-item label="退款提示:">

            <span style="line-height:30px">已退款金额：<span class="refundedAmount">{{ details.refundedAmount }}</span>，正在申请退款金额：<span class="refundingAmount">{{ details.refundingAmount }}</span>，可申请退款金额：<span class="refundableAmount">{{ details.refundableAmount }}</span></span>

          </el-form-item>
          <el-form-item label="退款方式:" prop="method">

            <el-radio-group v-model="formByApply.method">
              <el-radio label="1">原路退回（系统自动处理，退款到用户支付账号）</el-radio>
              <el-radio label="2">线下退回（人工审核处理，线下人工退回）</el-radio>
            </el-radio-group>

          </el-form-item>
          <el-form-item label="退款金额:" prop="amount">
            <el-input v-model="formByApply.amount" clearable style="width:160px">
              <template slot="prepend">￥</template>
            </el-input>
          </el-form-item>
          <el-form-item label="原因:" prop="remark">
            <el-input v-model="formByApply.remark" clearable />
          </el-form-item>
          <el-form-item label="" prop="">
            <el-button type="primary" @click="_apply()">
              确认
            </el-button>
            <el-button @click="isSearch = true">
              返回
            </el-button>
          </el-form-item>
        </el-form>

      </div>

    </div>
  </div>
</template>

<script>
import { searchOrder, getOrderDetails, apply } from '@/api/payrefund'
import { MessageBox } from 'element-ui'
import { getUrlParam, isEmpty } from '@/utils/commonUtil'
export default {
  name: 'PayTransRefundApply',
  data() {
    return {
      loading: false,
      listData: [],
      listQuery: {
        page: 1,
        limit: 10,
        payTransId: undefined,
        orderId: undefined,
        payPartnerPayTransId: undefined
      },
      loadingByRefundApply: false,
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
      formByApply: {
        orderId: '',
        method: '',
        remark: '',
        amount: 0
      },
      rulesByApply: {
        method: [{ required: true, max: 200, message: '请选择退款方式', trigger: 'change' }],
        remark: [{ required: true, min: 1, max: 200, message: '原因不能为空', trigger: 'change' }]
      },
      isSearch: true,
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      var payTransId = getUrlParam('payTransId')
      if (payTransId != null) {
        this.listQuery.payTransId = payTransId
        this.handleSearch()
      }
    },
    handleSearch() {
      if (isEmpty(this.listQuery.payTransId) && isEmpty(this.listQuery.orderId) && isEmpty(this.listQuery.payPartnerPayTransId)) {
        this.$message('找少输入一个搜索条件')
        return
      }

      this.listQuery.page = 1
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      searchOrder(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
          this.listTotal = d.total
          if (d.total === 0) {
            this.$message('查询不到有效的交易记录')
          }
        }
        this.loading = false
      })
    },
    dialogOpenByRefundApply(row) {
      if (row.exStatus.value === 2) {
        this.$message('该订单存在异常未有处理，请到订单中处理')
        return
      }

      this.loadingByRefundApply = true
      this.formByApply.orderId = row.id
      this.formByApply.remark = ''
      this.formByApply.method = ''
      this.formByApply.amount = 0

      getOrderDetails({ orderId: row.id }).then(res => {
        if (res.result === 1) {
          this.details = res.data
        }
        this.loadingByRefundApply = false
        this.isSearch = false
      })
    },
    _apply() {
      var _this = this

      this.$refs['formByApply'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要提交退款？', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            apply(_this.formByApply).then(res => {
              this.$message(res.message)
              if (res.result === 1) {
                var d = res.data
                this.$router.push({
                  path: '/payRefund/query?payRefundId=' + d.payRefundId
                })
              }
            })
          }).catch(() => {
          })
        }
      })
    }

  }
}
</script>

<style  lang="scss"  scoped>

#refund_apply{

}
</style>

