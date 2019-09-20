<template>
  <div id="productsku_list" class="app-container">
    <div class="filter-container">
      <el-input v-model="listQuery.orderSn" placeholder="订单号" va style="width: 200px;" class="filter-item" @keyup.enter.native="handleFilter" />
      <el-input v-model="listQuery.clientUserName" placeholder="下单用户" va style="width: 200px;" class="filter-item" @keyup.enter.native="handleFilter" />
      <el-button class="filter-item" style="margin-left: 10px;" type="primary" icon="el-icon-search" @click="handleFilter">
        查询
      </el-button>
    </div>
    <el-table
      :key="listKey"
      v-loading="loading"
      :data="listData"
      border
      fit
      highlight-current-row
      style="width: 100%;"
    >
      <el-table-column type="expand">
        <template slot-scope="scope">
          <div v-for="(detail,index) in scope.row.details" :key="index">
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
        </template>
      </el-table-column>

      <el-table-column label="订单号" prop="sn" align="left" width="220">
        <template slot-scope="scope">
          <span>{{ scope.row.sn }}</span>
        </template>
      </el-table-column>
      <el-table-column label="店铺" prop="storeName" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.storeName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="下单用户" prop="clientUserName" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.clientUserName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="下单方式" prop="sourceName" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.sourceName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="数量" prop="quantity" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.quantity }}</span>
        </template>
      </el-table-column>
      <el-table-column label="支付金额" prop="chargeAmount" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.chargeAmount }}</span>
        </template>
      </el-table-column>
      <el-table-column label="状态" prop="status" align="left" min-width="15%">
        <template slot-scope="scope">
          <span>{{ scope.row.status.text }}</span>
        </template>
      </el-table-column>
      <el-table-column label="下单时间" prop="submitTime" align="left" min-width="15%">
        <template slot-scope="scope">
          <span>{{ scope.row.submitTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" fixed="right" align="center" width="80" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button type="primary" size="mini" @click="dialogDetailsOpen(row)">
            查看
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />

    <el-dialog title="订单详情" :visible.sync="dialogDetailsIsVisible" width="800px">
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
                <el-col :span="8">
                  <el-form-item label-width="80px" label="下单用户:" class="postInfo-container-item">
                    {{ details.clientUserName }}
                  </el-form-item>
                </el-col>

                <el-col :span="8">
                  <el-form-item label-width="80px" label="下单方式:" class="postInfo-container-item">
                    {{ details.sourceName }}
                  </el-form-item>
                </el-col>

                <el-col :span="8">
                  <el-form-item label-width="80px" label="下单时间:" class="postInfo-container-item">
                    {{ details.submitTime }}
                  </el-form-item>
                </el-col>
              </el-row>
              <el-row>
                <el-col :span="8">
                  <el-form-item label-width="80px" label="原金额:" class="postInfo-container-item">
                    {{ details.originalAmount }}
                  </el-form-item>
                </el-col>

                <el-col :span="8">
                  <el-form-item label-width="80px" label="优惠金额:" class="postInfo-container-item">
                    {{ details.discountAmount }}
                  </el-form-item>
                </el-col>

                <el-col :span="8">
                  <el-form-item label-width="80px" label="支付金额:" class="postInfo-container-item">
                    {{ details.chargeAmount }}
                  </el-form-item>
                </el-col>
              </el-row>
              <el-row>
                <el-col :span="8">
                  <el-form-item label-width="80px" label="状态:" class="postInfo-container-item">
                    {{ details.status.text }}
                  </el-form-item>
                </el-col>
                <el-col :span="8" />
                <el-col :span="8" />
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
import { getList } from '@/api/order'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination

export default {
  name: 'OrderList',
  components: { Pagination },
  props: {
    storeid: {
      type: String,
      require: false,
      default: ''
    },
    machineid: {
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
        clientName: undefined,
        orderSn: undefined,
        storeId: undefined,
        machineId: undefined
      },
      isDesktop: this.$store.getters.isDesktop,
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
      }
    }
  },
  watch: {
    storeid: function(value) {
      this.listQuery.storeId = value
      console.log('this.listQuery.storeId 2 :' + this.listQuery.storeId)
      this.init()
    },
    machineid: function(value) {
      this.listQuery.machineId = value
      console.log('this.listQuery.machineId 2 :' + this.listQuery.machineId)
      this.init()
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }

    this.listQuery.storeId = this.storeid
    this.listQuery.machineId = this.machineid
    console.log('this.listQuery.storeId 1 :' + this.listQuery.storeId)
    console.log('this.listQuery.machineid 1 :' + this.listQuery.machineid)
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
