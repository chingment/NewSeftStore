<template>
  <div id="device_list">

    <div class="circle-status-bar">
      <div class="circle-item"> <span class="icon-status icon-status-1" /> <span class="name">关闭</span></div>
      <div class="circle-item"> <span class="icon-status icon-status-2" /> <span class="name">正常</span></div>
      <div class="circle-item"> <span class="icon-status icon-status-4" /> <span class="name">维护</span></div>
      <div class="circle-item"> <span class="icon-status icon-status-3" /> <span class="name">异常</span></div>
    </div>
    <div class="filter-container">

      <el-row :gutter="24">
        <el-col :xs="24" :sm="12" :lg="8" :xl="span" style="margin-bottom:20px">
          <el-input v-model="listQuery.id" clearable style="width: 100%" placeholder="设备编号" class="filter-item" />
        </el-col>
        <el-col :xs="24" :sm="12" :lg="8" :xl="span" style="margin-bottom:20px;display:flex;">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="onFilter">
            查询
          </el-button>
          <el-button v-if="opCode==='bindshop'" class="filter-item" type="primary" icon="el-icon-plus" @click="onDialogByOpenSelect(false,null)">
            添加
          </el-button>
        </el-col>
      </el-row>
    </div>
    <el-row v-loading="loading" :gutter="24">

      <el-col v-for="item in listData" v-show="deviceCount!==0" :key="item.id" :xs="24" :sm="12" :lg="8" :xl="span" class="my-col">
        <el-card class="box-card">
          <div slot="header" class="it-header clearfix">
            <div class="left">

              <div class="circle-item"> <span :class="'icon-status icon-status-'+item.status.value" /> <span class="name">{{ item.code }} <span style="font-size:12px;"> ({{ item.status.text }})</span></span></div>

            </div>
            <div class="right">
              <el-button v-if="opCode==='list'" type="text" @click="onManage(item)">管理</el-button>
              <el-button v-if="opCode==='bindshop'" type="text" @click="onUnBindShop(item)">解绑</el-button>
            </div>
          </div>
          <div class="storeName" style="font-size:12px;white-space: nowrap">{{ item.shopName }} [{{ item.lastRequestTime }}]</div>

          <div class="it-component">

            <div class="img"> <img :src="item.mainImgUrl" alt=""> </div>
            <div class="describe">
              <ul v-if="opCode==='list'">
                <li><el-button type="text" style="padding:0px;color:#67c23a" @click="onStock(item)">库存查看</el-button></li>
                <li><el-button type="text" style="padding:0px;color:#f38b3f" @click="onControlCenter(item)">控制中心</el-button></li>
              </ul>
            </div>
          </div>
        </el-card>
      </el-col>

      <el-col v-show="deviceCount===0" :xs="24" :sm="12" :lg="8" :xl="6" style="margin-bottom:20px">
        <el-card class="box-card">
          <div slot="header" class="it-header clearfix">
            <div class="left" />
            <el-button type="text">暂无设备，请联系您的客户经理绑定！</el-button>
          </div>
          <div class="it-component">

            <div style="margin:auto;height:120px !important;width:120px !important; line-height:125px;" class="el-upload el-upload--picture-card"><i data-v-62e19c49="" class="el-icon-plus" /></div>

          </div>
        </el-card>
      </el-col>

    </el-row>
    <div v-show="listData.length<=0&&deviceCount>0" class="list-empty">
      <span>暂无数据</span>
    </div>
    <el-dialog v-if="dialogByOpenSelectIsVisible" :title="'选择设备'" width="600px" :visible.sync="dialogByOpenSelectIsVisible" append-to-body>
      <div style="width:100%;height:400px">

        <div class="filter-container">
          <el-row :gutter="16">
            <el-col :span="8" :xs="24" style="margin-bottom:20px">
              <el-input v-model="listQueryBySelect.id" clearable style="width: 100%" placeholder="编号" class="filter-item" />
            </el-col>
            <el-col :span="8" :xs="24" style="margin-bottom:20px">
              <el-button class="filter-item" type="primary" icon="el-icon-search" @click="onFilterBySelect">
                查询
              </el-button>
            </el-col>
          </el-row>

        </div>
        <el-table
          :key="listKey"
          v-loading="loadingBySelect"
          :data="listDataBySelect"
          fit
          highlight-current-row
          style="width: 100%;"
        >
          <el-table-column label="序号" prop="id" align="left" width="80">
            <template slot-scope="scope">
              <span>{{ scope.$index+1 }} </span>
            </template>
          </el-table-column>
          <el-table-column label="编号" align="left" min-width="45%">
            <template slot-scope="scope">
              <span>{{ scope.row.code }}</span>
            </template>
          </el-table-column>
          <el-table-column label="门店" align="left" min-width="55%">
            <template slot-scope="scope">
              <span>{{ scope.row.shopName }}</span>
            </template>
          </el-table-column>
          <el-table-column label="操作" align="right" width="100" class-name="small-padding fixed-width">
            <template slot-scope="{row}">
              <template v-if="opCode==='bindshop'">
                <el-button v-if="row.isCanSelect" type="primary" size="mini" @click="onSelect(row)">
                  选择
                </el-button>
                <el-button v-else type="text" disabled>{{ row.opTips }}</el-button>
              </template>
            </template>
          </el-table-column>
        </el-table>

      </div>
    </el-dialog>
  </div>
</template>

<script>

import { MessageBox } from 'element-ui'
import { getList, initGetList, bindShop, unBindShop } from '@/api/devvending'

export default {
  name: 'VendingList',
  props: {
    opCode: {
      type: String,
      default: 'list'
    },
    storeId: {
      type: String,
      default: ''
    },
    shopId: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      loading: true,
      listKey: 'listQuery',
      listKeyBySelect: 'listQueryBySelectss',
      listQuery: {
        opCode: 'list',
        page: 1,
        limit: 1000,
        shopId: '',
        storeId: '',
        id: '',
        type: 'vending'
      },
      loadingBySelect: false,
      listQueryBySelect: {
        opCode: 'bindshop',
        page: 1,
        limit: 10,
        shopId: '',
        storeId: '',
        id: ''
      },
      dialogByOpenSelectIsVisible: false,
      deviceCount: 0,
      listData: [],
      listDataBySelect: [],
      span: 6,
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }
    this.span = this.opCode === 'list' ? 6 : 12
    this.init()
  },
  methods: {
    init() {
      this.loading = true
      this.listQuery.storeId = this.storeId
      this.listQuery.shopId = this.shopId

      if (this.opCode === 'bindshop') {
        this.listQuery.opCode = 'listbyshop'
      } else {
        this.listQuery.opCode = 'list'
      }

      initGetList().then(res => {
        if (res.result === 1) {
          var d = res.data
          this.deviceCount = d.deviceCount

          if (d.deviceCount > 0) {
            this.onGetList()
          }
        }
        this.loading = false
      })
    },
    onGetList() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      getList(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
        }
        this.loading = false
      })
    },
    onFilter() {
      this.listQuery.page = 1
      this.onGetList()
    },
    onManage(item) {
      this.$router.push({
        name: 'MerchDeviceManage',
        path: '/device/vending/manage',
        params: {
          id: item.id,
          tab: 'tabBaseInfo'
        }
      })
    },
    onStock(item) {
      this.$router.push({
        name: 'MerchDeviceManage',
        path: '/device/vending/manage',
        params: {
          id: item.id,
          tab: 'tabStock'
        }
      })
    },
    onControlCenter(item) {
      this.$router.push({
        name: 'MerchDeviceManage',
        path: '/device/vending/manage',
        params: {
          id: item.id,
          tab: 'tabControlCenter'
        }
      })
    },
    onGetListBySelect() {
      this.listQueryBySelect.storeId = this.storeId
      this.listQueryBySelect.shopId = this.shopId
      this.listQueryBySelect.opCode = 'listbyunbindshop'
      this.loadingBySelect = true
      getList(this.listQueryBySelect).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listDataBySelect = d.items
        }
        this.loadingBySelect = false
      })
    },
    onFilterBySelect() {
      this.listQueryBySelect.page = 1
      this.onGetListBySelect()
    },
    onDialogByOpenSelect() {
      this.dialogByOpenSelectIsVisible = true
      this.onGetListBySelect()
    },
    onUnBindShop: function(item) {
      MessageBox.confirm('确定要解绑该设备，解绑后库存将被清空，请谨慎操作！', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        unBindShop({ deviceId: item.id, storeId: item.storeId, shopId: item.shopId }).then(res => {
          if (res.result === 1) {
            this.$message({
              message: res.message,
              type: 'success'
            })
            this.onGetList()
          } else {
            this.$message({
              message: res.message,
              type: 'error'
            })
          }
        })
      })
    },
    onSelect: function(item) {
      if (this.opCode === 'bindshop') {
        MessageBox.confirm('确定绑定设备', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
          bindShop({ deviceId: item.id, storeId: this.storeId, shopId: this.shopId }).then(res => {
            if (res.result === 1) {
              this.$message({
                message: res.message,
                type: 'success'
              })
              this.onGetList()
              this.dialogByOpenSelectIsVisible = false
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
}
</script>

<style lang="scss" scoped>

#device_list{

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
padding: 0 5px;
    display: inline-block;
    flex: 1;
    overflow: hidden;
    white-space: nowrap;
    text-overflow: ellipsis
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
      padding: 0px;
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
