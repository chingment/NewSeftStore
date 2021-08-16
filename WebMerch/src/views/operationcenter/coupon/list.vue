<template>
  <div id="coupon_list">
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px">
          <el-input v-model="listQuery.name" clearable style="width: 100%" placeholder="优惠券名称" class="filter-item" />
        </el-col>
        <el-col :xs="24" :sm="12" :lg="6" :xl="4" style="margin-bottom:20px;display:flex">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter">
            查询
          </el-button>
          <el-button class="filter-item" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleCreate">
            新建
          </el-button>
          <el-button class="filter-item" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleOpenDialogBySendCoupon">
            赠送
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
      @selection-change="handleSelectionChange"
    >
      <el-table-column
        type="selection"
        width="55"
      />
      <el-table-column label="序号" prop="id" align="left" width="80">
        <template slot-scope="scope">
          <span>{{ scope.$index+1 }} </span>
        </template>
      </el-table-column>
      <el-table-column label="优惠券名称" prop="name" align="left" min-width="15%">
        <template slot-scope="scope">
          <span>{{ scope.row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column label="优惠券类型" prop="category" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.category }}</span>
        </template>
      </el-table-column>
      <el-table-column label="使用方式" prop="useMode" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.useMode }}</span>
        </template>
      </el-table-column>
      <el-table-column label="可使用范围" prop="useAreaType" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.useAreaType }}</span>
        </template>
      </el-table-column>
      <el-table-column label="使用门槛" prop="atLeastAmount" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.atLeastAmount }}</span>
        </template>
      </el-table-column>
      <el-table-column label="券种" prop="faceType" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.faceType }}</span>
        </template>
      </el-table-column>
      <el-table-column label="券值" prop="faceValue" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.faceValue }}</span>
        </template>
      </el-table-column>
      <el-table-column label="有效期" prop="validDate" align="left" min-width="15%">
        <template slot-scope="scope">
          <span>{{ scope.row.validDate }}</span>
        </template>
      </el-table-column>
      <el-table-column label="状态" prop="validDate" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.status.text }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" width="160" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <!-- <el-button type="primary" size="mini" @click="handleDetails(row)">
            查看
          </el-button> -->
          <el-button type="text" size="mini" @click="handleOpenDialogByReceiveRecord(row)">
            领取记录
          </el-button>
          <el-button type="text" size="mini" @click="handleEdit(row)">
            修改
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />

    <el-dialog v-if="dialogIsShowByReceiveRecord" title="领取记录" :visible.sync="dialogIsShowByReceiveRecord" width="800px" append-to-body>
      <pane-receive-record :coupon-id="couponId" />
    </el-dialog>

    <el-dialog v-if="dialogIsShowBySendCoupon" title="发送优惠券" :visible.sync="dialogIsShowBySendCoupon" width="800px" append-to-body>

      <el-form ref="formBySendCoupon" v-loading="loadingBySendCoupon" :model="formBySendCoupon" :rules="rulesBySendCoupon" label-width="80px">
        <el-form-item label="优惠券">
          <template v-for="(item,index) in selectCoupons">
            <div :key="index">{{ item.name }} </div>
          </template>
        </el-form-item>
        <el-form-item label="数量">
          <el-input-number v-model="formBySendCoupon.quantity" :min="1" :max="10" style="width:160px" />
        </el-form-item>
        <el-form-item label="客户" prop="clientUserIds">
          <client-select multiple :select-ids="formBySendCoupon.clientUserIds" @GetSelectIds="getClientUserIds" />
        </el-form-item>
      </el-form>

      <div slot="footer" class="dialog-footer">
        <el-button size="small" @click="dialogIsShowBySendCoupon = false">
          取消
        </el-button>
        <el-button size="small" type="primary" @click="handleSendCoupon">
          确定
        </el-button>
      </div>

    </el-dialog>

  </div>
</template>

<script>

import { MessageBox } from 'element-ui'
import { getList, send } from '@/api/coupon'
import Pagination from '@/components/Pagination'
import PaneReceiveRecord from './components/PaneReceiveRecord.vue'
import ClientSelect from '@/views/client/components/select.vue'
export default {
  name: 'OperationCenterCouponList',
  components: { Pagination, PaneReceiveRecord, ClientSelect },
  data() {
    return {
      loading: false,
      listKey: 0,
      listData: null,
      listTotal: 0,
      couponId: '',
      selectCoupons: [],
      listQuery: {
        page: 1,
        limit: 10,
        name: undefined
      },
      dialogIsShowByReceiveRecord: false,
      dialogIsShowBySendCoupon: false,
      loadingBySendCoupon: false,
      formBySendCoupon: {
        couponIds: [],
        quantity: 1,
        clientUserIds: []
      },
      rulesBySendCoupon: {
        clientUserIds: [
          { type: 'array', required: true, message: '请至少选择一个客户', trigger: 'change' }
        ]
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
    handleDetails(row) {
      this.$router.push({
        path: '/operationcenter/coupon/details?id=' + row.id
      })
    },
    handleEdit(row) {
      this.$router.push({
        path: '/operationcenter/coupon/edit?id=' + row.id
      })
    },
    handleCreate(row) {
      this.$router.push({
        path: '/operationcenter/coupon/add'
      })
    },
    handleOpenDialogByReceiveRecord(item) {
      this.couponId = item.id
      this.dialogIsShowByReceiveRecord = true
    },
    handleSelectionChange(val) {
      this.selectCoupons = val
    },
    handleOpenDialogBySendCoupon() {
      if (this.selectCoupons == null || this.selectCoupons.length === 0) {
        this.$message('请选择优惠券')
        return
      }

      this.formBySendCoupon.couponIds = []

      for (let i = 0; i < this.selectCoupons.length; i++) {
        this.formBySendCoupon.couponIds.push(this.selectCoupons[i].id)
      }

      this.formBySendCoupon.clientUserIds = []
      this.formBySendCoupon.quantity = 1
      this.dialogIsShowBySendCoupon = true
    },
    getClientUserIds(ids) {
      console.log(JSON.stringify(ids))
      this.formBySendCoupon.clientUserIds = ids
    },
    handleSendCoupon() {
      this.$refs['formBySendCoupon'].validate(valid => {
        if (valid) {
          MessageBox.confirm('确定要发送？', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            send(this.formBySendCoupon).then(res => {
              if (res.result === 1) {
                this.$message({
                  message: res.message,
                  type: 'success'
                })
                this.dialogIsShowBySendCoupon = false
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
    }

  }
}
</script>
