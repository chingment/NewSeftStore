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

              <el-button type="text" @click="onRemoveShop(item)">移除</el-button>

            </div>
          </div>
          <div class="it-component">
            <div class="img"> <img :src="item.mainImgUrl" alt=""> </div>
            <div class="describe">
              <ul>
                <li><el-button type="text" @click="onDialogOpenByDevice(item)">({{ item.deviceCount }}台)设备</el-button></li>
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
          <div class="it-component" @click="onDialogOpenByShop">
            <div style="margin:auto;height:120px !important;width:120px !important; line-height:125px;" class="el-upload el-upload--picture-card"><i data-v-62e19c49="" class="el-icon-plus" /></div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <el-dialog v-if="dialogByDeviceIsVisible" :title="'设备管理'" width="800px" :visible.sync="dialogByDeviceIsVisible" @close="onGetList(listQuery)">
      <div style="width:100%;height:600px">
        <pane-device-bind :store-id="storeId" :shop-id="shopId" />
      </div>
    </el-dialog>

    <el-dialog v-if="dialogByShopIsVisible" :title="'选择门店'" width="800px" :visible.sync="dialogByShopIsVisible">
      <div style="width:100%;height:600px">
        <pane-shop-bind :store-id="storeId" :select-method="onAddShop" />
      </div>
    </el-dialog>

  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { initManageShop, getShops, addShop, removeShop } from '@/api/store'
import { isEmpty } from '@/utils/commonUtil'
import PaneShopBind from './PaneShopBind'
import PaneDeviceBind from './PaneDeviceBind'
export default {
  name: 'StorePaneShop',
  components: { PaneShopBind, PaneDeviceBind },
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
      listDataByDevice: [],
      shopId: '',
      dialogByShopIsVisible: false,
      dialogByDeviceIsVisible: false
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
            // var d = res.data
          }
          this.loading = false
        })
        this.onGetList(this.listQuery)
      }
    },
    onGetList(listQuery) {
      this.loading = true
      getShops(listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
        }
        this.loading = false
      })
    },
    onDialogOpenByShop() {
      this.dialogByShopIsVisible = true
    },
    onDialogOpenByDevice(item) {
      this.shopId = item.id
      this.dialogByDeviceIsVisible = true
    },
    onAddShop(item) {
      MessageBox.confirm('确定要选择该门店？', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        addShop({ shopId: item.id, storeId: this.storeId }).then(res => {
          if (res.result === 1) {
            this.$message({
              message: res.message,
              type: 'success'
            })
            this.dialogByShopIsVisible = false
            this.onGetList(this.listQuery)
          } else {
            this.$message({
              message: res.message,
              type: 'error'
            })
          }
        })
      })
    },
    onRemoveShop(item) {
      MessageBox.confirm('确定要移除该门店？', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        removeShop({ shopId: item.id, storeId: this.storeId }).then(res => {
          if (res.result === 1) {
            this.$message({
              message: res.message,
              type: 'success'
            })
            this.dialogByShopIsVisible = false
            this.onGetList(this.listQuery)
          } else {
            this.$message({
              message: res.message,
              type: 'error'
            })
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
