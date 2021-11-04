<template>
  <div id="refund_handle">

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
        <el-table-column label="状态" align="left" fixed="right" width="100">
          <template slot-scope="scope">
            <el-tag :type="getStatusColor(scope.row.status.value)">{{ scope.row.status.text }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" align="center" fixed="right" width="100" class-name="small-padding fixed-width">
          <template slot-scope="{row}">
            <el-button v-if="row.status.value==1" type="text" size="mini" @click="onDialogOpenByRefundHandle(row)">
              处理
            </el-button>
          </template>
        </el-table-column>
      </el-table>
      <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="onGetList" />

    </div>

    <div v-show="isShowHandle">
      <div v-loading="loadingByRefundHandle">
        <el-page-header class="my-page-header" content="退款处理" @back="onGoBack" />
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
            <el-table-column label="操作" align="center" width="100" class-name="small-padding fixed-width">
              <template slot-scope="scope">
                <span v-if="scope.row.applySignRefunded">标记退款</span>
              </template>
            </el-table-column>
          </el-table>

        </div>

        <div class="row-title clearfix">
          <div class="pull-left"> <h5>退款申请</h5>
          </div>
        </div>

        <el-form ref="form" label-width="110px" style="max-width:800px;">
          <el-form-item label="退款提示:">

            <span style="line-height:30px;">已退款金额：<span class="refundedAmount">{{ details.order.refundedAmount }}</span>，正在申请退款金额：<span class="refundingAmount">{{ details.order.refundingAmount }}</span>，可申请退款金额：<span class="refundableAmount">{{ details.order.refundableAmount }}</span></span>

          </el-form-item>
          <el-form-item label="申请退款方式:" prop="method">
            <span>{{ details.applyMethod.text }}</span>
          </el-form-item>
          <el-form-item label="申请退款金额:" prop="amount">
            <span>{{ details.applyAmount }}</span>
          </el-form-item>
          <el-form-item label="申请原因:" prop="reason">
            <span>{{ details.applyRemark }}</span>
          </el-form-item>

        </el-form>

        <div class="row-title clearfix">
          <div class="pull-left"> <h5>退款处理</h5>
          </div>
        </div>

        <el-form ref="formByHandle" :model="formByHandle" :rules="rulesByHandle" label-width="110px" style="max-width:800px;">

          <el-form-item v-show="details.applyMethod.value===1" label="方式:" prop="result">
            <el-radio-group v-model="formByHandle.result">
              <el-radio label="3">转入自动退款</el-radio>
              <el-radio label="4">无效</el-radio>
            </el-radio-group>
          </el-form-item>

          <el-form-item v-show="details.applyMethod.value===2" label="结果:" prop="result">
            <el-radio-group v-model="formByHandle.result">
              <el-radio label="1">退款成功</el-radio>
              <el-radio label="2">退款失败</el-radio>
              <el-radio label="4">无效</el-radio>
            </el-radio-group>
          </el-form-item>

          <el-form-item label="备注:" prop="remark">
            <el-input v-model="formByHandle.remark" clearable />
          </el-form-item>
          <!--
          <el-form-item label="支付密码:" prop="remark">
            <password-input style="width:250px" @get-pwd="getPwd" />
          </el-form-item> -->

        </el-form>

      </div>
      <div slot="footer" class="dialog-footer" style="padding-left:110px;">
        <el-button size="small" type="primary" @click="onHandle()">
          提交
        </el-button>
        <el-button size="small" @click="onGoBack">
          返回
        </el-button>
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
import { getListByHandle, getHandleDetails, handle } from '@/api/payrefund'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination
import { MessageBox } from 'element-ui'
import PasswordInput from '@/components/PasswordInput'
export default {
  name: 'PayTransRefundHandle',
  components: { Pagination, PasswordInput },
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
        payPartnerPayTransId: undefined
      },
      formByHandle: {
        payRefundId: '',
        remark: '',
        result: '',
        amount: 0
      },
      rulesByHandle: {
        result: [{ required: true, max: 200, message: '请选择结果', trigger: 'change' }],
        remark: [{ required: true, min: 1, max: 200, message: '原因不能为空', trigger: 'change' }]
      },
      details: {
        payRefundId: '',
        applyMethod: { text: '' },
        applyAmount: '',
        applyTime: '',
        applyRemark: '',
        order: {
          id: '',
          storeName: '',
          clientUserName: '',
          sourceName: '',
          quantity: '',
          originalAmount: '',
          discountAmount: '',
          chargeAmount: '',
          submittedTime: '',
          status: { text: '' },
          refundedAmount: '',
          refundingAmount: '',
          refundableAmount: '',
          payWay: { text: '' }
        }
      },
      result: {
        isShow: false,
        icon: 'success',
        title: '提交成功',
        subTitle: '请根据提示进行操作'
      },
      loadingByRefundHandle: false,
      isShowList: true,
      isShowHandle: false,
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
      getListByHandle(this.listQuery).then(res => {
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
    onDialogOpenByRefundHandle(row) {
      this.loadingByRefundHandle = true
      this.formByHandle.payRefundId = row.id
      this.formByHandle.amount = row.applyAmount
      this.formByHandle.result = ''
      this.formByHandle.remark = ''

      // if (row.applyMethod.value === 1) {
      //   this.rulesByHandle.result[0].required = false
      // } else if (row.applyMethod.value === 2) {
      //   this.rulesByHandle.result[0].required = true
      // }

      getHandleDetails({ payRefundId: row.id }).then(res => {
        if (res.result === 1) {
          this.details = res.data
        }
        this.loadingByRefundHandle = false
        this.isShowList = false
        this.isShowHandle = true
      })
    },
    onHandle() {
      var _this = this

      this.$refs['formByHandle'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要处理？', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            handle(_this.formByHandle).then(res => {
              if (res.result === 1) {
                this.isShowHandle = false
                this.result.isShow = true
                this.$emit('onGetSummary')
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
      this.onGetList()
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
    }
  }
}
</script>
