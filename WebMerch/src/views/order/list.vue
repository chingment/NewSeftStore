<template>
  <div id="productsku_list" class="app-container">
    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :span="4" :xs="24" style="margin-bottom:20px">
          <el-input v-model="listQuery.orderSn" placeholder="订单号" va style="width: 100%" class="filter-item" @keyup.enter.native="handleFilter" />
        </el-col>
        <el-col v-if="isShowClientUserNameInput" :span="4" :xs="24" style="margin-bottom:20px">
          <el-input v-model="listQuery.clientUserName" placeholder="下单用户" va style="width: 100%" class="filter-item" @keyup.enter.native="handleFilter" />
        </el-col>
        <el-col :span="4" :xs="24" style="margin-bottom:20px">
          <el-select v-model="listQuery.orderStauts" clearable placeholder="全部状态" style="width: 100%">
            <el-option
              v-for="item in options_status"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-col>
        <el-col :span="4" :xs="24" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter">
            查询
          </el-button>
        </el-col>
      </el-row>
      <!-- // <el-input v-model="listQuery.orderSn" placeholder="订单号" va style="width: 200px;" class="filter-item" @keyup.enter.native="handleFilter" />
      // <el-input v-model="listQuery.clientUserName" placeholder="下单用户" va style="width: 200px;" class="filter-item" @keyup.enter.native="handleFilter" />
      // <el-button class="filter-item" style="margin-left: 10px;" type="primary" icon="el-icon-search" @click="handleFilter">
      //   查询
      // </el-button> -->
    </div>
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
          <div v-for="(sellChannelDetail,index) in scope.row.sellChannelDetails" :key="index">
            <div><span>+ {{ sellChannelDetail.name }} -> </span></div>
            <table class="table-skus" style="width:600px">
              <tr v-for="(pickupSku,sub_index) in sellChannelDetail.detailItems" :key="sub_index">
                <td style="20%">
                  <img :src="pickupSku.mainImgUrl" style="width:50px;height:50px;">
                </td>
                <td style="20%">
                  {{ pickupSku.name }}
                </td>
                <td style="30%">
                  x {{ pickupSku.quantity }}
                </td>
                <td style="15%">
                  {{ pickupSku.status.text }}
                </td>
                <td style="15%">
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
                          <el-image v-if="activity.imgUrl!==null"
    style="width: 100px; height: 100px"
    :src="activity.imgUrl" 
    :preview-src-list="activity.imgUrls">
  </el-image>

                      </el-timeline-item>
                    </el-timeline>
                    <el-button slot="reference">查看</el-button>
                  </el-popover>

                </td>
              </tr>
            </table>
          </div>
        </template>
      </el-table-column>

      <el-table-column label="订单号" prop="sn" align="left" :width="isDesktop==true?220:80">
        <template slot-scope="scope">
          <span>{{ scope.row.sn }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="店铺" prop="storeName" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.storeName }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="下单用户" prop="clientUserName" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.clientUserName }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="下单方式" prop="sourceName" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.sourceName }}</span>
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
      <el-table-column label="状态" prop="status" align="left" min-width="15%">
        <template slot-scope="scope">
          <span>{{ scope.row.status.text }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="isDesktop" label="下单时间" prop="submitTime" align="left" min-width="15%">
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

    <el-dialog  title="订单详情" :visible.sync="dialogDetailsIsVisible" :width="isDesktop==true?'800px':'90%'">
      <div v-loading="detailsLoading" >

        <div class="row-title clearfix">
          <div class="pull-left"> <h5>基本信息</h5>  
          </div>
 <div class="pull-right">
                 <el-button  icon="el-icon-refresh" circle @click="refreshDetails(details.id)" ></el-button>
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
        <div v-for="(sellChannelDetail,index) in details.sellChannelDetails" :key="index">
          <div><span>+ {{ sellChannelDetail.name }} -> </span></div>
          <table class="table-skus" style="width:600px">
            <tr v-for="(pickupSku,sub_index) in sellChannelDetail.detailItems" :key="sub_index">
              <td style="20%">
                <img :src="pickupSku.mainImgUrl" style="width:50px;height:50px;">
              </td>
              <td style="20%">
                {{ pickupSku.name }}
              </td>
              <td style="30%">
                x {{ pickupSku.quantity }}
              </td>
              <td style="15%">
                {{ pickupSku.status.text }}
              </td>
              <td style="15%">
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
                          <el-image v-if="activity.imgUrl!==null"
    style="width: 100px; height: 100px"
    :src="activity.imgUrl" 
    :preview-src-list="activity.imgUrls">
  </el-image>

                      </el-timeline-item>
                    </el-timeline>
                    <el-button slot="reference">查看</el-button>
                  </el-popover>

              </td>
            </tr>
          </table>
        </div>
      </div>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogDetailsIsVisible = false">
          关闭
        </el-button>
          <el-button type="primary" @click="refreshDetails(details.id)">刷新</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { getList,getDetails } from '@/api/order'
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
    },
    clientuserid: {
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
      dialogDetailsIsVisible: false,
      detailsLoading:false,
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
      isDesktop: this.$store.getters.isDesktop,
      isShowClientUserNameInput: true
    }
  },
  watch: {
    storeid: function(value) {
      this.listQuery.storeId = value
      this.init()
    },
    machineid: function(value) {
      this.listQuery.machineId = value
      this.init()
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }

    this.listQuery.storeId = this.storeid
    this.listQuery.machineId = this.machineid

    if (this.clientuserid === '') {
      this.isShowClientUserNameInput = true
    } else {
      this.isShowClientUserNameInput = false
    }
    this.listQuery.clientUserId = this.clientuserid
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
    refreshDetails(id){
      this.detailsLoading = true
      getDetails({id:id}).then(res => {
        if (res.result === 1) {
           this.details = res.data
        }
        this.detailsLoading = false
      })
    },
    dialogDetailsOpen(row) {
      this.detailsLoading = true
      getDetails({id:row.id}).then(res => {
        if (res.result === 1) {
           this.details = res.data
        }
        this.detailsLoading = false
        this.dialogDetailsIsVisible = true
      })
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
