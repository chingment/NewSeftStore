<template>
  <div id="store_kind">
    <el-container style="min-height:300px">
      <el-aside width="200px;" style="background:#fff">
        <div style="padding:4px;0px;text-align: center;">
          <el-button
            type="text"
            icon="el-icon-plus"
            style="font-size: 18px;color:#000;"
            @click="dialogKindOpen(false)"
          >新建分类</el-button>
        </div>

        <el-menu
          v-loading="isLoaingByKinds"
          :default-active="currentKindIndex+''"
          style="background:#fff"
          @select="kindSelect"
        >
          <el-menu-item v-for="(item,index) in listDataByKinds" :key="item.id" :index="index+''">
            <span slot="title">{{ item.name }}</span>
          </el-menu-item>
        </el-menu>
      </el-aside>

      <el-container>
        <el-header style="height:auto; text-align: left; font-size: 12px;background-color: #fff;padding: 4px 0 0 20px;">
          <el-button
            type="text"
            icon="el-icon-edit"
            style="font-size: 18px;color:#000;margin-right:10px"
            :disabled="kindEditBtnDisabled"
            @click="dialogKindOpen(true)"
          >{{ currentKindName }}</el-button>
          <el-button
            type="primary"
            style="margin-left:0px;"
            icon="el-icon-edit"
            size="small"
            :disabled="kindEditBtnDisabled"
            @click="dialogKindSpuOpen(false,null)"
          >添加商品</el-button>
        </el-header>
        <el-main class="fn-main">
          <el-row v-loading="isLoaingByKindSpus" :gutter="24">
            <el-col
              v-for="item in listDataByKindSpus"
              :key="item.id"

              :xs="24"
              :sm="12"
              :lg="8"
              :xl="6"
              class="my-col"
            >
              <el-card class="box-card box-card-product">
                <div slot="header" class="it-header clearfix">
                  <div class="left">
                    <span class="name">{{ item.name }}</span>
                  </div>
                  <div class="right">
                    <el-button type="text" @click="dialogKindSpuOpen(true,item)">编辑</el-button>
                  </div>
                </div>
                <div class="it-component">
                  <div class="img">
                    <img :src="item.mainImgUrl" alt>
                  </div>
                  <div class="describe" />
                </div>
              </el-card>
            </el-col>
          </el-row>
          <pagination
            v-show="listTotalByKindSpus>0"
            :total="listTotalByKindSpus"
            :page.sync="listQueryByGetKindSpus.page"
            :limit.sync="listQueryByGetKindSpus.limit"
            @pagination="_getKindSpus"
          />

          <span v-show="listTotalByKindSpus==0">该分类没有相关产品，请添加商品</span>
        </el-main>
      </el-container>
    </el-container>

    <el-dialog
      title="店铺分类"
      :visible.sync="dialogKindIsVisible"
      :width="isDesktop==true?'800px':'90%'"
    >
      <el-form
        ref="kindForm"
        v-loading="dialogKindIsLoading"
        :model="kindForm"
        :rules="kindRules"
        label-width="75px"
      >
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
          <el-dialog :visible.sync="uploadImgPreImgDialogVisibleByKindDisplayImgUrls" append-to-body>
            <img width="100%" :src="uploadImgPreImgDialogUrlByKindDisplayImgUrls" alt>
          </el-dialog>
          <el-alert
            title="提示：图片500*500，格式（jpg,png）不超过4M；第一张为主图，可拖动改变图片顺序"
            type="remark-gray"
            :closable="false"
          />
        </el-form-item>
        <el-form-item label="描述" prop="description">
          <el-input v-model="kindForm.description" type="textarea" />
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button size="small" type="primary" @click="_saveKind">保存</el-button>
        <el-button v-show="kindRemoveBtnShow" size="small" type="warning" @click="_removeKind">删除</el-button>
        <el-button size="small" @click="dialogKindIsVisible=false">关闭</el-button>
      </span>
    </el-dialog>

    <el-dialog
      :title="dialogKindSpuIsEdit?'编辑商品':'添加商品'"
      :visible.sync="dialogKindSpuIsVisible"
      :width="isDesktop==true?'800px':'90%'"
    >
      <el-form
        ref="kindSpuForm"
        v-loading="dialogKindSpuIsLoading"
        :model="kindSpuForm"
        :rules="kindSpuRules"
        label-width="80px"
      >
        <el-form-item label="所属分类">
          <el-select
            v-if="!dialogKindSpuIsEdit"
            v-model="kindSpuForm.kindId"
            :disabled="dialogKindSpuIsEdit?true:false"
            placeholder="请选择"
            style="width:300px"
          >
            <el-option
              v-for="item in listDataByKinds"
              :key="item.id"
              :label="item.name"
              :value="item.id"
            />
          </el-select>

          <span v-if="dialogKindSpuIsEdit">{{ kindSpuForm.kindName }}</span>

        </el-form-item>
        <el-form-item v-show="!dialogKindSpuIsEdit" label="商品搜索" prop="spuId">
          <el-autocomplete
            v-model="spuSearchName"
            style="width:300px"
            :fetch-suggestions="spuSearchAsync"
            placeholder="拼音/条形码/首字母"
            @select="spuSelect"
          />
        </el-form-item>

        <el-form-item label="商品名称">
          <span>{{ spuSearchName }}</span>
        </el-form-item>
        <el-form-item label="商品图片">
          <el-image style="width: 100px; height: 100px" :src="spuSearchMainImgUrl" fit="fit" />
        </el-form-item>
        <el-form-item label="商城库存" style="max-width:1000px">
          <table class="list-tb" cellpadding="0" cellspacing="0">
            <thead>
              <tr>
                <th style="width:100px">规格</th>
                <th style="width:100px">编码</th>
                <th style="width:100px">库存</th>
                <th style="width:100px">价格</th>
                <th v-if="kindSpuForm.isSupRentService" style="width:50px">租用</th>
                <th v-if="kindSpuForm.isSupRentService" style="width:100px">押金</th>
                <th v-if="kindSpuForm.isSupRentService" style="width:100px">月租</th>
                <th style="width:50px">下架</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item,x) in kindSpuForm.stocks" :key="x">
                <td>{{ item.specIdx }}</td>
                <td>{{ item.cumCode }}</td>
                <td>
                  <el-input v-model="item.sumQuantity" clearable style="width:90%" />
                </td>
                <td>
                  <el-input v-model="item.salePrice" clearable style="width:90%" />
                </td>
                <td v-if="kindSpuForm.isSupRentService">
                  <el-checkbox v-model="item.isUseRent" />
                </td>
                <td v-if="kindSpuForm.isSupRentService">
                  <el-input v-model="item.depositPrice" clearable style="width:90%" />
                </td>
                <td v-if="kindSpuForm.isSupRentService">
                  <el-input v-model="item.rentMhPrice" clearable style="width:90%" />
                </td>
                <td>
                  <el-checkbox v-model="item.isOffSell" />
                </td>
              </tr>
            </tbody>
          </table>
          <el-alert
            show-icon
            title="商城库存只用于线上配送到手,非门店配送到手"
            type="remark"
            :closable="false"
          />
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button size="small" type="primary" @click="_saveKindSpu">保存</el-button>
        <el-button v-show="kindSpuRemoveBtnShow" size="small" type="warning" @click="_removeKindSpu">删除</el-button>
        <el-button size="small" @click="dialogKindSpuIsVisible=false">关闭</el-button>
      </span>
    </el-dialog>
  </div>
</template>
<style lang="scss" scoped>
#store_kind .el-menu {
  border-right: solid 0px #e6e6e6;
}

#store_kind .el-col-5 {
  width: 20%;
}

.el-header {
  background-color: #b3c0d1;
  color: #333;
}

.el-aside {
  color: #333;
}

.box-card-product {
  .it-header {
    display: flex;
    justify-content: flex-start;
    align-items: center;
    position: relative;
    height: 20px;
    .left {
      flex: 1;
      justify-content: flex-start;
      align-items: center;
      display: block;
      height: 100%;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
      .name {
        padding: 0px 5px;
      }
    }
    .right {
      width: 100px;
      display: flex;
      justify-content: flex-end;
      align-items: center;
    }
  }
  .it-component {
    min-height: 100px;
    display: flex;
    .img {
      width: 120px;
      height: 120px;

      img {
        width: 100%;
        height: 100%;
      }
    }

    .describe {
      flex: 1;
      padding: 0px;
      font-size: 12px;

      ul {
        padding: 0px;
        margin: 0px;
        list-style: none;
        li {
          width: 100%;
          text-align: right;
          height: 26px;
          line-height: 26px;
        }
      }
    }
  }
}

.fn-main{
    box-shadow: 0 0 0 0 rgba(0, 0, 0, 0.1);
}
</style>

<script>
import { MessageBox } from 'element-ui'
import {
  getKinds,
  saveKind,
  saveKindSpu,
  getKindSpus,
  removeKind,
  removeKindSpu,
  getKindSpu
} from '@/api/store'
import { searchSpu, getSpecs } from '@/api/product'
import { isEmpty } from '@/utils/commonUtil'
import Pagination from '@/components/Pagination'
export default {
  name: 'StorePaneShop',
  components: { Pagination },
  props: {
    storeId: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      loading: false,
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
        kindId: '',
        name: '',
        description: '',
        displayImgUrls: []
      },
      kindRules: {
        name: [
          {
            required: true,
            min: 1,
            max: 6,
            message: '必填,且不能超过6个字符',
            trigger: 'change'
          }
        ],
        displayImgUrls: [
          {
            type: 'array',
            required: true,
            message: '至少上传一张,且必须少于5张',
            max: 4
          }
        ],
        description: [
          {
            required: false,
            min: 0,
            max: 500,
            message: '不能超过500个字符',
            trigger: 'change'
          }
        ]
      },
      dialogKindSpuIsVisible: false,
      dialogKindSpuIsLoading: false,
      dialogKindSpuIsEdit: false,
      kindSpuRemoveBtnShow: false,
      kindSpuForm: {
        storeId: '',
        kindId: '',
        kindName: '',
        spuId: '',
        isSupRentService: false,
        stocks: []
      },
      kindSpuRules: {
        storeId: [
          {
            required: true,
            min: 1,
            message: '必填,且不能超过6个字符',
            trigger: 'change'
          }
        ],
        kindId: [
          {
            required: true,
            min: 1,
            message: '必填,且不能超过6个字符',
            trigger: 'change'
          }
        ],
        spuId: [
          {
            required: true,
            min: 1,
            message: '请搜索商品后选择',
            trigger: 'change'
          }
        ]
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
      spuSearchName: '',
      spuSearchMainImgUrl: '',
      emptyImgUrl: 'http://file.17fanju.com/upload/default1.jpg',
      isDesktop: this.$store.getters.isDesktop
    }
  },
  watch: {
    storeId: function(val, oldval) {
      this.init()
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      if (!isEmpty(this.storeId)) {
        this.kindForm.storeId = this.storeId
        this.kindSpuForm.storeId = this.storeId
        this.listDataByKinds = []
        this.listDataByKindSpus = []
        this.currentKindIndex = 0
        this._getKinds()
      }
    },
    _getKinds() {
      this.isLoaingByKinds = true
      return getKinds({ id: this.storeId }).then(res => {
        this.isLoaingByKinds = false
        if (res.result === 1) {
          var d = res.data
          this.listDataByKinds = d

          if (d.length > 0) {
            var currentKindIndex = 0
            if (this.currentKindIndex > d.length) {
              currentKindIndex = d.length - 1
              this.currentKindIndex = currentKindIndex
            }

            this.currentKindName = d[this.currentKindIndex].name
            this.kindEditBtnDisabled = false
            this._getKindSpus()
          } else {
            this.kindEditBtnDisabled = true
            this.currentKindName = '暂无分类'
            this.listTotalByKindSpus = 0
          }
        }
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
      this.$refs['kindForm'].validate(valid => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          })
            .then(() => {
              saveKind(this.kindForm).then(res => {
                if (res.result === 1) {
                  this.$message({
                    message: res.message,
                    type: 'success'
                  })
                  this._getKinds()
                } else {
                  this.$message({
                    message: res.message,
                    type: 'error'
                  })
                }
              })
            })
            .catch(() => {})
        }
      })
    },
    _removeKind() {
      MessageBox.confirm('确定要删除', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      })
        .then(() => {
          removeKind({
            storeId: this.storeId,
            kindId: this.kindForm.kindId
          }).then(res => {
            if (res.result === 1) {
              this.$message({
                message: res.message,
                type: 'success'
              })
              this._getKinds()
              this.dialogKindIsVisible = false
            } else {
              this.$message({
                message: res.message,
                type: 'error'
              })
            }
          })
        })
        .catch(() => {})
    },
    _saveKindSpu() {
      this.$refs['kindSpuForm'].validate(valid => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          })
            .then(() => {
              saveKindSpu(this.kindSpuForm).then(res => {
                if (res.result === 1) {
                  this.$message({
                    message: res.message,
                    type: 'success'
                  })
                  this.dialogKindSpuIsVisible = false
                  this._getKindSpus()
                } else {
                  this.$message({
                    message: res.message,
                    type: 'error'
                  })
                }
              })
            })
            .catch(() => {})
        }
      })
    },
    _removeKindSpu() {
      MessageBox.confirm('确定要移除', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      })
        .then(() => {
          removeKindSpu({
            storeId: this.kindSpuForm.storeId,
            kindId: this.kindSpuForm.kindId,
            spuId: this.kindSpuForm.spuId
          }).then(res => {
            if (res.result === 1) {
              this.$message({
                message: res.message,
                type: 'success'
              })
              this.dialogKindSpuIsVisible = false
              this._getKindSpus()
            } else {
              this.$message({
                message: res.message,
                type: 'error'
              })
            }
          })
        })
        .catch(() => {})
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
        this.kindForm.kindId = kind.id
        this.kindForm.name = kind.name
        this.kindForm.description = kind.description
        this.kindForm.displayImgUrls = kind.displayImgUrls
        this.uploadImglistByKindDisplayImgUrls = this.getUploadImglist(
          kind.displayImgUrls
        )
      } else {
        this.kindRemoveBtnShow = false
        this.kindForm.kindId = ''
        this.kindForm.name = ''
        this.kindForm.description = ''
        this.kindForm.displayImgUrls = []
        this.uploadImglistByKindDisplayImgUrls = []
      }
    },
    dialogKindSpuOpen(isEdit, item) {
      this.dialogKindSpuIsEdit = isEdit
      this.dialogKindSpuIsVisible = true
      var kind = this.listDataByKinds[this.currentKindIndex]
      this.kindSpuForm.kindId = kind.id
      if (isEdit) {
        this.kindSpuRemoveBtnShow = true
        getKindSpu({
          storeId: item.storeId,
          spuId: item.spuId,
          kindId: item.kindId
        }).then(res => {
          if (res.result === 1) {
            var d = res.data
            this.kindSpuForm.spuId = item.spuId
            this.kindSpuForm.storeId = item.storeId
            this.kindSpuForm.stocks = d.stocks
            this.kindSpuForm.kindName = d.kindName
            this.kindSpuForm.isSupRentService = d.isSupRentService
            this.spuSearchName = d.name
            this.spuSearchMainImgUrl = d.mainImgUrl
          }
        })
      } else {
        this.kindSpuRemoveBtnShow = false
        this.kindSpuForm.spuId = ''
        this.kindSpuForm.stocks = []
        this.kindSpuForm.isSupRentService = false
        this.spuSearchName = ''
        this.spuSearchMainImgUrl = this.emptyImgUrl
      }
    },
    spuSearchAsync(queryString, cb) {
      searchSpu({ key: queryString }).then(res => {
        if (res.result === 1) {
          var d = res.data
          var restaurants = []
          for (var j = 0; j < d.length; j++) {
            restaurants.push({
              spuId: d[j].spuId,
              value: d[j].name,
              mainImgUrl: d[j].mainImgUrl,
              name: d[j].name,
              isSupRentService: d[j].isSupRentService
            })
          }

          cb(restaurants)
        }
      })
    },
    spuSelect(item) {
      this.spuSearchMainImgUrl = item.mainImgUrl
      this.kindSpuForm.storeId = this.storeId
      this.kindSpuForm.spuId = item.spuId
      this.kindSpuForm.isSupRentService = item.isSupRentService
      getSpecs({ id: item.spuId }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.kindSpuForm.stocks = d
        } else {
          this.kindSpuForm.stock = []
        }
      })
    },
    getUploadImglist(displayImgUrls) {
      var _uploadImglist = []
      for (var i = 0; i < displayImgUrls.length; i++) {
        _uploadImglist.push({
          status: 'success',
          url: displayImgUrls[i].url,
          response: {
            data: { name: displayImgUrls[i].name, url: displayImgUrls[i].url }
          }
        })
      }

      return _uploadImglist
    },
    getdisplayImgUrls(fileList) {
      var _displayImgUrls = []
      for (var i = 0; i < fileList.length; i++) {
        if (fileList[i].status === 'success') {
          _displayImgUrls.push({
            name: fileList[i].response.data.name,
            url: fileList[i].response.data.url
          })
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
