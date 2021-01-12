<template>
  <div id="store_list" class="app-container">
    <div class="circle-status-bar">
      <div class="circle-item"> <span class="icon-status icon-status-1" /> <span class="name">关闭</span></div>
      <div class="circle-item"> <span class="icon-status icon-status-2" /> <span class="name">正常</span></div>
    </div>
    <el-row v-loading="loading" :gutter="20">
      <el-col v-for="item in listData" :key="item.id" :span="6" :xs="24" style="margin-bottom:20px">
        <el-card class="box-card">
          <div slot="header" class="it-header clearfix">
            <div class="left">
              <div class="circle-item"> <span :class="'icon-status icon-status-'+item.status.value" /> <span class="name">{{ item.name }}</span>          <span style="font-size:12px;"> ({{ item.status.text }})</span></div>
            </div>
            <div class="right">
              <el-button type="text" @click="dialogOpenByFront(true,item)">编辑</el-button>
            </div>
          </div>
          <div class="it-component">
            <div class="img"> <img :src="item.mainImgUrl" alt=""> </div>
            <div class="describe">
              <ul>
                <li><el-button type="text" @click="dialogOpenByMachine(item)">({{ item.machineCount }}台)机器</el-button></li>
                <!-- <li><el-button type="text" style="color:#67c23a" @click="handleViewStock(item)">订单信息</el-button></li> -->
              </ul>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6" :xs="24" style="margin-bottom:20px">
        <el-card class="box-card">
          <div slot="header" class="it-header clearfix">
            <div class="left" />
            <el-button type="text">配置</el-button>
          </div>
          <div class="it-component">

            <div style="margin:auto;height:120px !important;width:120px !important; line-height:125px;" class="el-upload el-upload--picture-card"><i data-v-62e19c49="" class="el-icon-plus" /></div>

          </div>
        </el-card>
      </el-col>
    </el-row>

    <el-dialog v-loading="loadingByDialogByMachine" :title="'机器管理'" :visible.sync="dialogByMachineIsVisible">
      <div style="width:800px;height:600px">
        <el-col v-for="item in listDataByMachine" :key="item.id" :span="6" :xs="24" style="margin-bottom:20px">
          <el-card class="box-card">
            <div slot="header" class="it-header clearfix">
              <div class="left">
                <div class="circle-item"> <span :class="'icon-status icon-status-'+item.status.value" /> <span class="name">{{ item.name }}</span>          <span style="font-size:12px;"> ({{ item.status.text }})</span></div>
              </div>
              <div class="right">
                <el-button type="text" @click="handleRemoveMachine(item)">查看</el-button>
              </div>
            </div>
            <div class="it-component">
              <div class="img"> <img :src="item.mainImgUrl" alt=""> </div>
              <div class="describe" />
            </div>
          </el-card>
        </el-col>
      </div>
    </el-dialog>

  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { initManageShop, getShops, getMachines } from '@/api/store'
import { getUrlParam, isEmpty } from '@/utils/commonUtil'
import { all } from 'q'
export default {
  name: 'ManagePaneMachine',
  props: {
    storeid: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      loading: false,
      loadingByFromFront: false,
      loadingByDialogByMachine: false,
      listQuery: {
        page: 1,
        limit: 10,
        name: undefined
      },
      listData: [],
      listDataByMachine: [],
      storeId: '',
      storeName: '',
      formSelectMachines: [
      ],
      dialogByFrontIsEdit: false,
      dialogByFrontIsVisible: false,
      dialogByMachineIsVisible: false,
      formByFront: {
        name: '',
        address: '',
        briefDes: '',
        displayImgUrls: [],
        addressPoint: { // 详细地址经纬度
          lng: 0,
          lat: 0
        }
      },
      rulesByFront: {
        name: [{ required: true, min: 1, max: 30, message: '必填,且不能超过30个字符', trigger: 'change' }],
        address: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }],
        displayImgUrls: [{ type: 'array', required: true, message: '至少上传一张,且必须少于5张', max: 4 }],
        briefDes: [{ required: false, min: 0, max: 200, message: '不能超过200个字符', trigger: 'change' }]
      },
      uploadImglist: [],
      uploadImgMaxSize: 4,
      uploadImgPreImgDialogUrl: '',
      uploadImgPreImgDialogVisible: false,
      uploadImgServiceUrl: process.env.VUE_APP_UPLOADIMGSERVICE_URL
    }
  },
  watch: {
    storeid: function(val, oldval) {
      console.log('storeid3 值改变:' + val)
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
      if (!isEmpty(this.storeid)) {
        console.log('storeid3 1值改变:' + this.storeid)
        this.loading = true
        this.storeId = this.storeid
        this.listQuery.storeId = this.storeid
        initManageShop({ id: this.storeid }).then(res => {
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
    dialogOpenByFront(isEdit, item) {
      this.dialogByFrontIsVisible = true
      if (isEdit) {
        this.dialogByFrontIsEdit = true

        getShop({
          storeId: item.storeId,
          id: item.id
        }).then(res => {
          if (res.result === 1) {
            var d = res.data
            this.formByFront.id = d.id
            this.formByFront.storeId = d.storeId
            this.formByFront.name = d.name
            this.formByFront.address = d.address
            this.formByFront.briefDes = d.briefDes
            this.formByFront.displayImgUrls = d.displayImgUrls
            this.formByFront.isOpen = d.isOpen
            this.formByFront.status = d.status
            this.uploadImglist = this.getUploadImglist(d.displayImgUrls)
          }
        })
      } else {
        this.dialogByFrontIsEdit = false

        this.formByFront.id = ''
        this.formByFront.storeId = this.storeId
        this.formByFront.name = ''
        this.formByFront.address = ''
        this.formByFront.briefDes = ''
        this.formByFront.displayImgUrls = []
        this.uploadImglist = []
      }
    },
    dialogOpenByMachine(item) {
      this.dialogByMachineIsVisible = true
      this.loadingByDialogByMachine = true
      getMachines({ storeId: item.storeId, storeFrontId: item.id }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listDataByMachine = d
        }
        this.loadingByDialogByMachine = false
      })
    }
  }
}
</script>

<style lang="scss" scoped>

#store_list{

  padding: 20px;

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
