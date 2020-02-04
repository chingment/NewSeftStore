<template>
  <div id="store_add" class="app-container">
    <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="80px">
      <el-form-item label="店铺名称" prop="name">
        <el-input v-model="form.name" />
      </el-form-item>
      <el-form-item label="联系地址" prop="name">
        <el-input v-model="form.address" />
      </el-form-item>
      <el-form-item label="图片" prop="displayImgUrls">
        <el-input :value="form.displayImgUrls.toString()" style="display:none" />
        <el-upload
          ref="uploadImg"
          v-model="form.displayImgUrls"
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
        <el-input v-model="form.briefDes" type="text" maxlength="200" show-word-limit />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="onSubmit">保存</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>

import { MessageBox } from 'element-ui'
import { add, initAdd } from '@/api/store'
import { goBack } from '@/utils/commonUtil'
import Sortable from 'sortablejs'
import { all } from 'q'

export default {
  data() {
    return {
      loading: false,
      form: {
        name: '',
        address: '',
        briefDes: '',
        displayImgUrls: []
      },
      rules: {
        name: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }],
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
  mounted() {
    this.setUploadImgSort()
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      this.loading = true
      initAdd().then(res => {
        if (res.result === 1) {
          var d = res.data
          this.uploadCardCheckShow()
        }
        this.loading = false
      })
    },
    resetForm() {

    },
    onSubmit() {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            add(this.form).then(res => {
              this.$message(res.message)
              if (res.result === 1) {
                goBack(this)
              }
            })
          })
        }
      })
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
      if (this.form.displayImgUrls.length >= this.uploadImgMaxSize) {
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
      if (this.form.displayImgUrls.length === this.uploadImgMaxSize) {
        uploadcard[0].style.display = 'none'
      } else {
        uploadcard[0].style.display = 'inline-block'
      }
    },
    uploadRemoveHandle(file, fileList) {
      this.uploadImglist = fileList
      this.form.displayImgUrls = this.getdisplayImgUrls(fileList)
      this.uploadCardCheckShow()
    },
    uploadSuccessHandle(response, file, fileList) {
      this.uploadImglist = fileList
      this.form.displayImgUrls = this.getdisplayImgUrls(fileList)
      this.uploadCardCheckShow()
    },
    uploadErrorHandle(errs, file, fileList) {
      this.uploadImglist = fileList
      this.form.displayImgUrls = this.getdisplayImgUrls(fileList)
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

#store_add{
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

