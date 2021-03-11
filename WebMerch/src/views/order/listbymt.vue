<template>
  <div id="order_list">
    <div class="filter-container">
      <el-row :gutter="12">
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-input v-model="listQuery.orderId" clearable placeholder="订单号" va style="width: 100%" class="filter-item" @keyup.enter.native="handleFilter" @clear="handleFilter" />
        </el-col>
        <el-col v-if="isShowClientUserNameInput" :span="3" :xs="12" style="margin-bottom:20px">
          <el-input v-model="listQuery.clientUserName" clearable placeholder="下单用户" va style="width: 100%" class="filter-item" />
        </el-col>
        <el-col :span="3" :xs="12" style="margin-bottom:20px">
          <el-select v-model="listQuery.orderStatus" clearable placeholder="全部状态" style="width: 100%">
            <el-option
              v-for="item in options_status"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-col>
        <el-col :span="3" :xs="12" style="margin-bottom:20px">
          <el-select v-model="listQuery.pickupTrgStatus" clearable placeholder="触发状态" style="width: 100%">
            <el-option
              v-for="item in options_pickupTrgStatus"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-col>
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-checkbox v-model="listQuery.isHasEx">异常未处理</el-checkbox>
          <el-button class="filter-item" type="primary" icon="el-icon-search" style="margin-left:20px;" @click="handleFilter">
            查询
          </el-button>
        </el-col>
      </el-row>
      <el-button style="position: absolute;right: 10px;top: 20px;" icon="el-icon-refresh" circle @click="getListData(listQuery)" />

      <el-table
        :key="listKey"
        v-loading="loading"
        :data="listData"
        fit
        highlight-current-row
        style="width: 100%;"
      >
        <el-table-column type="expand">
          <template slot-scope="scope">
            <div v-for="(item,index) in scope.row.receiveDetails" :key="index">
              <div> <i class="el-icon-place" /><span> {{ item.name }} </span> <i class="el-icon-d-arrow-right" /> </div>
              <table class="table-skus" style="width:100%;table-layout:fixed;max-width: 800px;">
                <tr v-for="(pickupSku,sub_index) in item.detailItems" :key="sub_index">
                  <td style="20%">
                    <img :src="pickupSku.mainImgUrl" style="width:50px;height:50px;">
                  </td>
                  <td style="20%">
                    {{ pickupSku.name }}
                  </td>
                  <td style="30%">
                    x {{ pickupSku.quantity }}
                  </td>
                  <td v-show="scope.row.receiveMode===4" style="15%">
                    {{ pickupSku.status.text }}
                  </td>
                  <td v-show="scope.row.receiveMode===4" style="width:15%;text-align:center;">
                    <el-popover
                      v-if="pickupSku.pickupLogs.length>0"
                      placement="right"
                      width="400"
                      trigger="click"
                    >
                      <el-timeline>
                        <el-timeline-item
                          v-for="(activity, index) in pickupSku.pickupLogs"
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
                </tr>
              </table>
            </div>
          </template>
        </el-table-column>
        <!-- <span>{{ scope.row.id }}</span> <span style="color:#e1b11e">{{ scope.row.isTestMode==false?"":"[测]" }}</span> -->
        <el-table-column label="订单号" prop="id" align="left" :width="isDesktop==true?220:80">
          <template slot-scope="scope">
            <span :class="'c-mode-'+(scope.row.isTestMode==false?'1':'2')">{{ scope.row.id }}</span>
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
        <el-table-column v-if="isDesktop" label="数量" prop="quantity" align="left" min-width="10%">
          <template slot-scope="scope">
            <span>{{ scope.row.quantity }}</span>
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
        <el-table-column label="异常？" prop="status" align="left" width="120">
          <template slot-scope="scope">
            <span :class="'enable-status enable-status-'+scope.row.exStatus.value">{{ scope.row.exStatus.text }}</span>
          </template>
        </el-table-column>
        <el-table-column v-if="isDesktop" label="下单时间" prop="submittedTime" align="left" min-width="15%">
          <template slot-scope="scope">
            <span>{{ scope.row.submittedTime }}</span>
          </template>
        </el-table-column>
        <el-table-column label="操作" fixed="right" align="center" width="80" class-name="small-padding fixed-width">
          <template slot-scope="{row}">

            <el-button v-if="row.canHandleEx" type="warning" size="mini" @click="dialogDetailsOpen(row)">
              处理
            </el-button>
            <el-button v-else type="primary" size="mini" @click="dialogDetailsOpen(row)">
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
                  <el-col :span="12" />
                </el-row>
              </div>
            </el-col>
          </el-form>
          <div class="row-title clearfix">
            <div class="pull-left"> <h5>商品信息</h5>
            </div>
          </div>
          <div v-for="(receiveMode,index) in details.receiveModes" :key="index">
            <div> <i class="el-icon-place" /><span> {{ receiveMode.name }} </span> <i class="el-icon-d-arrow-right" /> </div>
            <table class="table-skus" style="width:100%;table-layout:fixed;">
              <tr v-for="(pickupSku,sub_index) in receiveMode.items" :key="sub_index">
                <td style="width:60px">
                  <img :src="pickupSku.mainImgUrl" style="width:50px;height:50px;">
                </td>
                <td style="width:100%">
                  {{ pickupSku.name }}
                </td>
                <td style="width:50px">
                  x {{ pickupSku.quantity }}
                </td>
                <td v-show="receiveMode.mode===4" style="width:200px;text-align:center;">
                  {{ pickupSku.status.text }}
                </td>
                <td v-show="receiveMode.mode===4" style="width:100px;text-align:center;">
                  <el-popover
                    v-if="pickupSku.pickupLogs.length>0"
                    placement="right"
                    width="400"
                    trigger="click"
                  >
                    <el-timeline>
                      <el-timeline-item
                        v-for="(activity, index) in pickupSku.pickupLogs"
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
                <td style="width:200px;text-align:center;">
                  <div v-if="pickupSku.status.value==6000">
                    <el-radio v-model="pickupSku.signStatus" label="1" style="margin-right:5px;">已取</el-radio>
                    <el-radio v-model="pickupSku.signStatus" label="2">未取</el-radio>
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
              <el-table-column v-if="isDesktop" label="序号" prop="id" align="left" width="80">
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
              <el-form-item label="申请退款:" prop="isRefund">
                <el-radio-group v-model="formByHandle.isRefund">
                  <el-radio :label="false">否</el-radio>
                  <el-radio :label="true">是</el-radio>
                </el-radio-group>
              </el-form-item>
              <el-form-item v-show="formByHandle.isRefund" label="退款提示:">
                <span>已退款金额：<span class="refundedAmount">{{ details.refundedAmount }}</span>，正在申请退款金额：<span class="refundingAmount">{{ details.refundingAmount }}</span>，可申请退款金额：<span class="refundableAmount">{{ details.refundableAmount }}</span></span>
              </el-form-item>
              <el-form-item v-show="formByHandle.isRefund" label="退款方式:" prop="refundMethod">
                <el-radio-group v-model="formByHandle.refundMethod">
                  <el-radio label="1">原路退回（系统自动处理，退款到用户支付账号）</el-radio>
                  <el-radio label="2">线下退回（人工审核处理，线下人工退回）</el-radio>
                </el-radio-group>
              </el-form-item>
              <el-form-item v-show="formByHandle.isRefund" label="退款金额:" prop="refundAmount">
                <el-input v-model="formByHandle.refundAmount" style="width:160px">
                  <template slot="prepend">￥</template>
                </el-input>
              </el-form-item>
              <el-form-item label="备注:" prop="remark">
                <el-input v-model="formByHandle.remark" />
              </el-form-item>

            </el-form>

            <p v-else>{{ details.exHandleRemark }}</p>
          </div>
        </div>
        <div slot="footer" class="dialog-footer">
          <el-button v-if="details.canHandleEx" type="primary" @click="_handleEx(details)">
            确认处理
          </el-button>
          <el-button @click="dialogDetailsIsVisible = false">
            关闭
          </el-button>
        </div>
      </el-dialog>

    </div>
  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { getList, getDetailsByMachineSelfTake, handleExByMachineSelfTake } from '@/api/order'
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
        page: 1,
        limit: 10,
        clientName: undefined,
        orderId: undefined,
        orderStatus: undefined,
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
        refundRecords: null
      },
      options_status: [{
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
        label: '机器自提'
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
      getDetailsByMachineSelfTake({ id: id }).then(res => {
        if (res.result === 1) {
          this.details = res.data
        }
        this.detailsLoading = false
      })
    },
    dialogDetailsOpen(row) {
      this.detailsLoading = true
      this.formByHandle.remark = ''
      getDetailsByMachineSelfTake({ id: row.id }).then(res => {
        if (res.result === 1) {
          this.details = res.data
        }
        this.detailsLoading = false
        this.dialogDetailsIsVisible = true
      })
    },
    _handleEx(details) {
      var _this = this
      var _formByHandle = _this.formByHandle
      if (isEmpty(_this.formByHandle.remark)) {
        this.$message('请输入备注')
        return
      }

      var uniques = []

      for (var i = 0; i < details.receiveModes.length; i++) {
        if (details.receiveModes[i].mode === 4) {
          var l_items = details.receiveModes[i].items
          for (var j = 0; j < l_items.length; j++) {
            if (l_items[j].status.value === 6000) {
              if (l_items[j].signStatus === 0) {
                this.$message('处理前，请选择【' + l_items[j].name + '】的取货状态 已取或未取')
                return
              } else {
                uniques.push({ uniqueId: l_items[j].uniqueId, signStatus: l_items[j].signStatus })
              }
            }
          }
        }
      }

      MessageBox.confirm('确定要处理,慎重操作，会影响机器实际库存', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        dangerouslyUseHTMLString: true,
        type: 'warning'
      }).then(() => {
        this.detailsLoading = true
        handleExByMachineSelfTake({ id: details.id, uniques: uniques, remark: _formByHandle.remark, isRefund: _formByHandle.isRefund, refundAmount: _formByHandle.refundAmount, refundMethod: _formByHandle.refundMethod, isRunning: false }).then(res => {
          this.$message(res.message)
          if (res.result === 1) {
            _this.refreshDetails(details.id)
            _this.getListData()
          }

          this.detailsLoading = false
        })
      })
    },
    selectNav(e) {

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
