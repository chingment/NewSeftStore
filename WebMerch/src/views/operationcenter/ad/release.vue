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
      <el-form-item label="文件" prop="fileUrls">
        <el-input :value="form.fileUrls.toString()" style="display:none" />
        <el-upload
          ref="uploadImg"
          :action="uploadServiceUrl"
          list-type="picture-card"
          :before-upload="onFileUploadBefore"
          :on-success="onFileUploadSuccess"
          :on-remove="onFileUploadRemove"
          :on-error="onFileUploadError"
          :limit="1"
        >
          <i class="el-icon-plus" />
          <div slot="file" slot-scope="{file}">
            <img
              v-if="file.name.indexOf('png')>-1||file.name.indexOf('jpg')>-1"
              class="el-upload-list__item-thumbnail"
              :src="file.url"
              alt=""
            >
            <video
              v-else-if="file.name.indexOf('mp4')>-1"
              :src="file.url"
              class="el-upload-list__item-thumbnail"
              controls="controls"
            />
            <span class="el-upload-list__item-actions">
              <span
                class="el-upload-list__item-preview"
                @click="onFileUploadPreview(file)"
              >
                <i class="el-icon-zoom-in" />
              </span>
              <span
                class="el-upload-list__item-delete"
                @click="onFileUploadRemove(file)"
              >
                <i class="el-icon-delete" />
              </span>
            </span>
          </div>

        </el-upload>
        <el-dialog :visible.sync="uploadPreDialogVisible">
          <img v-if="uploadPreDialogFileName.indexOf('png')>-1||uploadPreDialogFileName.indexOf('jpg')>-1" width="100%" :src="uploadPreDialogFileUrl" alt="">
          <video
            v-else-if="uploadPreDialogFileName.indexOf('mp4')>-1"
            :src="uploadPreDialogFileUrl"
            controls="controls"
          />
        </el-dialog>
        <div class="remark-tip"><span class="sign">*注</span>：{{ temp.adSpaceDescription }}</div>
      </el-form-item>
      <el-form-item label="有效期" prop="validDate">
        <el-date-picker
          v-model="form.validDate"
          type="daterange"
          range-separator="至"
          start-placeholder="开始日期"
          end-placeholder="结束日期"
          value-format="yyyy-MM-dd"
          style="width: 380px"
        />
      </el-form-item>
      <el-form-item label="对象" prop="belongIds">
        <el-checkbox v-model="belongsCheckAll" :indeterminate="belongsIsIndeterminate" @change="onBelongsCheckAllChange">全选</el-checkbox>
        <div style="margin: 15px 0;" />
        <el-checkbox-group v-model="form.belongIds" @change="onBelongsCheckedChange">
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
import { release, initRelease } from '@/api/ad'
import { getUrlParam } from '@/utils/commonUtil'
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
        adSpaceSupportFormat: '',
        belongs: [],
        fileType: ''
      },
      form: {
        adSpaceId: 0,
        title: '',
        belongIds: [],
        validDate: [],
        fileUrls: []
      },
      rules: {
        title: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }],
        fileUrls: [{ type: 'array', required: true, message: '至多上传一个文件', max: 1 }],
        belongIds: [{ type: 'array', required: true, message: '至少选择一个对象', min: 1 }],
        validDate: [{ type: 'array', required: true, message: '请选择有效期' }]
      },
      belongsCheckAll: false,
      belongsIsIndeterminate: true,
      uploadFilelist: [],
      uploadPreDialogFileName: '',
      uploadPreDialogFileUrl: '',
      uploadPreDialogVisible: false,
      uploadServiceUrl: process.env.VUE_APP_UPLOADIMGSERVICE_URL
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      this.loading = true
      var id = getUrlParam('id')

      initRelease({ adSpaceId: id }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.form.adSpaceId = d.adSpaceId
          this.temp.adSpaceName = d.adSpaceName
          this.temp.adSpaceDescription = d.adSpaceDescription
          this.temp.adSpaceSupportFormat = d.adSpaceSupportFormat
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
              if (res.result === 1) {
                this.$message({
                  message: res.message,
                  type: 'success'
                })
                this.$router.push({
                  path: '/operationcenter/ad/contents?id=' + this.form.adSpaceId
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
    onBelongsCheckAllChange(val) {
      var belongsChecked = []
      for (var i = 0; i < this.temp.belongs.length; i++) {
        belongsChecked.push(this.temp.belongs[i].id)
      }
      this.form.belongIds = val ? belongsChecked : []
      this.belongsIsIndeterminate = false
    },
    onBelongsCheckedChange(value) {
      const checkedCount = value.length
      this.belongsCheckAll = checkedCount === this.temp.belongs.length
      this.belongsIsIndeterminate = checkedCount > 0 && checkedCount < this.temp.belongs.length
    },
    getdisplayImgUrls(fileList) {
      var _fileUrls = []
      for (var i = 0; i < fileList.length; i++) {
        if (fileList[i].status === 'success') {
          _fileUrls.push({ name: fileList[i].response.data.name, url: fileList[i].response.data.url })
        }
      }
      return _fileUrls
    },
    onFileUploadBefore(file) {
      if (this.temp.adSpaceSupportFormat === null || this.temp.adSpaceSupportFormat === '') {
        this.$message({
          message: '未设置文件格式',
          type: 'error'
        })
        return false
      }

      if (this.temp.adSpaceSupportFormat.indexOf(file.type) === -1) {
        this.$message({
          message: '不支持该文件格式',
          type: 'error'
        })
        return false
      }
    },
    onFileUploadRemove(file) {
      if (this.uploadFilelist != null) {
        for (var i = 0; i < this.uploadFilelist.length; i++) {
          if (this.uploadFilelist[i].uid === file.uid) {
            this.uploadFilelist.splice(i, 1)
          }
        }
      }
      this.form.fileUrls = this.getdisplayImgUrls(this.uploadFilelist)
      if (this.form.fileUrls == null || this.form.fileUrls.length === 0) {
        var var1 = document.querySelector('.el-upload')
        var1.style.display = 'block'
      }
    },
    onFileUploadSuccess(response, file, fileList) {
      this.uploadFilelist = fileList
      this.form.fileUrls = this.getdisplayImgUrls(fileList)
      if (this.form.fileUrls.length === 1) {
        var var1 = document.querySelector('.el-upload')
        var1.style.display = 'none'
      }
    },
    onFileUploadError(errs, file, fileList) {
      this.uploadFilelist = fileList
      this.form.fileUrls = this.getdisplayImgUrls(fileList)
    },
    onFileUploadPreview(file) {
      this.uploadPreDialogFileName = file.name
      this.uploadPreDialogFileUrl = file.url
      this.uploadPreDialogVisible = true
    }
  }
}
</script>

<style  lang="scss"  scoped>

#adspace_release{
   max-width: 600px;

}
</style>

