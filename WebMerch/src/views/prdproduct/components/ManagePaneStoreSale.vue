<template>
  <div id="productsku_list" class="app-container">
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
      <el-table-column v-if="isDesktop" label="商品" prop="storeName" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.productSkuName }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="销售价" prop="clientUserName" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.productSkuSalePrice }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" fixed="right" align="center" width="80" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button type="primary" size="mini" @click="dialogDetailsOpen(row)">
            编辑
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-dialog title="商品详情" :visible.sync="dialogDetailsIsVisible" :width="isDesktop==true?'800px':'90%'">
      <div>
        <div class="row-title clearfix">
          <div class="pull-left"> <h5>基本信息</h5>
          </div>
        </div>
        <el-form class="form-container" style="display:flex">
          <el-col :span="24">

            <div class="postInfo-container">
              <el-row>
                <el-col :span="24">
                  <el-form-item label-width="80px" label="订单编号:" class="postInfo-container-item">
                    {{ details.sn }}
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
                    {{ details.submitTime }}
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
        <div v-for="(detail,index) in details.details" :key="index">
          <div><span>+ {{ detail.sellChannelRefName }} -> </span></div>
          <table class="table-skus" style="width:600px">
            <tr v-for="(sub_detail,sub_index) in detail.detials" :key="sub_index">
              <td style="20%">
                <img :src="sub_detail.prdProductSkuMainImgUrl" style="width:50px;height:50px;">
              </td>
              <td style="20%">
                {{ sub_detail.prdProductSkuName }}
              </td>
              <td style="30%">
                x {{ sub_detail.quantity }}
              </td>
              <td style="30%">
                {{ sub_detail.status.text }}
              </td>
            </tr>
          </table>
        </div>
      </div>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogDetailsIsVisible = false">
          关闭
        </el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { getOnSaleStores } from '@/api/prdproduct'

export default {
  props: {
    productid: {
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
      dialogDetailsIsVisible: false,
      details: {
        sn: '',
        storeName: '',
        clientUserName: '',
        sourceName: '',
        quantity: '',
        originalAmount: '',
        discountAmount: '',
        chargeAmount: '',
        submitTime: '',
        status: { text: '' },
        details: undefined
      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    this.listQuery.id = this.productid
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
    dialogDetailsOpen(row) {
      this.details = row
      this.dialogDetailsIsVisible = true
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
