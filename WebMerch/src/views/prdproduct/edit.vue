<template>
  <div id="productsku_edit" class="app-container">
    <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="80px">
      <el-form-item label="名称" prop="name">
        <el-input v-model="form.name" />
      </el-form-item>
      <el-form-item label="条形码" prop="singleSkuBarCode">
        <el-input v-model="form.singleSkuBarCode" />
      </el-form-item>
      <el-form-item label="图片" prop="displayImgUrls">
        <el-input :value="form.displayImgUrls.toString()" style="display:none" />
        <el-upload
          ref="uploadImg"
          v-model="form.displayImgUrls"
          :action="uploadImgServiceUrl"
          list-type="picture-card"
          :on-success="handleSuccess"
          :on-remove="handleRemove"
          :on-error="handleError"
          :on-preview="handlePreview"
          :file-list="uploadImglist"
          :limit="4"
        >
          <i class="el-icon-plus" />
        </el-upload>
        <el-dialog :visible.sync="uploadImgPreImgDialogVisible">
          <img width="100%" :src="uploadImgPreImgDialogUrl" alt="">
        </el-dialog>
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
      <el-form-item label="销售价" prop="singleSkuSalePrice">
        <el-input v-model="form.singleSkuSalePrice" style="width:160px">
          <template slot="prepend">￥</template>
        </el-input>
      </el-form-item>
      <el-form-item label="规格" prop="singleSkuSpecDes">
        <el-input v-model="form.singleSkuSpecDes" />
      </el-form-item>
      <el-form-item label="简短描述" style="max-width:1000px">
        <el-input v-model="form.briefDes" maxlength="200" />
      </el-form-item>
      <el-form-item label="商品详情" style="max-width:1000px">
        <Editor id="tinymce" v-model="form.detailsDes" :init="tinymce_init" />
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

import tinymce from 'tinymce/tinymce'
import Editor from '@tinymce/tinymce-vue'
import 'tinymce/themes/silver/theme'

import 'tinymce/plugins/image'// 插入上传图片插件
import 'tinymce/plugins/media'// 插入视频插件
import 'tinymce/plugins/table'// 插入表格插件
import 'tinymce/plugins/link' // 超链接插件
import 'tinymce/plugins/code' // 代码块插件
import 'tinymce/plugins/lists'// 列表插件
import 'tinymce/plugins/contextmenu' // 右键菜单插件
import 'tinymce/plugins/wordcount' // 字数统计插件
import 'tinymce/plugins/colorpicker' // 选择颜色插件
import 'tinymce/plugins/textcolor' // 文本颜色插件
import 'tinymce/plugins/fullscreen' // 全屏
import 'tinymce/plugins/help'
import 'tinymce/plugins/charmap'
import 'tinymce/plugins/paste'
import 'tinymce/plugins/print'
import 'tinymce/plugins/preview'
import 'tinymce/plugins/hr'
import 'tinymce/plugins/anchor'
import 'tinymce/plugins/pagebreak'
import 'tinymce/plugins/spellchecker'
import 'tinymce/plugins/searchreplace'
import 'tinymce/plugins/visualblocks'
import 'tinymce/plugins/visualchars'
import 'tinymce/plugins/insertdatetime'
import 'tinymce/plugins/nonbreaking'
import 'tinymce/plugins/autosave'
import 'tinymce/plugins/fullpage'
import 'tinymce/plugins/toc'
import Sortable from 'sortablejs'
export default {
  components: { Treeselect, Editor },
  data() {
    return {
      loading: false,
      form: {
        id: '',
        name: '',
        kindIds: [],
        subjectIds: [],
        detailsDes: '',
        briefDes: '',
        displayImgUrls: [],
        singleSkuBarCode: '',
        singleSkuSalePrice: 0,
        singleSkuSpecDes: ''
      },
      rules: {
        name: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }],
        singleSkuBarCode: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }],
        kindIds: [{ type: 'array', required: true, message: '至少必选一个,且必须少于3个', max: 3 }],
        displayImgUrls: [{ type: 'array', required: true, message: '至少上传一张,且必须少于5张', max: 4 }],
        singleSkuSalePrice: [{ required: true, message: '金额格式,eg:88.88', pattern: fromReg.money }],
        singleSkuSpecDes: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }],
        briefDes: [{ required: false, min: 0, max: 200, message: '不能超过200个字符', trigger: 'change' }]
      },
      uploadImglist: [],
      uploadImgPreImgDialogUrl: '',
      uploadImgPreImgDialogVisible: false,
      uploadImgServiceUrl: process.env.VUE_APP_UPLOADIMGSERVICE_URL,
      treeselect_kind_normalizer: treeselectNormalizer,
      treeselect_kind_options: [],
      treeselect_subject_normalizer: treeselectNormalizer,
      treeselect_subject_options: [],
      tinymce_init: {
        language_url: '/static/tinymce/langs/zh_CN.js', // 语言包的路径
        language: 'zh_CN', // 语言
        height: 430,
        skin_url: '/static/tinymce/skins/ui/oxide',
        images_upload_url: process.env.VUE_APP_UPLOADIMGSERVICE_URL,
        menubar: false, // 隐藏最上方menu菜单
        browser_spellcheck: true, // 拼写检查
        branding: false, // 去水印
        statusbar: false, // 隐藏编辑器底部的状态栏
        elementpath: false, // 禁用下角的当前标签路径
        paste_data_images: true, // 允许粘贴图像
        plugins: 'lists image media table wordcount code fullscreen help  toc fullpage autosave nonbreaking insertdatetime visualchars visualblocks searchreplace spellchecker pagebreak link charmap paste print preview hr anchor',
        toolbar: [
          'newdocument undo redo | formatselect visualaid|cut copy paste selectall| bold italic underline strikethrough |codeformat blockformats| superscript subscript  | forecolor backcolor | alignleft aligncenter alignright alignjustify | outdent indent |  removeformat ',
          'code  bullist numlist | lists image media table link |fullscreen help toc fullpage restoredraft nonbreaking insertdatetime visualchars visualblocks searchreplace spellchecker pagebreak anchor charmap  pastetext print preview hr'
        ],
        images_upload_handler: (blobInfo, success, failure) => {
          const img = 'data:image/jpeg;base64,' + blobInfo.base64()
          success(img)
        }
      }
    }
  },
  mounted() {
    tinymce.init({})
    this.setUploadImgSort()
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
          this.form.singleSkuSalePrice = d.skus[0].salePrice
          this.form.singleSkuSpecDes = d.skus[0].specDes

          this.uploadImglist = this.getUploadImglist(d.displayImgUrls)
          this.treeselect_subject_options = d.subjects
          this.treeselect_kind_options = d.kinds
        }
        this.loading = false
      })
    },
    resetForm() {

    },
    onSubmit() {
      console.log(JSON.stringify(this.form))
      this.$refs['form'].validate((valid) => {
        if (valid) {
          var skus = []
          skus.push({ id: this.form.singleSkuId, specDes: this.form.singleSkuSpecDes, salePrice: this.form.singleSkuSalePrice, barCode: this.form.singleSkuBarCode })
          var _form = {}
          _form.id = this.form.id
          _form.name = this.form.name
          _form.kindIds = this.form.kindIds
          _form.detailsDes = this.form.detailsDes
          _form.briefDes = this.form.briefDes
          _form.displayImgUrls = this.form.displayImgUrls
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
    handleRemove(file, fileList) {
      this.uploadImglist = fileList
      this.form.displayImgUrls = this.getdisplayImgUrls(fileList)
    },
    handleSuccess(response, file, fileList) {
      this.uploadImglist = fileList
      this.form.displayImgUrls = this.getdisplayImgUrls(fileList)
    },
    handleError(errs, file, fileList) {
      this.uploadImglist = fileList
      this.form.displayImgUrls = this.getdisplayImgUrls(fileList)
    },
    handlePreview(file) {
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

          _this.form.displayImgUrls = _this.getdisplayImgUrls(_this.uploadImglist)
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

