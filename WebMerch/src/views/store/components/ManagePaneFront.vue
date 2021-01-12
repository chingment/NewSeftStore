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
            <el-button type="text" @click="dialogOpenByFront(false,null)">新建</el-button>
          </div>
          <div class="it-component">

            <div style="margin:auto;height:120px !important;width:120px !important; line-height:125px;" class="el-upload el-upload--picture-card" @click="dialogOpenByFront(false,null)"><i class="el-icon-plus" /></div>

          </div>
        </el-card>
      </el-col>
    </el-row>

    <el-dialog :title="dialogByFrontIsEdit?'编辑':'新建'" :visible.sync="dialogByFrontIsVisible" width="800px">
      <el-form ref="formByFront" v-loading="loadingByFromFront" :model="formByFront" :rules="rulesByFront" label-width="80px">
        <el-form-item label="门店名称" prop="name">
          <el-input v-model="formByFront.name" clearable />
        </el-form-item>
        <el-form-item label="联系地址" prop="address">
          <el-input v-model="formByFront.address" clearable />
        </el-form-item>
        <el-form-item label="图片" prop="displayImgUrls">
          <el-input :value="formByFront.displayImgUrls.toString()" style="display:none" />
          <el-upload
            ref="uploadImg"
            v-model="formByFront.displayImgUrls"
            :action="uploadImgServiceUrl"
            list-type="picture-card"
            :before-upload="uploadBeforeHandle"
            :on-success="uploadSuccessHandle"
            :on-remove="uploadRemoveHandle"
            :on-error="uploadErrorHandle"
            :on-preview="uploadPreviewHandle"
            :file-list="uploadImglist"
          >
            <i class="el-icon-plus" />
          </el-upload>
          <el-dialog :visible.sync="uploadImgPreImgDialogVisible">
            <img width="100%" :src="uploadImgPreImgDialogUrl" alt="">
          </el-dialog>
          <div class="remark-tip"><span class="sign">*注</span>：图片500*500，格式（jpg,png）不超过4M；第一张为主图，可拖动改变图片顺序</div>
        </el-form-item>
        <el-form-item label="简短描述" style="max-width:1000px">
          <el-input v-model="formByFront.briefDes" type="text" maxlength="200" clearable show-word-limit />
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogByFrontIsVisible = false">
          取消
        </el-button>
        <el-button type="primary" @click="_saveFront">
          保存
        </el-button>
      </div>
    </el-dialog>

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
import { initManageFront, getFrontList, saveFront, getFront, getMachineList } from '@/api/store'
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
    this.setUploadImgSort()
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
        initManageFront({ id: this.storeid }).then(res => {
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
      getFrontList(listQuery).then(res => {
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

        getFront({
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
    _saveFront() {
      this.$refs['formByFront'].validate(valid => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          })
            .then(() => {
              saveFront(this.formByFront).then(res => {
                this.$message(res.message)
                if (res.result === 1) {
                  this.init()

                  this.dialogByFrontIsVisible = false
                }
              })
            })
            .catch(() => {})
        }
      })
    },
    getUploadImglist(displayImgUrls) {
      var _uploadImglist = []
      if (displayImgUrls !== null) {
        for (var i = 0; i < displayImgUrls.length; i++) {
          _uploadImglist.push({ status: 'success', url: displayImgUrls[i].url, response: { data: { name: displayImgUrls[i].name, url: displayImgUrls[i].url }}})
        }
      }

      return _uploadImglist
    },
    getdisplayImgUrls(fileList) {
      var _displayImgUrls = []
      for (var i = 0; i < fileList.length; i++) {
        if (fileList[i].status === 'success') {
          _displayImgUrls.push({ name: fileList[i].response.data.name, url: fileList[i].response.data.url })
        }
      }
      return _displayImgUrls
    },
    uploadBeforeHandle(file) {
      if (this.formByFront.displayImgUrls.length >= this.uploadImgMaxSize) {
        this.$message.error('上传图片不能超过4张!')
        return false
      }

      const imgType = file.type
      const isLt4M = file.size / 1024 / 1024 < 4
      //  var a = isLt4M === true ? 'true' : 'false'
      if (imgType !== 'image/jpeg' && imgType !== 'image/png' && imgType !== 'image/jpg') {
        this.$message('图片格式仅支持(jpg,png)')
        return false
      }

      if (!isLt4M) {
        this.$message('图片大小不能超过4M')
        return false
      }

      return true
    },
    uploadCardCheckShow() {
      var uploadcard = this.$refs.uploadImg.$el.querySelectorAll('.el-upload--picture-card')
      if (this.formByFront.displayImgUrls.length === this.uploadImgMaxSize) {
        uploadcard[0].style.display = 'none'
      } else {
        uploadcard[0].style.display = 'inline-block'
      }
    },
    uploadRemoveHandle(file, fileList) {
      this.uploadImglist = fileList
      this.formByFront.displayImgUrls = this.getdisplayImgUrls(fileList)
      this.uploadCardCheckShow()
    },
    uploadSuccessHandle(response, file, fileList) {
      this.uploadImglist = fileList
      this.formByFront.displayImgUrls = this.getdisplayImgUrls(fileList)
      this.uploadCardCheckShow()
    },
    uploadErrorHandle(errs, file, fileList) {
      this.uploadImglist = fileList
      this.formByFront.displayImgUrls = this.getdisplayImgUrls(fileList)
    },
    uploadPreviewHandle(file) {
      this.uploadImgPreImgDialogUrl = file.url
      this.uploadImgPreImgDialogVisible = true
    },
    setUploadImgSort() {
      var _this = this
      const $ul = _this.$refs.uploadImg.$el.querySelectorAll('.el-upload-list')[0]
      new Sortable($ul, {
        onUpdate: function(event) {
        // 修改items数据顺序
          var newIndex = event.newIndex
          var oldIndex = event.oldIndex
          var $li = $ul.children[newIndex]
          var $oldLi = $ul.children[oldIndex]
          // 先删除移动的节点
          $ul.removeChild($li)
          // 再插入移动的节点到原有节点，还原了移动的操作
          if (newIndex > oldIndex) {
            $ul.insertBefore($li, $oldLi)
          } else {
            $ul.insertBefore($li, $oldLi.nextSibling)
          }
          // 更新items数组
          var item = _this.uploadImglist.splice(oldIndex, 1)
          _this.uploadImglist.splice(newIndex, 0, item[0])

          _this.formByFront.displayImgUrls = _this.getdisplayImgUrls(_this.uploadImglist)
        // 下一个tick就会走patch更新
        },
        animation: 150
      })
    },
    dialogOpenByMachine(item) {
      this.dialogByMachineIsVisible = true
      this.loadingByDialogByMachine = true
      getMachineList({ storeId: item.storeId, storeFrontId: item.id }).then(res => {
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
