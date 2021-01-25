<template>
  <div id="adspace_release">
    <page-header />
    <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="80px">
      <el-form-item label="所属版位">
        {{ temp.adSpaceName }}
      </el-form-item>
      <el-form-item label="标题" prop="title">
        <el-input v-model="form.title" clearable />
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
          :limit="1"
        >
          <i class="el-icon-plus" />
        </el-upload>
        <el-dialog :visible.sync="uploadImgPreImgDialogVisible">
          <img width="100%" :src="uploadImgPreImgDialogUrl" alt="">
        </el-dialog>
        <div class="remark-tip"><span class="sign">*注</span>：{{ temp.adSpaceDescription }}</div>
      </el-form-item>
      <el-form-item label="对象" prop="belongIds">
        <el-checkbox v-model="belongsCheckAll" :indeterminate="belongsIsIndeterminate" @change="handleBelongsCheckAllChange">全选</el-checkbox>
        <div style="margin: 15px 0;" />
        <el-checkbox-group v-model="form.belongIds" @change="handleBelongsCheckedChange">
          <el-checkbox v-for="(belong,index) in temp.belongs" :key="index" :label="belong.id">{{ belong.name }}</el-checkbox>
        </el-checkbox-group>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="onSubmit">发布</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { release, initRelease } from '@/api/adspace'
import fromReg from '@/utils/formReg'
import { goBack, getUrlParam } from '@/utils/commonUtil'
import PageHeader from '@/components/PageHeader/index.vue'
export default {
  name: 'OperationCenterAdspaceRelease',
  components: {
    PageHeader
  },
  data() {
    return {
      loading: false,
      temp: {
        adSpaceName: '',
        adSpaceDescription: '',
        belongs: []
      },
      form: {
        adSpaceId: 0,
        title: '',
        belongIds: [],
        displayImgUrls: []
      },
      rules: {
        title: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }],
        displayImgUrls: [{ type: 'array', required: true, message: '至多上传一张', max: 1 }],
        belongIds: [{ type: 'array', required: true, message: '至少选择一个对象', min: 1 }]
      },
      belongsCheckAll: false,
      belongsIsIndeterminate: true,
      uploadImglist: [],
      uploadImgPreImgDialogUrl: '',
      uploadImgPreImgDialogVisible: false,
      uploadImgServiceUrl: process.env.VUE_APP_UPLOADIMGSERVICE_URL
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      this.loading = true
      var id = getUrlParam('id')

      initRelease({ id: id }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.form.adSpaceId = d.adSpaceId
          this.temp.adSpaceName = d.adSpaceName
          this.temp.adSpaceDescription = d.adSpaceDescription
          this.temp.belongs = d.belongs
        }
        this.loading = false
      })
    },
    onSubmit() {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            release(this.form).then(res => {
              this.$message(res.message)
              if (res.result === 1) {
                goBack(this)
              }
            })
          })
        }
      })
    },
    handleBelongsCheckAllChange(val) {
      var belongsChecked = []
      for (var i = 0; i < this.temp.belongs.length; i++) {
        belongsChecked.push(this.temp.belongs[i].id)
      }
      this.form.belongIds = val ? belongsChecked : []
      this.belongsIsIndeterminate = false
    },
    handleBelongsCheckedChange(value) {
      const checkedCount = value.length
      this.belongsCheckAll = checkedCount === this.temp.belongs.length
      this.belongsIsIndeterminate = checkedCount > 0 && checkedCount < this.temp.belongs.length
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
      if (this.form.displayImgUrls.length === 0) {
        var var1 = document.querySelector('.el-upload')
        var1.style.display = 'block'
      }
    },
    handleSuccess(response, file, fileList) {
      this.uploadImglist = fileList
      this.form.displayImgUrls = this.getdisplayImgUrls(fileList)
      if (this.form.displayImgUrls.length === 1) {
        var var1 = document.querySelector('.el-upload')
        var1.style.display = 'none'
      }
    },
    handleError(errs, file, fileList) {
      this.uploadImglist = fileList
      this.form.displayImgUrls = this.getdisplayImgUrls(fileList)
    },
    handlePreview(file) {
      this.uploadImgPreImgDialogUrl = file.url
      this.uploadImgPreImgDialogVisible = true
    }
  }
}
</script>

<style  lang="scss"  scoped>

#adspace_release{
   max-width: 600px;

}
</style>

