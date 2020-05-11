<template>
  <div id="productsku_edit" class="app-container">
    <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="100px">
      <el-form-item label="名称" prop="name">
        <el-input v-model="form.name" clearable />
      </el-form-item>
      <el-form-item label="编码" prop="singleSkuCumCode">
        <el-input v-model="form.singleSkuCumCode" clearable />
      </el-form-item>
      <el-form-item label="条形码" prop="singleSkuBarCode">
        <el-input v-model="form.singleSkuBarCode" clearable />
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
          <img width="100%" :src="uploadImgPreImgDialogUrlByDisplayImgUrls" alt="">
        </el-dialog>
        <div class="remark-tip"><span class="sign">*注</span>：图片500*500，格式（jpg,png）不超过4M；第一张为主图，可拖动改变图片顺序</div>
      </el-form-item>
      <el-form-item label="所属分类" prop="kindIds">
        <el-input :value="form.kindIds.toString()" style="display:none" />
        <treeselect
          v-model="form.kindIds"
          :multiple="true"
          :options="treeselect_kind_options"
          :normalizer="treeselect_kind_normalizer"
          :flat="true"
          sort-value-by="INDEX"
          :default-expand-level="99"
          placeholder="选择"
          no-children-text=""
        />
      </el-form-item>
      <el-form-item label="默认销售价" prop="singleSkuSalePrice">
        <el-input v-model="form.singleSkuSalePrice" style="width:160px">
          <template slot="prepend">￥</template>
        </el-input>

        <el-checkbox v-model="form.isUnifyUpdateSalePrice">立即统一更新</el-checkbox>
        <div class="remark-tip" style="line-height:24px;"><span class="sign">*注</span>：该价格作为默认销售价，单独修改店铺价格可点击在售店铺里更新 或 统一修改可勾选立即统一更新</div>
      </el-form-item>
      <el-form-item label="规格" prop="singleSkuSpecDes">
        <el-input v-model="form.singleSkuSpecDes" clearable />
      </el-form-item>
      <el-form-item label="简短描述" style="max-width:1000px">
        <el-input v-model="form.briefDes" maxlength="200" clearable />
      </el-form-item>
      <el-form-item label="详情图片" prop="detailsDes">
        <el-input :value="form.detailsDes==null?'':form.detailsDes.toString()" style="display:none" />
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
          <img width="100%" :src="uploadImgPreImgDialogUrlByDetailsDes" alt="">
        </el-dialog>
        <div class="remark-tip"><span class="sign">*注</span>：图片不超过4M；可拖动改变图片顺序</div>

      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="onSubmit">保存</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>

import { MessageBox } from 'element-ui'
import { edit, initEdit } from '@/api/prdproduct'
import fromReg from '@/utils/formReg'
import { goBack, getUrlParam, treeselectNormalizer } from '@/utils/commonUtil'
import Treeselect from '@riophae/vue-treeselect'
import '@riophae/vue-treeselect/dist/vue-treeselect.css'

import Sortable from 'sortablejs'
export default {
  components: { Treeselect },
  data() {
    return {
      loading: false,
      form: {
        id: '',
        name: '',
        kindIds: [],
        subjectIds: [],
        detailsDes: [],
        briefDes: '',
        displayImgUrls: [],
        singleSkuCumCode: '',
        singleSkuBarCode: '',
        singleSkuSalePrice: 0,
        singleSkuSpecDes: '',
        isUnifyUpdateSalePrice: false
      },
      rules: {
        name: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }],
        singleSkuCumCode: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }],
        singleSkuBarCode: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }],
        // kindIds: [{ type: 'array', required: true, message: '至少必选一个,且必须少于3个', max: 3 }],
        displayImgUrls: [{ type: 'array', required: true, message: '至少上传一张,且必须少于5张', max: 4 }],
        singleSkuSalePrice: [{ required: true, message: '金额格式,eg:88.88', pattern: fromReg.money }],
        singleSkuSpecDes: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }],
        detailsDes: [{ type: 'array', required: false, message: '不能超过3张', max: 3 }]
      },
      treeselect_kind_normalizer: treeselectNormalizer,
      treeselect_kind_options: [],
      treeselect_subject_normalizer: treeselectNormalizer,
      treeselect_subject_options: [],
      uploadImglistByDisplayImgUrls: [],
      uploadImgPreImgDialogUrlByDisplayImgUrls: '',
      uploadImgPreImgDialogVisibleByDisplayImgUrls: false,
      uploadImgPmsByByDisplayImgUrls: { folder: 'product', isBuildms: 'true' },
      uploadImgServiceUrl: process.env.VUE_APP_UPLOADIMGSERVICE_URL,
      uploadImglistByDetailsDes: [],
      uploadImgPreImgDialogUrlByDetailsDes: '',
      uploadImgPreImgDialogVisibleByDetailsDes: false,
      uploadImgPmsByByDetailsDes: { folder: 'product', isBuildms: 'false' }
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
          this.form.kindIds = d.kindIds
          this.form.detailsDes = d.detailsDes
          this.form.briefDes = d.briefDes
          this.form.displayImgUrls = d.displayImgUrls

          this.form.singleSkuId = d.skus[0].id
          this.form.singleSkuBarCode = d.skus[0].barCode
          this.form.singleSkuCumCode = d.skus[0].cumCode
          this.form.singleSkuSalePrice = d.skus[0].salePrice
          this.form.singleSkuSpecDes = d.skus[0].specDes

          this.uploadImglistByDisplayImgUrls = this.getUploadImglist(d.displayImgUrls)
          this.uploadImglistByDetailsDes = this.getUploadImglist(d.detailsDes)
          this.treeselect_subject_options = d.subjects
          this.treeselect_kind_options = d.kinds
        }
        this.loading = false
      })
    },
    resetForm() {

    },
    onSubmit() {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          var skus = []
          skus.push({ id: this.form.singleSkuId, specDes: this.form.singleSkuSpecDes, salePrice: this.form.singleSkuSalePrice, barCode: this.form.singleSkuBarCode, cumCode: this.form.singleSkuCumCode })
          var _form = {}
          _form.id = this.form.id
          _form.name = this.form.name
          _form.kindIds = this.form.kindIds
          _form.detailsDes = this.form.detailsDes
          _form.briefDes = this.form.briefDes
          _form.displayImgUrls = this.form.displayImgUrls
          _form.isUnifyUpdateSalePrice = this.form.isUnifyUpdateSalePrice
          _form.skus = skus

          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            edit(_form).then(res => {
              this.$message(res.message)
              if (res.result === 1) {
                goBack(this)
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
    }
  }
}
</script>

<style lang="scss" scoped>

#productsku_edit{
.el-form .el-form-item{
  max-width: 600px;
}

.el-upload-list >>> .sortable-ghost {
  opacity: .8;
  color: #fff!important;
  background: #42b983!important;
}

.el-upload-list >>> .el-tag {
  cursor: pointer;
}
}

</style>

