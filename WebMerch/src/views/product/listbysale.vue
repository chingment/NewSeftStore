<template>
  <div id="product_list">

    <div class="filter-container">
      <el-row :gutter="24">
        <el-col :xs="24" :sm="12" :lg="8" :xl="6" style="margin-bottom:20px">

          <el-autocomplete
            v-model="listQuery.key"
            :fetch-suggestions="handleSpuSrh"
            placeholder="商品名称/编码/条形码/首拼音母"
            clearable
            style="width: 100%"
            @select="handleSpuSel"
            @keyup.enter.native="handleFilter"
            @clear="handleFilter"
          />

        </el-col>
        <el-col :xs="24" :sm="12" :lg="8" :xl="6" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter">
            查询
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
    >
      <el-table-column label="序号" prop="id" fixed="left" align="left" width="80">
        <template slot-scope="scope">
          <span>{{ scope.$index+1 }} </span>
        </template>
      </el-table-column>
      <el-table-column label="图片" prop="mainImgUrl" fixed="left" align="center" width="110">
        <template slot-scope="scope">
          <img :src="scope.row.mainImgUrl" style="width:80px;height:80px;">
        </template>
      </el-table-column>
      <el-table-column label="店铺" prop="sn" align="left" width="180">
        <template slot-scope="scope">
          <span>{{ scope.row.storeName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="名称" align="left" min-width="100%">
        <template slot-scope="scope">
          <span>{{ scope.row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column label="编码" width="180">
        <template slot-scope="scope">
          <span>{{ scope.row.cumCode }}</span>
        </template>
      </el-table-column>
      <el-table-column label="下架？" align="left" width="180">
        <template slot-scope="scope">
          <span>{{ scope.row.isOffSell==true?"是":"否" }}</span>
        </template>
      </el-table-column>
      <el-table-column label="销售价" align="left" width="180">
        <template slot-scope="scope">
          <span>{{ scope.row.salePrice }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" fixed="right" align="center" width="80" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button type="text" size="mini" @click="dialogEditOpen(row)">
            编辑
          </el-button>
        </template>
      </el-table-column>
    </el-table>
    <pagination v-show="listTotal>0" :total="listTotal" background :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />
    <el-dialog title="店铺商品编辑" :visible.sync="dialogEditIsVisible" :width="isDesktop==true?'600px':'90%'">

      <el-form ref="form" :model="form" :rules="rules" label-width="80px">
        <el-form-item label="店铺">
          <span>{{ form.storeName }}</span>
        </el-form-item>
        <el-form-item label="商品">
          <span>{{ form.name }}</span>
        </el-form-item>
        <el-form-item label="图片">
          <img
            :src="form.mainImgUrl"
            alt=""
            style="width:100px;height:100px"
          >
        </el-form-item>
        <el-form-item label="销售价" prop="salePrice">
          <el-input v-model="form.salePrice" style="width:160px" class="ip-prepend">
            <template slot="prepend">￥</template>
          </el-input>
        </el-form-item>
        <el-form-item label="下架">
          <el-checkbox v-model="form.isOffSell" />
        </el-form-item>
      </el-form>

      <div slot="footer" class="dialog-footer">
        <el-button size="small" @click="dialogEditIsVisible = false">
          取消
        </el-button>
        <el-button size="small" type="primary" @click="handleEdit">
          确定
        </el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { getListBySale, searchSku, editSale } from '@/api/product'
import fromReg from '@/utils/formReg'
import Pagination from '@/components/Pagination'
export default {
  components: { Pagination },
  props: {
    spuId: {
      type: String,
      require: false,
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
        key: ''
      },
      dialogEditIsVisible: false,
      rules: {
        salePrice: [{ required: true, message: '金额格式,eg:88.88', pattern: fromReg.money }]
      },
      form: {
        storeId: '',
        storeName: '',
        skuId: '',
        name: '',
        mainImgUrl: '',
        salePrice: 0,
        isOffSell: false
      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    this.listQuery.id = this.spuId
    this.init()
  },
  methods: {
    init() {
      this.getListData()
    },
    getListData() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      getListBySale(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
          this.listTotal = d.total
        }
        this.loading = false
      })
    },
    handleEdit() {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            editSale(this.form).then(res => {
              if (res.result === 1) {
                this.$message({
                  message: res.message,
                  type: 'success'
                })
                this.dialogEditIsVisible = false
                this.getListData()
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
    handleFilter() {
      this.listQuery.page = 1
      this.getListData()
    },
    handleSpuSrh(queryString, cb) {
      searchSku({ key: queryString }).then(res => {
        if (res.result === 1) {
          var d = res.data
          var restaurants = []
          for (var j = 0; j < d.length; j++) {
            restaurants.push({ 'value': d[j].name, 'mainImgUrl': d[j].mainImgUrl, 'name': d[j].name, 'spuId': d[j].spuId })
          }

          cb(restaurants)
        }
      })
    },
    handleSpuSel(item) {
      this.listQuery.key = item.name
      getListBySale(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
          this.listTotal = d.total
        }
        this.loading = false
      })
    },
    dialogEditOpen(row) {
      this.dialogEditIsVisible = true
      this.form.storeId = row.storeId
      this.form.storeName = row.storeName
      this.form.skuId = row.skuId
      this.form.mainImgUrl = row.mainImgUrl
      this.form.name = row.name
      this.form.salePrice = row.salePrice
      this.form.isOffSell = row.isOffSell
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
}
</style>
