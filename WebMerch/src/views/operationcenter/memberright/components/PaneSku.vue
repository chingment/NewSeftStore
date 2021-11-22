<template>
  <div id="sku_list">
    <div class="filter-container">
      <el-row :gutter="12">
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-input v-model="listQuery.name" clearable style="width: 100%" placeholder="商品名称" class="filter-item" @keyup.enter.native="handleFilter" @clear="handleFilter" />
        </el-col>
        <el-col :span="6" :xs="24" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter">查询</el-button>
          <el-button class="filter-item" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleOpenDialogByAddSku">添加</el-button>
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
          <span>{{ scope.row.memberPrice }}</span>
        </template>
      </el-table-column>
      <el-table-column label="有效期" prop="memberPrice" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.validDate[0] }}~{{ scope.row.validDate[1] }}</span>
        </template>
      </el-table-column>
      <el-table-column label="状态" prop="status" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.status.text }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" width="160" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button type="text" size="mini" @click="handleOpenDialogByEditSku(row)">
            修改
          </el-button>
        </template>
      </el-table-column>
    </el-table>
    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="_getListData" />

    <el-dialog
      title="添加"
      :visible.sync="dialogByAddSkuIsVisible"
      :width="isDesktop==true?'800px':'90%'"
    >
      <el-form
        ref="formByAddSku"
        v-loading="dialogByAddSkuIsLoading"
        :model="formByAddSku"
        :rules="rulesByAddSku"
        label-width="120px"
      >
        <el-form-item label="商品搜索">
          <el-autocomplete
            v-model="searchNameBySku"
            style="width: 400px"
            :fetch-suggestions="searchAsyncBySku"
            placeholder="名称"
            @select="selectBySku"
          />
        </el-form-item>

        <el-form-item label="商品名称">
          <span>{{ formByAddSku.skuName }}</span>
        </el-form-item>
        <el-form-item label="商品编码">
          <span>{{ formByAddSku.skuCumCode }}</span>
        </el-form-item>
        <el-form-item label="店铺">
          <el-select v-model="formByAddSku.storeIds" multiple placeholder="选择店铺" style="width: 400px">
            <el-option
              v-for="item in optionsStores"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="会员价" prop="memberPrice">
          <el-input v-model="formByAddSku.memberPrice" clearable style="width:160px">
            <template slot="prepend">￥</template>
          </el-input>
        </el-form-item>
        <el-form-item label="有效期" prop="validDate">
          <el-date-picker
            v-model="formByAddSku.validDate"
            type="daterange"
            range-separator="至"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
            value-format="yyyy-MM-dd"
            style="width: 400px"
          />
        </el-form-item>
        <el-form-item label="禁用" prop="isDisabled">
          <el-switch v-model="formByAddSku.isDisabled" />
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button size="small" type="primary" @click="handelAddSku">保存</el-button>
        <el-button size="small" @click="dialogByAddSkuIsVisible=false">关闭</el-button>
      </span>
    </el-dialog>

    <el-dialog
      title="修改"
      :visible.sync="dialogByEditSkuIsVisible"
      :width="isDesktop==true?'800px':'90%'"
    >
      <el-form
        ref="formByEditSku"
        v-loading="dialogByEditSkuIsLoading"
        :model="formByEditSku"
        :rules="rulesByEditSku"
        label-width="120px"
      >
        <el-form-item label="商品名称">
          <span>{{ formByEditSku.skuName }}</span>
        </el-form-item>
        <el-form-item label="商品编码">
          <span>{{ formByEditSku.skuCumCode }}</span>
        </el-form-item>
        <el-form-item label="店铺">
          <span>{{ formByEditSku.storeName }}</span>
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
        <el-form-item label="禁用" prop="isDisabled">
          <el-switch v-model="formByEditSku.isDisabled" />
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button size="small" type="primary" @click="handleEditSku">保存</el-button>
        <el-button size="small" @click="dialogByEditSkuIsVisible=false">关闭</el-button>
      </span>
    </el-dialog>

  </div>
</template>

<script>

import { MessageBox } from 'element-ui'
import { getSkus, addSku, editSku } from '@/api/memberright'
import { searchSku } from '@/api/product'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination
import { isEmpty } from '@/utils/commonUtil'
import { getStores } from '@/api/common'
export default {
  name: 'OperationCenterMemberRightPaneSku',
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
      formByAddSku: {
        skuId: '',
        skuName: '',
        skuCumCode: '',
        storeIds: [],
        levelStId: '',
        memberPrice: 0,
        validDate: [],
        isDisabled: false
      },
      rulesByAddSku: {
        name: [{ required: true, min: 1, max: 200, message: '请选择优惠券', trigger: 'change' }]
      },
      formByEditSku: {
        skuId: '',
        skuName: '',
        skuCumCode: '',
        storeId: '',
        storeName: [],
        levelStId: '',
        memberPrice: 0,
        validDate: [],
        isDisabled: false
      },
      rulesByEditSku: {
        name: [{ required: true, min: 1, max: 200, message: '请选择优惠券', trigger: 'change' }]
      },
      optionsStores: [],
      searchNameBySku: '',
      dialogByAddSkuIsVisible: false,
      dialogByAddSkuIsLoading: false,
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

        getStores().then(res => {
          if (res.result === 1) {
            this.optionsStores = res.data
          }
        })
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
    handleEditSku(item) {
      this.$refs['formByEditSku'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            editSku(this.formByEditSku).then(res => {
              if (res.result === 1) {
                this.$message({
                  message: res.message,
                  type: 'success'
                })
                this.dialogByEditSkuIsVisible = false
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
    handelAddSku() {
      this.$refs['formByAddSku'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            addSku(this.formByAddSku).then(res => {
              if (res.result === 1) {
                this.$message({
                  message: res.message,
                  type: 'success'
                })
                this.dialogByAddSkuIsVisible = false
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
    handleOpenDialogByEditSku(item) {
      this.dialogByEditSkuIsVisible = true
      this.formByEditSku.skuId = item.skuId
      this.formByEditSku.storeId = item.storeId
      this.formByEditSku.levelStId = item.levelStId
      this.formByEditSku.storeName = item.storeName
      this.formByEditSku.skuName = item.skuName
      this.formByEditSku.skuCumCode = item.skuCumCode
      this.formByEditSku.memberPrice = item.memberPrice
      this.formByEditSku.validDate = item.validDate
      this.formByEditSku.isDisabled = item.isDisabled
    },
    handleOpenDialogByAddSku(item) {
      this.dialogByAddSkuIsVisible = true
      this.searchNameBySku = ''
      this.formByAddSku.skuId = ''
      this.formByAddSku.storeIds = []
      this.formByAddSku.skuName = ''
      this.formByAddSku.skuCumCode = ''
      this.formByAddSku.memberPrice = 0
      this.formByAddSku.validDate = []
    },
    searchAsyncBySku(queryString, cb) {
      searchSku({ key: queryString }).then(res => {
        if (res.result === 1) {
          var d = res.data
          var restaurants = []
          for (var j = 0; j < d.length; j++) {
            restaurants.push({
              value: d[j].name,
              id: d[j].skuId,
              name: d[j].name,
              cumCode: d[j].cumCode
            })
          }

          cb(restaurants)
        }
      })
    },
    selectBySku(item) {
      this.formByAddSku.levelStId = this.levelstId
      this.formByAddSku.skuId = item.id
      this.formByAddSku.skuName = item.name
      this.formByAddSku.skuCumCode = item.cumCode
    }
  }
}
</script>
