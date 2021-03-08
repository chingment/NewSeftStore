<template>
  <div id="sku_list">
    <div class="filter-container">
      <el-row :gutter="12">
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-input v-model="listQuery.name" clearable style="width: 100%" placeholder="商品名称" class="filter-item" @keyup.enter.native="handleFilter" @clear="handleFilter" />
        </el-col>
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter">查询</el-button>
          <el-button class="filter-item" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleOpenDialogByEditSku">添加</el-button>
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
      <el-table-column label="商品名称" prop="skuName" align="left" min-width="40%">
        <template slot-scope="scope">
          <span>{{ scope.row.skuName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="店铺" prop="storeName" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.storeName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="实际价" prop="salePrice" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.salePrice }}</span>
        </template>
      </el-table-column>
      <el-table-column label="会员价" prop="memberPrice" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.memberPrice }}</span> 张
        </template>
      </el-table-column>
      <el-table-column label="状态" prop="status" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.status.text }}</span> 张
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" width="160" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button type="primary" size="mini" @click="handleRemove(row)">
            修改
          </el-button>
        </template>
      </el-table-column>
    </el-table>
    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="_getListData" />

    <el-dialog
      title="添加"
      :visible.sync="dialogByEditSkuIsVisible"
      :width="isDesktop==true?'800px':'90%'"
    >
      <el-form
        ref="formByEditSku"
        v-loading="dialogByEditSkuIsLoading"
        :model="formByEditSku"
        :rules="rulesEditSku"
        label-width="120px"
      >
        <el-form-item label="商品搜索">
          <el-autocomplete
            v-model="searchNameBySku"
            style="width:300px"
            :fetch-suggestions="searchAsyncBySku"
            placeholder="名称"
            @select="selectBySku"
          />
        </el-form-item>

        <el-form-item label="商品名称">
          <span>{{ formByEditSku.name }}</span>
        </el-form-item>
        <el-form-item label="商品编码">
          <span>{{ formByEditSku.cumCode }}</span>
        </el-form-item>
        <el-form-item label="会员价" prop="memberPrice">
          <el-input v-model="formByEditSku.memberPrice" clearable style="width:160px">
            <template slot="prepend">￥</template>
          </el-input>
        </el-form-item>
        <el-form-item label="有效期" prop="validDate">
          <el-date-picker
            v-model="formByEditSku.validDate"
            type="daterange"
            range-separator="至"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
            value-format="yyyy-MM-dd"
            style="width: 400px"
          />
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button type="primary" @click="handelEditSku">保存</el-button>
        <el-button @click="dialogByEditSkuIsVisible=false">关闭</el-button>
      </span>
    </el-dialog>

  </div>
</template>

<script>

import { MessageBox } from 'element-ui'
import { getSkus, editSku } from '@/api/memberright'
import { searchSku } from '@/api/product'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination
import { getUrlParam, isEmpty } from '@/utils/commonUtil'

export default {
  name: 'ManagePaneSku',
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
      formByEditSku: {
        name: '',
        cumCode: '',
        levelStId: '',
        quantity: 0,
        memberPrice: 0,
        validDate: []
      },
      rulesEditSku: {
        name: [{ required: true, min: 1, max: 200, message: '请选择优惠券', trigger: 'change' }]
      },
      searchNameBySku: '',
      dialogByEditSkuIsVisible: false,
      dialogByEditSkuIsLoading: false,
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
      getSkus(this.listQuery).then(res => {
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
    handleEdit(item) {
      MessageBox.confirm('确定要移除该优惠券？', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        editSku({ }).then(res => {
          this.$message(res.message)
          if (res.result === 1) {
            this._getListData()
          }
        })
      })
    },
    handelEditSku() {
      this.$refs['formByEditSku'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            editSku(this.formByEditSku).then(res => {
              this.$message(res.message)
              if (res.result === 1) {
                this.dialogByEditSkuIsVisible = false
                this._getListData()
              }
            })
          }).catch(() => {
          })
        }
      })
    },
    handleOpenDialogByEditSku(item) {
      this.dialogByEditSkuIsVisible = true
      this.searchNameBySku = ''
      this.formByEditSku.name = ''
      this.formByEditSku.cumCode = ''
      this.formByEditSku.memberPrice = 0
      this.formByEditSku.validDate = []
    },
    searchAsyncBySku(queryString, cb) {
      console.log('queryString:' + queryString)
      searchSku({ key: queryString }).then(res => {
        if (res.result === 1) {
          var d = res.data
          var restaurants = []
          for (var j = 0; j < d.length; j++) {
            restaurants.push({
              value: d[j].name,
              id: d[j].id,
              name: d[j].name,
              cumCode: d[j].cumCode
            })
          }

          cb(restaurants)
        }
      })
    },
    selectBySku(item) {
      console.log(JSON.stringify(item))
      this.formByEditSku.levelStId = this.levelstId
      this.formByEditSku.name = item.name
      this.formByEditSku.cumCode = item.cumCode
    }
  }
}
</script>
