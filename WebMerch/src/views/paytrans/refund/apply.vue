<template>
  <div id="refund_apply">

    <div v-show="isShowList">
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
        <el-form-item>
          <el-button type="primary" icon="el-icon-search" @click="onSearch">查询</el-button>
        </el-form-item>
      </el-form>
      <el-table
        v-loading="loading"
        :data="listData"
        fit
        highlight-current-row
        style="width: 100%;"
      >
        <el-table-column label="订单号" prop="id" align="left" width="220">
          <template slot-scope="scope">
            <span>{{ scope.row.id }}</span>
          </template>
        </el-table-column>
        <el-table-column label="商户单号" prop="payTransId" align="left" width="220">
          <template slot-scope="scope">
            <span>{{ scope.row.payTransId }}</span>
          </template>
        </el-table-column>
        <el-table-column label="店铺" prop="storeName" align="left" width="150">
          <template slot-scope="scope">
            <span>{{ scope.row.storeName }}</span>
          </template>
        </el-table-column>
        <el-table-column label="下单用户" prop="clientUserName" align="left" width="120">
          <template slot-scope="scope">
            <span>{{ scope.row.clientUserName }}</span>
          </template>
        </el-table-column>
        <el-table-column label="下单方式" prop="sourceName" align="left" width="100">
          <template slot-scope="scope">
            <span>{{ scope.row.sourceName }}</span>
          </template>
        </el-table-column>
        <el-table-column label="取货方式" prop="sourceName" align="left" width="100">
          <template slot-scope="scope">
            <span>{{ scope.row.receiveModeName }}</span>
          </template>
        </el-table-column>
        <el-table-column label="触发状态" prop="sourceName" align="left" width="100">
          <template slot-scope="scope">
            <span>{{ scope.row.pickupTrgStatus.text }}</span>
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
        <el-table-column label="状态" prop="status" align="left" width="100">
          <template slot-scope="scope">
            <span>{{ scope.row.status.text }}</span>
          </template>
        </el-table-column>
        <el-table-column label="下单时间" prop="submittedTime" align="left" width="160">
          <template slot-scope="scope">
            <span>{{ scope.row.submittedTime }}</span>
          </template>
        </el-table-column>
        <el-table-column label="操作" fixed="right" align="center" width="180" class-name="small-padding fixed-width">
          <template slot-scope="{row}">
            <el-button type="text" size="mini" @click="onDialogOpenByRefundApply(row)">
              退款
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </div>

    <div v-show="isShowHandle">
      <div v-loading="loadingByRefundApply">
        <el-page-header class="my-page-header" content="申请退款" @back="onGoBack" />
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
        <div v-if="details.order.isTimeoutPayed">

          <el-alert
            title="该订单是超时支付正常"
            type="error"
            :closable="false"
          />

        </div>
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
            <el-table-column label="操作" align="center" width="100" class-name="small-padding fixed-width">
              <template slot-scope="scope">
                <el-checkbox v-if="scope.row.applyCanSignRefunded" v-model="scope.row.applySignRefunded">标记退款</el-checkbox>
              </template>

            </el-table-column>
          </el-table>

        </div>

        <div class="row-title clearfix">
          <div class="pull-left"> <h5>退款申请</h5>
          </div>
        </div>

        <el-form ref="formByApply" :model="formByApply" :rules="rulesByApply" label-width="110px" style="max-width:800px;">
          <el-form-item label="退款提示:">

            <span style="line-height:30px">已退款金额：<span class="refundedAmount">{{ details.order.refundedAmount }}</span>，正在申请退款金额：<span class="refundingAmount">{{ details.order.refundingAmount }}</span>，可申请退款金额：<span class="refundableAmount">{{ details.order.refundableAmount }}</span></span>

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
            <el-button type="primary" @click="onApply()">
              提交
            </el-button>
            <el-button @click="onGoBack">
              返回
            </el-button>
          </el-form-item>
        </el-form>

      </div>

    </div>

    <el-result v-show="result.isShow" :icon="result.icon" :title="result.title" :sub-title="result.subTitle">
      <template slot="extra">
        <el-button type="primary" size="medium" @click="onGoList">返回处理列表</el-button>
        <el-button type="success" size="medium" @click="onSawResult">查看处理结果</el-button>
      </template>
    </el-result>
  </div>
</template>

<script>
import { searchOrder, getApplyDetails, apply } from '@/api/payrefund'
import { MessageBox } from 'element-ui'
import { getUrlParam, isEmpty } from '@/utils/commonUtil'
import PageHeader from '@/components/PageHeader/index.vue'
export default {
  name: 'PayTransRefundApply',
  components: { PageHeader },
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
          payWay: { text: '' }
        }
      },
      formByApply: {
        orderId: '',
        method: '',
        remark: '',
        amount: 0,
        refundSkus: []
      },
      rulesByApply: {
        method: [{ required: true, max: 200, message: '请选择退款方式', trigger: 'change' }],
        remark: [{ required: true, min: 1, max: 200, message: '原因不能为空', trigger: 'change' }]
      },
      result: {
        isShow: false,
        icon: 'success',
        title: '提交成功',
        subTitle: '请根据提示进行操作'
      },
      isShowList: true,
      isShowHandle: false,
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
        this.onSearch()
      }
    },
    onSearch() {
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
    onDialogOpenByRefundApply(row) {
      if (row.exStatus.value === 2) {
        this.$message({
          message: '该订单存在异常未有处理，请到订单中处理',
          type: 'error'
        })
        return
      }

      this.loadingByRefundApply = true
      this.formByApply.orderId = row.id
      this.formByApply.remark = ''
      this.formByApply.method = ''
      this.formByApply.amount = 0

      getApplyDetails({ orderId: row.id }).then(res => {
        if (res.result === 1) {
          this.details = res.data
        }
        this.loadingByRefundApply = false
        this.isShowList = false
        this.isShowHandle = true
      })
    },
    onApply() {
      var _this = this

      var refundSkus = []

      var skus = this.details.order.skus
      for (var i = 0; i < skus.length; i++) {
        var l_sku = skus[i]
        if (l_sku.applySignRefunded) {
          refundSkus.push({ uniqueId: l_sku.uniqueId, signRefunded: l_sku.applySignRefunded, refundedAmount: l_sku.applyRefundedAmount, refundedQuantity: l_sku.applyRefundedQuantity })
        }
      }

      if (refundSkus.length <= 0) {
        this.$message({
          message: '至少标记一个退款商品',
          type: 'error'
        })
        return
      }

      _this.formByApply.refundSkus = refundSkus

      this.$refs['formByApply'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要提交退款？', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            apply(_this.formByApply).then(res => {
              if (res.result === 1) {
                this.isShowHandle = false
                this.result.isShow = true
                this.$emit('onGetSummary')
                // var d = res.data
                // this.$router.push({
                //   path: '/paytrans/refund/query?payRefundId=' + d.payRefundId
                // })
              } else {
                this.$message({
                  message: res.message,
                  type: 'error'
                })
              }
            })
          }).catch(() => {
          })
        }
      })
    },
    onGoList() {
      this.isShowList = true
      this.isShowHandle = false
      this.result.isShow = false
    },
    onSawResult() {
      this.$router.push({
        path: '/paytrans/refund/query'
      })
    },
    onGoBack() {
      this.isShowList = true
      this.isShowHandle = false
      this.result.isShow = false
    }

  }
}
</script>

<style  lang="scss"  scoped>

#refund_apply{

}
</style>

