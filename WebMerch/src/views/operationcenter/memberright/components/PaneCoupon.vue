<template>
  <div id="coupon_list">
    <div class="filter-container">
      <el-row :gutter="12">
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-input v-model="listQuery.name" clearable style="width: 100%" placeholder="优惠券名称" class="filter-item" />
        </el-col>
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter">查询</el-button>
          <el-button class="filter-item" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleOpenDialogByAddCoupon">添加</el-button>
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
      <el-table-column label="序号" prop="id" align="left" width="80">
        <template slot-scope="scope">
          <span>{{ scope.$index+1 }} </span>
        </template>
      </el-table-column>
      <el-table-column label="优惠券名称" prop="name" align="left" min-width="40%">
        <template slot-scope="scope">
          <span>{{ scope.row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column label="券种" prop="faceType" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.faceType }}</span>
        </template>
      </el-table-column>
      <el-table-column label="券值" prop="faceValue" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.faceValue }}</span>
        </template>
      </el-table-column>
      <el-table-column label="数量" prop="faceValue" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.quantity }}</span> 张
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" width="160" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button type="text" size="mini" @click="handleRemove(row)">
            移除
          </el-button>
        </template>
      </el-table-column>
    </el-table>
    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="_getListData" />

    <el-dialog
      title="添加"
      :visible.sync="dialogByAddCouponIsVisible"
      :width="isDesktop==true?'600px':'90%'"
    >
      <el-form
        ref="formByAddCoupon"
        v-loading="dialogByAddCouponIsLoading"
        :model="formByAddCoupon"
        :rules="rulesAddCoupon"
        label-width="120px"
      >
        <el-form-item label="优惠券搜索">
          <el-autocomplete
            v-model="searchNameByCoupon"
            style="width:300px"
            :fetch-suggestions="searchAsyncByCoupon"
            placeholder="名称"
            @select="selectByCoupon"
          />
        </el-form-item>

        <el-form-item label="优惠券名称">
          <span>{{ formByAddCoupon.name }}</span>
        </el-form-item>
        <el-form-item label="数量">
          <el-input-number v-model="formByAddCoupon.quantity" :min="0" :max="20" style="width:160px" />
        </el-form-item>

      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button size="small" type="primary" @click="handelAddCoupon">保存</el-button>
        <el-button size="small" @click="dialogByAddCouponIsVisible=false">关闭</el-button>
      </span>
    </el-dialog>

  </div>
</template>

<script>

import { MessageBox } from 'element-ui'
import { getCoupons, removeCoupon, addCoupon, searchCoupon } from '@/api/memberright'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination
import { isEmpty } from '@/utils/commonUtil'

export default {
  name: 'OperationCenterMemberRightPaneCoupon',
  components: { Pagination },
  props: {
    levelstId: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      loading: false,
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        page: 1,
        limit: 10,
        name: undefined
      },
      formByAddCoupon: {
        name: '',
        levelStId: '',
        couponId: '',
        quantity: 0
      },
      rulesAddCoupon: {
        name: [{ required: true, min: 1, max: 200, message: '请选择优惠券', trigger: 'change' }]
      },
      searchNameByCoupon: '',
      dialogByAddCouponIsVisible: false,
      dialogByAddCouponIsLoading: false,
      isDesktop: this.$store.getters.isDesktop
    }
  },
  watch: {
    levelstId: function(val, oldval) {
      this.init()
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      if (!isEmpty(this.levelstId)) {
        this.listQuery.levelStId = this.levelstId
        this._getListData()
      }
    },
    _getListData() {
      this.loading = true
      getCoupons(this.listQuery).then(res => {
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
      this._getListData()
    },
    handleRemove(item) {
      MessageBox.confirm('确定要移除该优惠券？', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        removeCoupon({ couponStId: item.couponStId }).then(res => {
          if (res.result === 1) {
            this.$message({
              message: res.message,
              type: 'success'
            })
            this._getListData()
          } else {
            this.$message({
              message: res.message,
              type: 'error'
            })
          }
        })
      })
    },
    handelAddCoupon() {
      this.$refs['formByAddCoupon'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            addCoupon(this.formByAddCoupon).then(res => {
              if (res.result === 1) {
                this.$message({
                  message: res.message,
                  type: 'success'
                })
                this.dialogByAddCouponIsVisible = false
                this._getListData()
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
    handleOpenDialogByAddCoupon(item) {
      this.dialogByAddCouponIsVisible = true
      this.searchNameByCoupon = ''
      this.formByAddCoupon.levelStId = this.levelstId
      this.formByAddCoupon.couponId = ''
      this.formByAddCoupon.name = ''
      this.formByAddCoupon.quantity = 0
    },
    searchAsyncByCoupon(queryString, cb) {
      searchCoupon({ key: queryString }).then(res => {
        if (res.result === 1) {
          var d = res.data
          var restaurants = []
          for (var j = 0; j < d.length; j++) {
            restaurants.push({
              value: d[j].name,
              id: d[j].id,
              name: d[j].name
            })
          }

          cb(restaurants)
        }
      })
    },
    selectByCoupon(item) {
      this.formByAddCoupon.couponId = item.id
      this.formByAddCoupon.name = item.name
    }
  }
}
</script>
