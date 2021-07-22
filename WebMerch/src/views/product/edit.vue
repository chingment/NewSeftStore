<template>
  <div id="product_baseinfo">
    <page-header />
    <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="100px">
      <el-form-item label="名称" prop="name">
        <el-input v-model="form.name" clearable />
      </el-form-item>
      <el-form-item label="货号" prop="spuCode">
        <el-input v-model="form.spuCode" clearable />
      </el-form-item>
      <el-form-item label="供应商">
        <el-select
          v-model="form.supplierId"
          clearable
          filterable
          remote
          reserve-keyword
          placeholder="代码/名称"
          :remote-method="searchSupplier"
          :loading="supplier_option_loading"
          style="width:300px"
        >
          <el-option
            v-for="item in supplier_options"
            :key="item.id"
            :label="item.name"
            :value="item.id"
          />
        </el-select>
      </el-form-item>
      <el-form-item label="图片" prop="displayImgUrls">
        <el-input :value="form.displayImgUrls.toString()" style="display:none" />
        <el-upload
          ref="uploadImgByDisplayImgUrls"
          v-model="form.displayImgUrls"
          :action="uploadImgServiceUrl"
          :data="uploadImgPmsByByDisplayImgUrls"
          list-type="picture-card"
          :on-success="handleSuccessByDisplayImgUrls"
          :on-remove="handleRemoveByDisplayImgUrls"
          :on-error="handleErrorByDisplayImgUrls"
          :on-preview="handlePreviewByDisplayImgUrls"
          :before-upload="handleBeforeUploadByDisplayImgUrls"
          :file-list="uploadImglistByDisplayImgUrls"
          :limit="4"
        >
          <i class="el-icon-plus" />
        </el-upload>
        <el-dialog :visible.sync="uploadImgPreImgDialogVisibleByDisplayImgUrls">
          <img width="100%" :src="uploadImgPreImgDialogUrlByDisplayImgUrls" alt>
        </el-dialog>
        <el-alert
          title="提示：图片500*500，格式（jpg,png）不超过4M；第一张为主图，可拖动改变图片顺序"
          type="remark-gray"
          :closable="false"
        />
      </el-form-item>
      <el-form-item label="所属分类" prop="kindIds">
        <el-cascader
          v-model="form.kindIds"
          :options="kind_options"
          placeholder="请选择"
          style="width:300px"
          clearable
        />
        <el-alert show-icon title="如果商品分类不满足业务需要，请联系系统管理员进行添加或修改" type="remark" :closable="false" />
      </el-form-item>
      <el-form-item label="特色标签" prop="charTags">
        <el-tag
          v-for="tag in form.charTags"
          :key="tag"
          closable
          :disable-transitions="false"
          @close="charTagsHandleClose(tag)"
        >{{ tag }}</el-tag>
        <el-input
          v-if="charTagsInputVisible"
          ref="saveTagInput"
          v-model="charTagsInputValue"
          class="input-new-tag"
          size="small"
          @keyup.enter.native="charTagsHandleInputConfirm"
          @blur="charTagsHandleInputConfirm"
        />
        <el-button v-else class="button-new-tag" size="small" @click="charTagsShowInput">+ 添加</el-button>
      </el-form-item>
      <el-form-item label="取货模式">
        <el-radio-group v-model="form.supReceiveMode">
          <el-radio-button label="1">仅限配送</el-radio-button>
          <el-radio-button label="2">仅限门店自提</el-radio-button>
          <el-radio-button label="3">可配送或门店自提</el-radio-button>
          <!-- <el-radio-button label="6">仅限门店消费</el-radio-button> -->
        </el-radio-group>
      </el-form-item>
      <el-form-item label="属性" style="max-width:1000px">
        <el-checkbox v-model="form.isMavkBuy">单独购买</el-checkbox>
        <el-checkbox v-model="form.isTrgVideoService">音视频咨询</el-checkbox>
        <el-checkbox v-model="form.isRevService">预约服务商品</el-checkbox>
        <el-checkbox v-model="form.isSupRentService">支持租赁方式</el-checkbox>
      </el-form-item>
      <el-form-item label="SKU列表" style="max-width:1000px">
        <el-checkbox v-model="form.isUnifyUpdateSalePrice">统一更新所有店铺销售信息（价格，下架）</el-checkbox>

        <el-alert
          show-icon
          title="提示：勾选后，下面列表里的（价格，下架）会统一更新到所有店铺，不勾选只作参考价格"
          type="remark"
          :closable="false"
        />

        <table class="list-tb" cellpadding="0" cellspacing="0">
          <thead>
            <tr>
              <th v-for="(item,y) in form.specItems" :key="y">{{ item.name }}</th>
              <th style="width:180px">编码</th>
              <th style="width:180px">条形码</th>
              <th style="width:100px">价格</th>
              <th style="width:100px">下架</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(item,x) in form.skus" :key="x">

              <td v-for="(specDes,y) in item.specDes" :key="y">

                <el-input v-model="specDes.value" clearable style="width:90%" />

              </td>

              <td>
                <el-input v-model="item.cumCode" clearable style="width:90%" />
              </td>
              <td>
                <el-input v-model="item.barCode" clearable style="width:90%" />
              </td>
              <td>
                <el-input v-model="item.salePrice" clearable style="width:90%" />
              </td>
              <td>
                <el-checkbox v-model="item.isOffSell" />
              </td>
            </tr>
          </tbody>
        </table>
      </el-form-item>

      <el-form-item label="简短描述" style="max-width:1000px">
        <el-input v-model="form.briefDes" maxlength="200" clearable />
      </el-form-item>
      <el-form-item label="详情图片" prop="detailsDes">
        <el-input
          :value="form.detailsDes==null?'':form.detailsDes.toString()"
          style="display:none"
        />
        <el-upload
          ref="uploadImgByDetailsDes"
          v-model="form.detailsDes"
          :action="uploadImgServiceUrl"
          list-type="picture-card"
          :on-success="handleSuccessByDetailsDes"
          :on-remove="handleRemoveByDetailsDes"
          :on-error="handleErrorByDetailsDes"
          :on-preview="handlePreviewByDetailsDes"
          :before-upload="handleBeforeUploadByDetailsDes"
          :file-list="uploadImglistByDetailsDes"
          :data="uploadImgPmsByByDetailsDes"
          :limit="4"
        >
          <i class="el-icon-plus" />
        </el-upload>
        <el-dialog :visible.sync="uploadImgPreImgDialogVisibleByDetailsDes">
          <img width="100%" :src="uploadImgPreImgDialogUrlByDetailsDes" alt>
        </el-dialog>
        <el-alert title="提示：图片不超过4M；可拖动改变图片顺序" type="remark-gray" :closable="false" />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="onSubmit">保存</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>

import { MessageBox } from 'element-ui'
import { edit, initEdit } from '@/api/product'
import supplier from '@/api/supplier'
import fromReg from '@/utils/formReg'
import { getUrlParam, strLen, isMoney } from '@/utils/commonUtil'
import PageHeader from '@/components/PageHeader/index.vue'
import '@riophae/vue-treeselect/dist/vue-treeselect.css'

import Sortable from 'sortablejs'
export default {
  name: 'ProductEdit',
  components: { PageHeader },
  data() {
    return {
      loading: false,
      form: {
        id: '',
        name: '',
        spuCode: '',
        supplierId: '',
        kindIds: [],
        subjectIds: [],
        detailsDes: [],
        charTags: [],
        specItems: [],
        isMavkBuy: false,
        isTrgVideoService: false,
        isRevService: false,
        isSupRentService: false,
        supReceiveMode: '1',
        briefDes: '',
        displayImgUrls: [],
        isUnifyUpdateSalePrice: false,
        skus: []
      },
      rules: {
        name: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }],
        spuCode: [{ required: true, min: 1, max: 50, message: '必填,且不能超过50个字符', trigger: 'change' }],
        kindIds: [{ type: 'array', required: true, message: '请选择一个三级商品分类', min: 3, max: 3 }],
        displayImgUrls: [{ type: 'array', required: true, message: '至少上传一张,且必须少于5张', max: 4 }],
        detailsDes: [{ type: 'array', required: false, message: '不能超过3张', max: 3 }],
        charTags: [{ type: 'array', required: false, message: '不能超过5个', max: 3 }]
      },
      kind_options: [],
      supplier_options: [],
      supplier_option_loading: false,
      uploadImglistByDisplayImgUrls: [],
      uploadImgPreImgDialogUrlByDisplayImgUrls: '',
      uploadImgPreImgDialogVisibleByDisplayImgUrls: false,
      uploadImgPmsByByDisplayImgUrls: { folder: 'product', isBuildms: 'true' },
      uploadImgServiceUrl: process.env.VUE_APP_UPLOADIMGSERVICE_URL,
      uploadImglistByDetailsDes: [],
      uploadImgPreImgDialogUrlByDetailsDes: '',
      uploadImgPreImgDialogVisibleByDetailsDes: false,
      uploadImgPmsByByDetailsDes: { folder: 'product', isBuildms: 'false' },
      charTagsInputVisible: false,
      charTagsInputValue: ''
    }
  },
  mounted() {
    this.setUploadImgSortByDisplayImgUrls()
    this.setUploadImgSortByDetailsDes()
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      this.loading = true
      var id = getUrlParam('id')
      initEdit({ id: id }).then(res => {
        if (res.result === 1) {
          var d = res.data

          this.form.id = d.id
          this.form.name = d.name
          this.form.spuCode = d.spuCode
          this.form.kindIds = d.kindIds
          this.form.detailsDes = d.detailsDes
          this.form.briefDes = d.briefDes
          this.form.displayImgUrls = d.displayImgUrls
          this.form.specItems = d.specItems
          this.form.skus = d.skus
          this.form.isMavkBuy = d.isMavkBuy
          this.form.isTrgVideoService = d.isTrgVideoService
          this.form.isRevService = d.isRevService
          this.form.isSupRentService = d.isSupRentService
          this.form.supReceiveMode = d.supReceiveMode + ''
          this.form.charTags = d.charTags === null ? [] : d.charTags
          this.uploadImglistByDisplayImgUrls = this.getUploadImglist(d.displayImgUrls)
          this.uploadImglistByDetailsDes = this.getUploadImglist(d.detailsDes)
          this.kind_options = d.kinds
          this.supplier_options = [{ id: d.supplierId, name: d.supplierName }]
          this.form.supplierId = d.supplierId
        }
        this.loading = false
      })
    },
    resetForm() {

    },
    onSubmit() {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          for (var i = 0; i < this.form.skus.length; i++) {
            var strName = '规格 '

            for (var j = 0; j < this.form.skus[i].specDes.length; j++) {
              strName += this.form.skus[i].specDes[j].value + ' '
            }

            if (strLen(this.form.skus[i].cumCode) <= 0 || strLen(this.form.skus[i].cumCode) > 30) {
              this.$message(strName + '的编码不能为空，且不能超过30个字符')
              return false
            }

            if (strLen(this.form.skus[i].barCode) > 30) {
              this.$message(strName + '的条形码不能超过30个字符')
              return false
            }

            if (!isMoney(this.form.skus[i].salePrice)) {
              this.$message(strName + '的价格格式必须为:xx.xx,eg: 88.88')
              return false
            }
          }

          var _form = {}
          _form.id = this.form.id
          _form.name = this.form.name
          _form.spuCode = this.form.spuCode
          _form.kindIds = this.form.kindIds
          _form.detailsDes = this.form.detailsDes
          _form.specDes = this.form.specDes
          _form.briefDes = this.form.briefDes
          _form.displayImgUrls = this.form.displayImgUrls
          _form.isUnifyUpdateSalePrice = this.form.isUnifyUpdateSalePrice
          _form.charTags = this.form.charTags
          _form.isTrgVideoService = this.form.isTrgVideoService
          _form.isRevService = this.form.isRevService
          _form.isSupRentService = this.form.isSupRentService
          _form.isMavkBuy = this.form.isMavkBuy
          _form.supReceiveMode = this.form.supReceiveMode
          _form.skus = this.form.skus
          _form.supplierId = this.form.supplierId
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            edit(_form).then(res => {
              if (res.result === 1) {
                this.$message({
                  message: res.message,
                  type: 'success'
                })
              } else {
                this.$message({
                  message: res.message,
                  type: 'error'
                })
              }
            })
          })
        }
      })
    },
    getUploadImglist(imgUrls) {
      var _uploadImglist = []
      if (imgUrls != null) {
        for (var i = 0; i < imgUrls.length; i++) {
          _uploadImglist.push({ status: 'success', url: imgUrls[i].url, name: imgUrls[i].name, response: { data: { name: imgUrls[i].name, url: imgUrls[i].url }}})
        }
      }
      return _uploadImglist
    },
    handleGetDisplayImgUrls(fileList) {
      var _displayImgUrls = []
      for (var i = 0; i < fileList.length; i++) {
        if (fileList[i].status === 'success') {
          _displayImgUrls.push({ name: fileList[i].response.data.name, url: fileList[i].response.data.url })
        }
      }
      return _displayImgUrls
    },
    handleRemoveByDisplayImgUrls(file, fileList) {
      this.uploadImglist = fileList
      this.form.displayImgUrls = this.handleGetDisplayImgUrls(fileList)
    },
    handleSuccessByDisplayImgUrls(response, file, fileList) {
      this.uploadImglist = fileList
      this.form.displayImgUrls = this.handleGetDisplayImgUrls(fileList)
    },
    handleErrorByDisplayImgUrls(errs, file, fileList) {
      this.uploadImglist = fileList
      this.form.displayImgUrls = this.handleGetDisplayImgUrls(fileList)
    },
    handlePreviewByDisplayImgUrls(file) {
      this.uploadImgPreImgDialogUrlByDisplayImgUrls = file.url
      this.uploadImgPreImgDialogVisibleByDisplayImgUrls = true
    },
    handleBeforeUploadByDisplayImgUrls(file) {
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
    setUploadImgSortByDisplayImgUrls() {
      var _this = this
      const $ul = _this.$refs.uploadImgByDisplayImgUrls.$el.querySelectorAll('.el-upload-list')[0]
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
          var item = _this.uploadImglistByDisplayImgUrls.splice(oldIndex, 1)
          _this.uploadImglistByDisplayImgUrls.splice(newIndex, 0, item[0])

          _this.form.displayImgUrls = _this.handleGetDisplayImgUrls(_this.uploadImglistByDisplayImgUrls)
        // 下一个tick就会走patch更新
        },
        animation: 150
      })
    },
    handleGetDetailsDes(fileList) {
      var _imgUrls = []
      for (var i = 0; i < fileList.length; i++) {
        if (fileList[i].status === 'success') {
          _imgUrls.push({ name: fileList[i].response.data.name, url: fileList[i].response.data.url })
        }
      }
      return _imgUrls
    },
    handleRemoveByDetailsDes(file, fileList) {
      this.uploadImglistByDetailsDes = fileList
      this.form.detailsDes = this.handleGetDetailsDes(fileList)
    },
    handleSuccessByDetailsDes(response, file, fileList) {
      this.uploadImglistByDetailsDes = fileList
      this.form.detailsDes = this.handleGetDetailsDes(fileList)
    },
    handleErrorByDetailsDes(errs, file, fileList) {
      this.uploadImglistByDetailsDes = fileList
      this.form.detailsDes = this.handleGetDetailsDes(fileList)
    },
    handlePreviewByDetailsDes(file) {
      this.uploadImgPreImgDialogUrlByDetailsDes = file.url
      this.uploadImgPreImgDialogVisibleByDetailsDes = true
    },
    handleBeforeUploadByDetailsDes(file) {
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
    setUploadImgSortByDetailsDes() {
      var _this = this
      const $ul = _this.$refs.uploadImgByDetailsDes.$el.querySelectorAll('.el-upload-list')[0]
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
          var item = _this.uploadImglistByDetailsDes.splice(oldIndex, 1)
          _this.uploadImglistByDetailsDes.splice(newIndex, 0, item[0])

          _this.form.detailsDes = _this.handleGetDisplayImgUrls(_this.uploadImglistByDetailsDes)
        // 下一个tick就会走patch更新
        },
        animation: 150
      })
    },
    charTagsHandleClose(tag) {
      this.form.charTags.splice(this.form.charTags.indexOf(tag), 1)
    },
    charTagsShowInput() {
      this.charTagsInputVisible = true
      this.$nextTick(_ => {
        this.$refs.saveTagInput.$refs.input.focus()
      })
    },
    charTagsHandleInputConfirm() {
      const inputValue = this.charTagsInputValue

      // if (inputValue === '') {

      if (inputValue !== '' && this.form.charTags.length <= 2) {
        this.form.charTags.push(inputValue)
      } else if (inputValue !== '' && this.form.charTags.length >= 3) {
        this.$message('最多输入3个特色标签')
      }

      this.charTagsInputVisible = false
      this.charTagsInputValue = ''
    },
    searchSupplier(query) {
      if (query != null && query !== '') {
        this.supplier_option_loading = true

        supplier.search({ key: query }).then(res => {
          if (res.result === 1) {
            var d = res.data
            this.supplier_options = d.items
          }
          this.supplier_option_loading = false
        })
      } else {
        this.supplier_options = []
      }
    }
  }
}
</script>

<style lang="scss" scoped>
#product_baseinfo {
  .el-form .el-form-item {
    max-width: 600px;
  }

  .el-upload-list >>> .sortable-ghost {
    opacity: 0.8;
    color: #fff !important;
    background: #42b983 !important;
  }

  .el-upload-list >>> .el-tag {
    cursor: pointer;
  }

  .el-tag {
    margin-right: 10px;
  }
  .button-new-tag {
    height: 32px;
    line-height: 30px;
    padding-top: 0;
    padding-bottom: 0;
  }
  .input-new-tag {
    width: 90px;
    margin-right: 10px;
    vertical-align: bottom;
  }
}
</style>

