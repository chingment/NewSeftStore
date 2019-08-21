<template>
  <div id="store_add" class="app-container">
    <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="80px">
      <el-form-item label="店铺名称" prop="name">
        <el-input v-model="form.name" />
      </el-form-item>
      <el-form-item label="联系地址" prop="name">
        <el-input v-model="form.address" />
      </el-form-item>
      <el-form-item label="图片" prop="dispalyImgUrls">
        <el-input :value="form.dispalyImgUrls.toString()" style="display:none" />
        <el-upload
          ref="uploadImg"
          v-model="form.dispalyImgUrls"
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
        <div class="remark-tip"><span class="sign">*注</span>：第一张默认为主图，可拖动改变图片顺便</div>
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
import { addStore, initAddStore } from '@/api/store'
import { goBack } from '@/utils/commonUtil'
import Sortable from 'sortablejs'

export default {
  data() {
    return {
      loading: false,
      form: {
        name: '',
        address: '',
        briefDes: '',
        dispalyImgUrls: []
      },
      rules: {
        name: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }],
        address: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }],
        dispalyImgUrls: [{ type: 'array', required: true, message: '至少上传一张,且必须少于5张', max: 4 }],
        briefDes: [{ required: false, min: 0, max: 200, message: '不能超过200个字符', trigger: 'change' }]
      },
      uploadImglist: [],
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
      initAddStore().then(res => {
        if (res.result === 1) {
          var d = res.data
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
            addStore(this.form).then(res => {
              this.$message(res.message)
              if (res.result === 1) {
                goBack(this)
              }
            })
          })
        }
      })
    },
    getDispalyImgUrls(fileList) {
      var _dispalyImgUrls = []
      for (var i = 0; i < fileList.length; i++) {
        if (fileList[i].status === 'success') {
          _dispalyImgUrls.push({ name: fileList[i].response.data.name, url: fileList[i].response.data.url })
        }
      }
      return _dispalyImgUrls
    },
    handleRemove(file, fileList) {
      this.uploadImglist = fileList
      this.form.dispalyImgUrls = this.getDispalyImgUrls(fileList)
    },
    handleSuccess(response, file, fileList) {
      this.uploadImglist = fileList
      this.form.dispalyImgUrls = this.getDispalyImgUrls(fileList)
    },
    handleError(errs, file, fileList) {
      this.uploadImglist = fileList
      this.form.dispalyImgUrls = this.getDispalyImgUrls(fileList)
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

          _this.form.dispalyImgUrls = _this.getDispalyImgUrls(_this.uploadImglist)
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

