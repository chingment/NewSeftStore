<template>
  <div id="store_shop">
    <div class="circle-status-bar">
      <div class="circle-item"> <span class="icon-status icon-status-1" /> <span class="name">关闭</span></div>
      <div class="circle-item"> <span class="icon-status icon-status-2" /> <span class="name">正常</span></div>
    </div>
    <el-row v-loading="loading" :gutter="20">
      <el-col v-for="item in listData" :key="item.id" :xs="24" :sm="12" :lg="8" :xl="6" class="my-col">
        <el-card class="box-card">
          <div slot="header" class="it-header clearfix">
            <div class="left">
              <div class="circle-item"> <span :class="'icon-status icon-status-'+item.status.value" /> <span class="name">{{ item.name }}</span>          <span style="font-size:12px;"> ({{ item.status.text }})</span></div>
            </div>
            <div class="right">

              <el-button type="text" @click="handleRemoveShop(item)">移除</el-button>

            </div>
          </div>
          <div class="it-component">
            <div class="img"> <img :src="item.mainImgUrl" alt=""> </div>
            <div class="describe">
              <ul>
                <li v-if="item.stcMode.indexOf('K')>-1"><el-button type="text" @click="dialogOpenByMachine(item)">({{ item.machineCount }}台)机器</el-button></li>
                <!-- <li><el-button type="text" style="color:#67c23a" @click="handleViewStock(item)">订单信息</el-button></li> -->
              </ul>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :xs="24" :sm="12" :lg="8" :xl="6" class="my-col">
        <el-card class="box-card">
          <div slot="header" class="it-header clearfix">
            <div class="left" />

          </div>
          <div class="it-component" @click="dialogOpenByShop">
            <div style="margin:auto;height:120px !important;width:120px !important; line-height:125px;" class="el-upload el-upload--picture-card"><i data-v-62e19c49="" class="el-icon-plus" /></div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <el-dialog v-if="dialogByMachineIsVisible" :title="'机器管理'" width="800px" :visible.sync="dialogByMachineIsVisible" @close="getListData(listQuery)">
      <div style="width:100%;height:600px">
        <manage-pane-machine op-code="bindshop" :store-id="storeId" :shop-id="shopId" />
      </div>
    </el-dialog>

    <el-dialog v-if="dialogByShopIsVisible" :title="'选择门店'" width="800px" :visible.sync="dialogByShopIsVisible">
      <div style="width:100%;height:600px">
        <manage-pane-shop op-code="select" :store-id="storeId" :select-method="handleAddShop" />
      </div>
    </el-dialog>

  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { initManageShop, getShops, addShop, removeShop } from '@/api/store'
import { getUrlParam, isEmpty } from '@/utils/commonUtil'
import { all } from 'q'
import managePaneShop from '@/views/shop/select'
import managePaneMachine from '@/views/machine/list'
export default {
  name: 'ManagePaneMachine',
  components: { managePaneShop, managePaneMachine },
  props: {
    storeId: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      loading: false,
      listQuery: {
        page: 1,
        limit: 10,
        name: undefined
      },
      listData: [],
      listDataByMachine: [],
      shopId: '',
      dialogByShopIsVisible: false,
      dialogByMachineIsVisible: false
    }
  },
  watch: {
    storeId: function(val, oldval) {
      this.init()
    }
  },
  mounted() {

  },
  created() {
    this.init()
  },
  methods: {
    init() {
      if (!isEmpty(this.storeId)) {
        this.loading = true
        this.listQuery.storeId = this.storeId
        initManageShop({ id: this.storeId }).then(res => {
          if (res.result === 1) {
            var d = res.data
          }
          this.loading = false
        })
        this.getListData(this.listQuery)
      }
    },
    getListData(listQuery) {
      this.loading = true
      // this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: listQuery })
      getShops(listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
        }
        this.loading = false
      })
    },
    dialogOpenByShop() {
      this.dialogByShopIsVisible = true
    },
    dialogOpenByMachine(item) {
      this.shopId = item.id
      this.dialogByMachineIsVisible = true
    },
    handleAddShop(item) {
      MessageBox.confirm('确定要选择该门店？', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        addShop({ shopId: item.id, storeId: this.storeId }).then(res => {
          this.$message(res.message)
          if (res.result === 1) {
            this.dialogByShopIsVisible = false
            this.getListData(this.listQuery)
          }
        })
      })
    },
    handleRemoveShop(item) {
      MessageBox.confirm('确定要移除该门店？', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        removeShop({ shopId: item.id, storeId: this.storeId }).then(res => {
          this.$message(res.message)
          if (res.result === 1) {
            this.dialogByShopIsVisible = false
            this.getListData(this.listQuery)
          }
        })
      })
    }
  }
}
</script>

<style lang="scss" scoped>

#store_shop{

.bm-view {
  width: 100%;
  height: 200px;
  margin-top: 20px;
}

  .it-header{
    display: flex;
    justify-content: flex-start;
    align-items: center;
    position: relative;
    height:20px ;
    .left{
      flex: 1;
      justify-content: flex-start;
      align-items: center;
      display: block;
      height: 100%;
    overflow: hidden;
text-overflow:ellipsis;
white-space: nowrap;
    .name{
    padding: 0px 5px;
    }
    }
    .right{
      width: 100px;
      display: flex;
      justify-content: flex-end;
      align-items: center;
    }

  }
  .it-component{
    min-height: 100px;
    display: flex;
    .img{
      width: 120px;
      height: 120px;

      img{
        width: 100%;
        height: 100%;
      }
    }

    .describe{
      flex: 1;
      padding: 5px;
      font-size: 12px;

      ul{
        padding: 0px;
        margin: 0px;
        list-style: none;
         li{
           width: 100%;
             text-align: right;
        height: 26px;
        line-height: 26px;
      }
      }
    }
  }
}
</style>
