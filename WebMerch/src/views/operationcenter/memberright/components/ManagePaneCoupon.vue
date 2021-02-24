<template>
  <div id="coupon_list">
    <div class="filter-container">
      <el-row :gutter="12">
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-input v-model="listQuery.name" clearable style="width: 100%" placeholder="优惠券名称" class="filter-item" @keyup.enter.native="handleFilter" @clear="handleFilter" />
        </el-col>
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter">查询</el-button>
          <el-button class="filter-item" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleCreate">添加</el-button>
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
          <el-button type="primary" size="mini" @click="handleRemove(row)">
            移除
          </el-button>
        </template>
      </el-table-column>
    </el-table>
    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />
  </div>
</template>

<script>

import { MessageBox } from 'element-ui'
import { getCouponsByLevelSt, removeCoupon } from '@/api/memberright'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination
import { getUrlParam, isEmpty } from '@/utils/commonUtil'

export default {
  name: 'ManagePaneCoupon',
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
      getCouponsByLevelSt(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
          this.listTotal = d.total
        }
        this.loading = false
      })
    },
    handleRemove(item) {
      MessageBox.confirm('确定要移除该优惠券？', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        removeCoupon({ couponStId: item.couponStId }).then(res => {
          this.$message(res.message)
          if (res.result === 1) {
            this._getListData()
          }
        })
      })
    }
  }
}
</script>
