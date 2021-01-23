<template>
  <div id="product_storesale">
    <el-table
      :key="listKey"
      v-loading="loading"
      :data="listData"
      fit
      highlight-current-row
      style="width: 100%;"
    >
      <el-table-column label="店铺" prop="sn" align="left" :width="isDesktop==true?220:80">
        <template slot-scope="scope">
          <span>{{ scope.row.storeName }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="商品" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.productSkuName }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="下架？" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.productSkuIsOffSell==true?"是":"否" }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="销售价" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.productSkuSalePrice }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" fixed="right" align="center" width="80" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button type="primary" size="mini" @click="dialogEditOpen(row)">
            编辑
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-dialog title="店铺商品编辑" :visible.sync="dialogEditIsVisible" :width="isDesktop==true?'500px':'90%'">

      <el-form ref="form" :model="form" :rules="rules" label-width="80px">
        <el-form-item label="店铺">
          <span>{{ form.storeName }}</span>
        </el-form-item>
        <el-form-item label="商品">
          <span>{{ form.productSkuName }}</span>
        </el-form-item>
        <el-form-item label="图片">
          <img
            :src="form.productSkuMainImgUrl"
            alt=""
            style="width:100px;height:100px"
          >
        </el-form-item>
        <el-form-item label="销售价" prop="productSkuSalePrice">
          <el-input v-model="form.productSkuSalePrice" style="width:160px" class="ip-prepend">
            <template slot="prepend">￥</template>
          </el-input>
        </el-form-item>
        <el-form-item label="下架">
          <el-checkbox v-model="form.productSkuIsOffSell" />
        </el-form-item>
      </el-form>

      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogEditIsVisible = false">
          取消
        </el-button>
        <el-button type="primary" @click="handleEdit">
          确定
        </el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { getOnSaleStores, editSalePriceOnStore } from '@/api/prdproduct'
import fromReg from '@/utils/formReg'
export default {
  props: {
    productId: {
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
        id: undefined
      },
      dialogEditIsVisible: false,
      rules: {
        productSkuSalePrice: [{ required: true, message: '金额格式,eg:88.88', pattern: fromReg.money }]
      },
      form: {
        storeId: '',
        storeName: '',
        productSkuId: '',
        productSkuName: '',
        productSkuMainImgUrl: '',
        productSkuSalePrice: 0,
        productSkuIsOffSell: false
      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    this.listQuery.id = this.productId
    this.init()
  },
  methods: {
    init() {
      this.getListData()
    },
    getListData() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      getOnSaleStores(this.listQuery).then(res => {
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
            editSalePriceOnStore(this.form).then(res => {
              this.$message(res.message)
              if (res.result === 1) {
                this.dialogEditIsVisible = false
                this.getListData()
              }
            })
          }).catch(() => {
          })
        }
      })
    },
    dialogEditOpen(row) {
      this.dialogEditIsVisible = true
      this.form.storeId = row.storeId
      this.form.storeName = row.storeName
      this.form.productSkuId = row.productSkuId
      this.form.productSkuMainImgUrl = row.productSkuMainImgUrl
      this.form.productSkuName = row.productSkuName
      this.form.productSkuSalePrice = row.productSkuSalePrice
      this.form.productSkuIsOffSell = row.productSkuIsOffSell
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
