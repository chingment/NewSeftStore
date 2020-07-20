<template>
  <div class="prodcut-list">
    <el-container style="min-height:300px">
      <el-aside width="200px;">
        <div style="padding:10px;0px;text-align: center;">
          <el-button type="primary" icon="el-icon-edit" size="small" @click="dialogKindOpen(false)">新建分类</el-button>
        </div>

        <el-menu
          v-loading="isLoaingByKinds"
          :default-active="currentKindIndex+''"
          @select="kindSelect"
        >
          <el-menu-item v-for="(item,index) in listDataByKinds" :key="item.id" :index="index+''">
            <span slot="title">{{ item.name }}</span>
          </el-menu-item>

        </el-menu>

      </el-aside>

      <el-container>
        <el-header style="text-align: left; font-size: 12px;background-color: #fff">

          <span style="font-size: 21px;display: block; float: left;margin-right: 20px;width: 130px;">{{ currentKindName }} </span>

          <el-button type="primary" icon="el-icon-edit" size="small" :disabled="kindEditBtnDisabled" @click="dialogKindOpen(true)">编辑分类</el-button>

          <el-button type="primary" icon="el-icon-edit" size="small" :disabled="kindEditBtnDisabled" @click="dialogKindSpuOpen()">添加商品</el-button>

        </el-header>
        <el-main>
          <el-row v-loading="isLoaingByKindSpus" :gutter="20">
            <el-col v-for="item in listDataByKindSpus" :key="item.id" :span="5" style="margin-bottom:20px;">
              <el-card class="box-card box-card-product">
                <div slot="header" class="it-header clearfix">
                  <div class="left">
                    <span class="name">{{ item.name }}</span>
                  </div>
                  <div class="right">
                    <el-button type="text">管理</el-button>
                  </div>
                </div>
                <div class="it-component">
                  <div class="img"> <img :src="item.mainImgUrl" alt=""> </div>
                  <div class="describe" />
                </div>
              </el-card>
            </el-col>
          </el-row>
          <pagination v-show="listTotalByKindSpus>0" :total="listTotalByKindSpus" :page.sync="listQueryByGetKindSpus.page" :limit.sync="listQueryByGetKindSpus.limit" @pagination="_getKindSpus" />

          <span v-show="listTotalByKindSpus==0">该分类没有相关产品，请添加商品 {{ listTotalByKindSpus }}</span>
        </el-main>
      </el-container>
    </el-container>

    <el-dialog title="店铺分类" :visible.sync="dialogKindIsVisible" :width="isDesktop==true?'800px':'90%'">
      <el-form ref="kindForm" v-loading="dialogKindIsLoading" :model="kindForm" :rules="kindRules" label-width="75px">
        <el-form-item label="名称" prop="name">
          <el-input v-model="kindForm.name" clearable style="width:300px" />
        </el-form-item>
        <el-form-item label="图片" prop="displayImgUrls">
          <el-input :value="kindForm.displayImgUrls.toString()" style="display:none" />
          <el-upload
            ref="uploadImg"
            :action="uploadImgServiceUrl"
            list-type="picture-card"
            :on-success="handleSuccessByKindDisplayImgUrls"
            :on-remove="handleRemoveByKindDisplayImgUrls"
            :on-error="handleErrorByKindDisplayImgUrls"
            :on-preview="handlePreviewByKindDisplayImgUrls"
            :file-list="uploadImglistByKindDisplayImgUrls"
            :limit="4"
          >
            <i class="el-icon-plus" />
          </el-upload>
          <el-dialog :visible.sync="uploadImgPreImgDialogVisibleByKindDisplayImgUrls">
            <img width="100%" :src="uploadImgPreImgDialogUrlByKindDisplayImgUrls" alt="">
          </el-dialog>
          <div class="remark-tip"><span class="sign">*注</span>：第一张默认为主图，可拖动改变图片顺便</div>
        </el-form-item>
        <el-form-item label="描述" prop="description">
          <el-input v-model="kindForm.description" type="textarea" />
        </el-form-item>
        <el-form-item label="数据同步">
          <el-switch v-model="kindForm.isSynElseStore" />  <span>开启后，该分类信息同步到其它店铺分类信息中</span>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="_saveKind">保存</el-button>
          <el-button v-show="kindRemoveBtnShow" type="warning" @click="_removeKind">删除</el-button>
        </el-form-item>
      </el-form>

    </el-dialog>

    <el-dialog title="添加商品" :visible.sync="dialogKindSpuIsVisible" :width="isDesktop==true?'800px':'90%'">
      <el-form ref="kindSpuForm" v-loading="dialogKindSpuIsLoading" :model="kindSpuForm" :rules="kindSpuRules" label-width="80px">

        <el-form-item label="所属分类">
          <el-select v-model="kindSpuForm.kindId" placeholder="请选择" style="width:300px">
            <el-option
              v-for="item in listDataByKinds"
              :key="item.id"
              :label="item.name"
              :value="item.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="商品搜索" prop="productId">
          <el-autocomplete
            v-model="productSearchName"
            style="width:300px"
            :fetch-suggestions="productSearchAsync"
            placeholder="拼音/条形码/首字母"
            @select="productSelect"
          />
        </el-form-item>

        <el-form-item label="商品名称">
          <span>{{ productSearchName }}</span>
        </el-form-item>
        <el-form-item label="商品图片">
          <el-image
            style="width: 100px; height: 100px"
            :src="productSearchMainImgUrl"
            fit="fit"
          />
        </el-form-item>

        <el-form-item label="数据同步">
          <el-switch v-model="kindSpuForm.isSynElseStore" />  <span>开启后，该商品信息同步到其它店铺分类信息中</span>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="_saveKindSpu">保存</el-button>
        </el-form-item>
      </el-form>

    </el-dialog>

  </div>

</template>
<style lang="scss" scoped>

.prodcut-list   .el-menu{
    border-right: solid 0px #e6e6e6;
}

.prodcut-list .el-col-5{
  width: 20%;
}

  .el-header {
    background-color: #B3C0D1;
    color: #333;
    line-height: 60px;
  }

  .el-aside {
    color: #333;
  }

.box-card-product{
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

<script>
import { MessageBox } from 'element-ui'
import { getKinds, saveKind, saveKindSpu, getKindSpus, removeKind } from '@/api/store'
import { search } from '@/api/prdproduct'
import { getUrlParam } from '@/utils/commonUtil'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination
export default {
  name: 'ManagePaneProduct',
  components: { Pagination },
  data() {
    return {
      loading: false,
      storeId: '',
      isLoaingByKinds: false,
      isLoaingByKindSpus: false,
      listDataByKinds: [],
      dialogKindIsVisible: false,
      dialogKindIsEdit: false,
      dialogKindIsLoading: false,
      currentKindIndex: 0,
      currentKindName: '全部',
      kindEditBtnDisabled: true,
      kindRemoveBtnShow: false,
      kindForm: {
        id: '',
        name: '',
        description: '',
        displayImgUrls: [],
        isSynElseStore: false
      },
      kindRules: {
        name: [{ required: true, min: 1, max: 6, message: '必填,且不能超过6个字符', trigger: 'change' }],
        displayImgUrls: [{ type: 'array', required: true, message: '至少上传一张,且必须少于5张', max: 4 }],
        description: [{ required: false, min: 0, max: 500, message: '不能超过500个字符', trigger: 'change' }]
      },
      dialogKindSpuIsVisible: false,
      dialogKindSpuIsLoading: false,
      kindSpuForm: {
        storeId: '',
        kindId: '',
        productId: '',
        isSynElseStore: false
      },
      kindSpuRules: {
        storeId: [{ required: true, min: 1, message: '必填,且不能超过6个字符', trigger: 'change' }],
        kindId: [{ required: true, min: 1, message: '必填,且不能超过6个字符', trigger: 'change' }],
        productId: [{ required: true, min: 1, message: '请搜索商品后选择', trigger: 'change' }]
      },
      listQueryByGetKindSpus: {
        page: 1,
        limit: 10,
        storeId: '',
        kindId: ''
      },
      listDataByKindSpus: [],
      listTotalByKindSpus: 0,
      uploadImglistByKindDisplayImgUrls: [],
      uploadImgPreImgDialogUrlByKindDisplayImgUrls: '',
      uploadImgPreImgDialogVisibleByKindDisplayImgUrls: false,
      uploadImgServiceUrl: process.env.VUE_APP_UPLOADIMGSERVICE_URL,
      productSearchName: '',
      productSearchMainImgUrl: '',
      emptyImgUrl: 'http://file.17fanju.com/upload/default1.jpg',
      isDesktop: this.$store.getters.isDesktop
    }
  },
  watch: {
    '$route'(to, from) {
      this.init()
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      var id = getUrlParam('id')
      this.storeId = id
      this.kindForm.storeId = id
      this.kindSpuForm.storeId = id
      this.listDataByKinds = []
      this.listDataByKindSpus = []
      this.currentKindIndex = 0
      this._getKinds()
    },
    _getKinds() {
      this.isLoaingByKinds = true
      return getKinds({ id: this.storeId }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listDataByKinds = d

          if (d.length > 0) {
            this.currentKindName = d[this.currentKindIndex].name
            this.kindEditBtnDisabled = false
            this._getKindSpus()
          } else {
            this.kindEditBtnDisabled = true
            this.currentKindName = '暂无分类'
            this.listTotalByKindSpus = 0
          }
        }
        this.isLoaingByKinds = false
      })
    },
    kindSelect(index, indexPath) {
      var kind = this.listDataByKinds[index]
      this.kindEditBtnDisabled = false
      this.currentKindIndex = index
      this.currentKindName = kind.name
      this._getKindSpus()
    },
    _saveKind() {
      this.$refs['kindForm'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            saveKind(this.kindForm).then(res => {
              this.$message(res.message)
              if (res.result === 1) {
                this._getKinds()
              }
            })
          }).catch(() => {
          })
        }
      })
    },
    _removeKind() {
      MessageBox.confirm('确定要删除', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        removeKind({ storeId: this.storeId, id: this.kindForm.id, isSynElseStore: this.kindForm.isSynElseStore }).then(res => {
          this.$message(res.message)
          if (res.result === 1) {
            this._getKinds()
            this.dialogKindIsVisible = false
          }
        })
      }).catch(() => {
      })
    },
    _saveKindSpu() {
      this.$refs['kindSpuForm'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            saveKindSpu(this.kindSpuForm).then(res => {
              this.$message(res.message)
              if (res.result === 1) {
                this.dialogKindSpuIsVisible = false
                this._getKindSpus()
              }
            })
          }).catch(() => {
          })
        }
      })
    },
    _getKindSpus() {
      var kind = this.listDataByKinds[this.currentKindIndex]
      this.listQueryByGetKindSpus.storeId = this.storeId
      this.listQueryByGetKindSpus.kindId = kind.id
      this.isLoaingByKindSpus = true
      getKindSpus(this.listQueryByGetKindSpus).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listDataByKindSpus = d.items
          this.listTotalByKindSpus = d.total
        }
        this.isLoaingByKindSpus = false
      })
    },
    dialogKindOpen(isEdit) {
      this.dialogKindIsVisible = true

      if (isEdit) {
        this.kindRemoveBtnShow = true
        var kind = this.listDataByKinds[this.currentKindIndex]
        this.kindForm.id = kind.id
        this.kindForm.name = kind.name
        this.kindForm.description = kind.description
        this.kindForm.displayImgUrls = kind.displayImgUrls
        this.uploadImglistByKindDisplayImgUrls = this.getUploadImglist(kind.displayImgUrls)
        this.kindForm.isSynElseStore = false
      } else {
        this.kindRemoveBtnShow = false
        this.kindForm.id = ''
        this.kindForm.name = ''
        this.kindForm.description = ''
        this.kindForm.displayImgUrls = []
        this.uploadImglistByKindDisplayImgUrls = []
        this.kindForm.isSynElseStore = false
      }
    },
    dialogKindSpuOpen() {
      this.dialogKindSpuIsVisible = true
      var kind = this.listDataByKinds[ this.currentKindIndex]
      this.kindSpuForm.kindId = kind.id
      this.kindSpuForm.productId = ''
      this.productSearchName = ''
      this.productSearchMainImgUrl = this.emptyImgUrl
    },
    productSearchAsync(queryString, cb) {
      console.log('queryString:' + queryString)
      search({ key: queryString }).then(res => {
        if (res.result === 1) {
          var d = res.data
          var restaurants = []
          for (var j = 0; j < d.length; j++) {
            restaurants.push({ 'value': d[j].name, 'mainImgUrl': d[j].mainImgUrl, 'name': d[j].name, 'productId': d[j].productId })
          }

          cb(restaurants)
        }
      })
    },
    productSelect(item) {
      console.log(JSON.stringify(item))
      this.productSearchMainImgUrl = item.mainImgUrl
      this.kindSpuForm.storeId = this.storeId
      this.kindSpuForm.productId = item.productId
    },
    getUploadImglist(displayImgUrls) {
      var _uploadImglist = []
      for (var i = 0; i < displayImgUrls.length; i++) {
        _uploadImglist.push({ status: 'success', url: displayImgUrls[i].url, response: { data: { name: displayImgUrls[i].name, url: displayImgUrls[i].url }}})
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
    handleRemoveByKindDisplayImgUrls(file, fileList) {
      this.uploadImglistByKindDisplayImgUrls = fileList
      this.kindForm.displayImgUrls = this.getdisplayImgUrls(fileList)
    },
    handleSuccessByKindDisplayImgUrls(response, file, fileList) {
      this.uploadImglistByKindDisplayImgUrls = fileList
      this.kindForm.displayImgUrls = this.getdisplayImgUrls(fileList)
    },
    handleErrorByKindDisplayImgUrls(errs, file, fileList) {
      this.uploadImglistByKindDisplayImgUrls = fileList
      this.kindForm.displayImgUrls = this.getdisplayImgUrls(fileList)
    },
    handlePreviewByKindDisplayImgUrls(file) {
      this.uploadImgPreImgDialogUrlByKindDisplayImgUrls = file.url
      this.uploadImgPreImgDialogVisibleByKindDisplayImgUrls = true
    }
  }
}
</script>
