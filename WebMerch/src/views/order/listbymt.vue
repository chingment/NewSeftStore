<template>
  <div id="order_list">
    <div class="filter-container">
      <el-form ref="form" label-width="120px" class="query-box">
        <el-form-item label="状态">
          <el-radio-group v-model="listQuery.orderStatus">
            <el-radio-button v-for="item in options_status" :key="item.value" :label="item.value">{{ item.label }}</el-radio-button>
          </el-radio-group>
        </el-form-item>
        <el-form-item v-if="isShowClientUserNameInput" label="下单用户">
          <el-input v-model="listQuery.clientUserName" clearable placeholder="下单用户" style="max-width: 300px;" class="filter-item" />
        </el-form-item>
        <el-form-item label="异常">
          <el-checkbox v-model="listQuery.isHasEx">异常未处理</el-checkbox>
        </el-form-item>
        <el-form-item label="订单号">
          <el-input v-model="listQuery.orderId" clearable placeholder="订单号" style="max-width: 300px;" class="filter-item" />
        </el-form-item>
        <el-form-item label="设备">
          <el-input v-model="listQuery.deviceCumCode" clearable placeholder="设备编码" style="max-width: 300px;" class="filter-item" />
        </el-form-item>
        <el-form-item label="下单时间">
          <el-date-picker
            v-model="listQuery.submittedTimeArea"
            type="datetimerange"
            :picker-options="pickerOptions"
            range-separator="至"
            start-placeholder="开始时间"
            end-placeholder="结束时间"
            align="right"
            style="max-width: 400px;"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="el-icon-search" @click="handleFilter">查 询</el-button>
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
      <el-table-column label="订单号" fixed="left" prop="id" align="left" width="220">
        <template slot-scope="scope">
          <span :class="'c-mode-'+(scope.row.isTestMode==false?'1':'2')">{{ scope.row.id }}</span>
        </template>
      </el-table-column>
      <el-table-column label="店铺" prop="storeName" align="left" min-width="100%">
        <template slot-scope="scope">
          <span>{{ scope.row.storeName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="设备" prop="deviceCode" align="left" width="120">
        <template slot-scope="scope">
          <span>{{ scope.row.deviceCode }}</span>
        </template>
      </el-table-column>
      <el-table-column label="下单用户" prop="clientUserName" align="left" width="120">
        <template slot-scope="scope">
          <span>{{ scope.row.clientUserName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="下单方式" prop="sourceName" align="left" width="120">
        <template slot-scope="scope">
          <span>{{ scope.row.sourceName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="取货方式" prop="sourceName" align="left" width="120">
        <template slot-scope="scope">
          <span>{{ scope.row.receiveModeName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="触发状态" prop="sourceName" align="left" width="120">
        <template slot-scope="scope">
          <span>{{ scope.row.pickupTrgStatus.text }}</span>
        </template>
      </el-table-column>
      <el-table-column label="数量" prop="quantity" align="left" width="60">
        <template slot-scope="scope">
          <span>{{ scope.row.quantity }}</span>
        </template>
      </el-table-column>
      <el-table-column label="金额" prop="chargeAmount" align="left" width="120">
        <template slot-scope="scope">
          <span>{{ scope.row.chargeAmount }}</span>
        </template>
      </el-table-column>
      <el-table-column label="异常？" prop="status" align="left" width="120">
        <template slot-scope="scope">
          <el-tag :type="getExStatusColor(scope.row.exStatus.value)">{{ scope.row.exStatus.text }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="下单时间" prop="submittedTime" align="left" width="160">
        <template slot-scope="scope">
          <span>{{ scope.row.submittedTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="状态" fixed="right" prop="status" align="center" width="100">
        <template slot-scope="scope">
          <span>{{ scope.row.status.text }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" fixed="right" align="center" width="80" class-name="small-padding fixed-width">
        <template slot-scope="{row}">

          <el-button v-if="row.canHandleEx" type="text" size="mini" @click="dialogDetailsOpen(row)">
            处理
          </el-button>
          <el-button v-else type="text" size="mini" @click="dialogDetailsOpen(row)">
            查看
          </el-button>

        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />

    <el-dialog title="订单详情" :visible.sync="dialogDetailsIsVisible" :width="isDesktop==true?'800px':'90%'">
      <div v-loading="detailsLoading">

        <div class="row-title clearfix">
          <div class="pull-left"> <h5>基本信息</h5>
          </div>
          <div class="pull-right">
            <el-button icon="el-icon-refresh" circle @click="refreshDetails(details.id)" />
          </div>
        </div>
        <el-form class="form-container" style="display:flex">
          <el-col :span="24">

            <div class="postInfo-container">
              <el-row>
                <el-col :span="24">
                  <el-form-item label-width="80px" label="订单编号:" class="postInfo-container-item">
                    {{ details.id }} <span style="color:#e1b11e">{{ details.isTestMode==false?"":"[测]" }}</span>
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
                <el-col :span="12">
                  <el-form-item label-width="80px" label="设备编码:" class="postInfo-container-item">
                    {{ details.deviceCumCode }}
                  </el-form-item>
                </el-col>
              </el-row>
              <el-row>
                <el-col :span="12">
                  <el-form-item label-width="80px" label="支付方式:" class="postInfo-container-item">
                    {{ details.payWay.text }}
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
        <div>
          <table class="table-skus" style="width:100%;table-layout:fixed;">
            <tr v-for="(sku,sub_index) in details.skus" :key="sub_index">
              <td style="width:60px">
                <img :src="sku.mainImgUrl" style="width:50px;height:50px;">
              </td>
              <td style="width:100%">
                {{ sku.name }}
              </td>
              <td style="width:50px">
                x {{ sku.quantity }}
              </td>
              <td v-show="details.receiveMode===4" style="width:200px;text-align:center;">
                {{ sku.status.text }}
              </td>
              <td v-show="details.receiveMode===4" style="width:100px;text-align:center;">
                <el-popover
                  v-if="sku.pickupLogs.length>0"
                  placement="right"
                  width="400"
                  trigger="click"
                >
                  <el-timeline>
                    <el-timeline-item
                      v-for="(activity, index) in sku.pickupLogs"
                      :key="index"
                      :timestamp="activity.timestamp"
                    >
                      {{ activity.content }}
                      <br>
                      <el-image
                        v-for="(url, k) in activity.imgUrls"
                        :key="k"
                        style="width: 80px; height: 80px;margin-right:5px;"
                        :src="url"
                        :preview-src-list="activity.imgUrls"
                      />

                    </el-timeline-item>
                  </el-timeline>
                  <el-link slot="reference" type="primary" style="margin-right:15px;">出货流程</el-link>
                </el-popover>
              </td>
              <td style="width:200px">
                <div v-if="sku.status.value==6000">
                  <el-radio v-model="sku.signStatus" label="1" style="margin-right:5px;">已取</el-radio>
                  <el-radio v-model="sku.signStatus" label="2">未取</el-radio>
                </div>
              </td>
            </tr>
          </table>
        </div>
        <div v-if="details.refundRecords!=null&&details.refundRecords.length>0">
          <div class="row-title clearfix">
            <div class="pull-left"> <h5> 退款记录</h5>
            </div>
          </div>

          <el-table
            :data="details.refundRecords"
            fit
            highlight-current-row
            style="width: 100%;"
          >
            <el-table-column label="序号" prop="id" align="left" width="80">
              <template slot-scope="scope">
                <span>{{ scope.$index+1 }} </span>
              </template>
            </el-table-column>
            <el-table-column label="交易号" align="left" min-width="40%">
              <template slot-scope="scope">
                <span>{{ scope.row.id }}</span>
              </template>
            </el-table-column>
            <el-table-column label="状态" align="left" min-width="15%">
              <template slot-scope="scope">
                <span>{{ scope.row.status.text }}</span>
              </template>
            </el-table-column>
            <el-table-column label="金额" align="left" min-width="15%">
              <template slot-scope="scope">
                <span>{{ scope.row.amount }}</span>
              </template>
            </el-table-column>
            <el-table-column label="时间" align="left" min-width="30%">
              <template slot-scope="scope">
                <span>{{ scope.row.dateTime }}</span>
              </template>
            </el-table-column>
          </el-table>
        </div>
        <div v-if="details.exIsHappen">
          <div class="row-title clearfix">
            <div class="pull-left"> <h5> 异常处理</h5>
            </div>
          </div>

          <el-form v-if="details.canHandleEx" ref="formByApply" :model="formByHandle" :rules="rulesByHandle" label-width="80px" style="max-width:800px;">
            <el-form-item label="备注:" prop="remark">
              <el-input v-model="formByHandle.remark" />
            </el-form-item>
          </el-form>
          <p v-else>{{ details.exHandleRemark }}</p>
        </div>
      </div>
      <div slot="footer" class="dialog-footer">
        <el-button v-if="details.canHandleEx" size="small" type="primary" @click="_handleEx(details)">
          确认处理
        </el-button>
        <el-button size="small" @click="dialogDetailsIsVisible = false">
          关闭
        </el-button>
      </div>
    </el-dialog>

  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { getList, getDetails, handleExByDeviceSelfTake, SendDeviceShip } from '@/api/order'
import Pagination from '@/components/Pagination'
import { isEmpty, getUrlParam } from '@/utils/commonUtil'
export default {
  name: 'OrderListByMt',
  components: { Pagination },
  props: {
    storeId: {
      type: String,
      require: false,
      default: ''
    },
    sellChannelRefId: {
      type: String,
      require: false,
      default: ''
    },
    receiveMode: {
      type: String,
      require: false,
      default: '4'
    },
    clientUserId: {
      type: String,
      require: false,
      default: ''
    }
  },
  data() {
    return {
      navActive: '1',
      loading: false,
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        receiveMode: 4,
        deviceCumCode: '',
        submittedTimeArea: undefined,
        page: 1,
        limit: 10,
        clientName: undefined,
        orderId: undefined,
        orderStatus: '0',
        storeId: undefined,
        isHasEx: false,
        sellChannelRefId: undefined
      },
      dialogDetailsIsVisible: false,
      detailsLoading: false,
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
        isRunning: false,
        refundRecords: null,
        payWay: { text: '' }
      },
      options_status: [{
        value: '0',
        label: '全部'
      }, {
        value: '2000',
        label: '待支付'
      }, {
        value: '3000',
        label: '已支付'
      }, {
        value: '4000',
        label: '已完成'
      }, {
        value: '5000',
        label: '已取消'
      }],
      options_ReceiveModes: [{
        value: '4',
        label: '设备自提'
      }, {
        value: '2',
        label: '店铺自取'
      }, {
        value: '1',
        label: '配送商品'
      }],
      options_pickupTrgStatus: [
        {
          value: '1',
          label: '未触发'
        }, {
          value: '2',
          label: '已触发'
        }
      ],
      formByHandle: {
        refundMethod: '1',
        remark: '',
        isRefund: false,
        refundAmount: 0
      },
      rulesByHandle: {
        remark: [{ required: true, min: 1, max: 200, message: '原因不能为空', trigger: 'change' }]
      },
      pickerOptions: {
        shortcuts: [{
          text: '最近一周',
          onClick(picker) {
            const end = new Date()
            const start = new Date()
            start.setTime(start.getTime() - 3600 * 1000 * 24 * 7)
            picker.$emit('pick', [start, end])
          }
        }, {
          text: '最近一个月',
          onClick(picker) {
            const end = new Date()
            const start = new Date()
            start.setTime(start.getTime() - 3600 * 1000 * 24 * 30)
            picker.$emit('pick', [start, end])
          }
        }, {
          text: '最近三个月',
          onClick(picker) {
            const end = new Date()
            const start = new Date()
            start.setTime(start.getTime() - 3600 * 1000 * 24 * 90)
            picker.$emit('pick', [start, end])
          }
        }]
      },
      isDesktop: this.$store.getters.isDesktop,
      isShowClientUserNameInput: true
    }
  },
  watch: {
    $route() {
      var receiveMode = getUrlParam('receiveMode')
      this.listQuery.receiveMode = receiveMode
      this.init()
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }

    var urlPrm_isHasEx = this.$cookies.get('isHasEx')
    if (urlPrm_isHasEx === '1') {
      this.$cookies.remove('isHasEx')
      this.listQuery.isHasEx = true
    } else {
      this.listQuery.isHasEx = false
    }

    var receiveMode = getUrlParam('receiveMode')
    if (receiveMode != null) {
      this.listQuery.receiveMode = receiveMode
    } else {
      this.listQuery.receiveMode = this.receiveMode
    }

    this.listQuery.storeId = this.storeId
    this.listQuery.sellChannelRefId = this.sellChannelRefId

    if (this.clientUserId === '') {
      this.isShowClientUserNameInput = true
    } else {
      this.isShowClientUserNameInput = false
    }
    this.listQuery.clientUserId = this.clientUserId
    this.init()
  },
  methods: {
    init() {
      this.getListData()
    },
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
    refreshDetails(id) {
      this.detailsLoading = true
      getDetails({ id: id }).then(res => {
        if (res.result === 1) {
          this.details = res.data
        }
        this.detailsLoading = false
      })
    },
    dialogDetailsOpen(row) {
      this.dialogDetailsIsVisible = true
      this.detailsLoading = true
      this.formByHandle.remark = ''
      getDetails({ id: row.id }).then(res => {
        if (res.result === 1) {
          this.details = res.data
        }
        this.detailsLoading = false
      })
    },
    _handleEx(details) {
      var _this = this
      var _formByHandle = _this.formByHandle
      if (isEmpty(_this.formByHandle.remark)) {
        this.$message({ message: '请输入备注', type: 'warning' })
        return
      }

      var uniques = []
      if (details.receiveMode === 4) {
        for (var i = 0; i < details.skus.length; i++) {
          var l_sku = details.skus[i]
          if (l_sku.status.value === 6000) {
            if (l_sku.signStatus === 0) {
              this.$message({ message: '处理前，请选择【' + l_sku.name + '】的取货状态 已取或未取', type: 'warning' })
              return
            } else {
              uniques.push({ uniqueId: l_sku.uniqueId, signStatus: l_sku.signStatus })
            }
          }
        }
      }

      MessageBox.confirm('确定要处理,慎重操作，会影响设备实际库存', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        dangerouslyUseHTMLString: true,
        type: 'warning'
      }).then(() => {
        this.detailsLoading = true
        handleExByDeviceSelfTake({ id: details.id, uniques: uniques, remark: _formByHandle.remark, isRefund: _formByHandle.isRefund, refundAmount: _formByHandle.refundAmount, refundMethod: _formByHandle.refundMethod, isRunning: false }).then(res => {
          if (res.result === 1) {
            this.$message({
              message: res.message,
              type: 'success'
            })
            _this.refreshDetails(details.id)
            _this.getListData()
          } else {
            this.$message({
              message: res.message,
              type: 'error'
            })
          }

          this.detailsLoading = false
        })
      })
    },
    _sendDevice(details) {
      MessageBox.confirm('确定要发起出货', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        dangerouslyUseHTMLString: true,
        type: 'warning'
      }).then(() => {
        SendDeviceShip({ id: details.id }).then(res => {
          if (res.result === 1) {
            this.$message({
              message: res.message,
              type: 'success'
            })
          } else {
            this.$message({
              message: res.message,
              type: 'error'
            })
          }
        })
      })
    },
    selectNav(e) {

    },
    getExStatusColor(status) {
      switch (status) {
        case 0:
          return 'info'
        case 1:
          return ''
        case 2:
          return 'danger'
        case 3:
          return ''
        case 4:
        case 5:
          return ''
      }
      return ''
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

  .el-form-item {
    margin-bottom: 10px;
}
}
</style>
