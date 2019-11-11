<template>
  <div id="machine_baseinfo" v-loading="loading" class="app-container">

    <el-form v-show="!isEdit" class="noeditform" label-width="100px">
      <el-form-item label="机器编号">
        {{ temp.id }}
      </el-form-item>
      <el-form-item label="机器名称">
        {{ temp.name }}
      </el-form-item>
      <el-form-item label="控制程序号">
        {{ temp.ctrlSdkVersion }}
      </el-form-item>
      <el-form-item label="应用程序号">
        {{ temp.appVersion }}
      </el-form-item>
      <el-form-item label="机器状态">
        {{ temp.status.text }}
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="openEdit">编辑</el-button>
      </el-form-item>
    </el-form>

    <el-form v-show="isEdit" ref="form" v-loading="loading" :model="form" :rules="rules" label-width="100px">
      <el-form-item label="机器编号">
        {{ temp.id }}
      </el-form-item>
      <el-form-item label="机器名称" prop="name">
        <el-input v-model="form.name" />
      </el-form-item>
      <el-form-item label="控制程序号">
        {{ temp.ctrlSdkVersion }}
      </el-form-item>
      <el-form-item label="应用程序号">
        {{ temp.appVersion }}
      </el-form-item>
      <el-form-item label="机器状态">
        {{ temp.status.text }}
      </el-form-item>
      <el-form-item>
        <el-button type="info" @click="cancleEdit">取消</el-button>
        <el-button type="primary" @click="onSubmit">保存</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>
<script>

import { MessageBox } from 'element-ui'
import { edit, initManageBaseInfo } from '@/api/machine'
import { getUrlParam } from '@/utils/commonUtil'

export default {
  name: 'ManagePaneBaseInfo',
  data() {
    return {
      isEdit: false,
      loading: false,
      temp: {
        name: '',
        status: {
          text: '',
          value: ''
        }
      },
      form: {
        id: '',
        name: '',
        logoImgUrl: ''
      },
      rules: {
        name: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }],
        displayImgUrls: [{ type: 'array', required: true, message: '至少上传一张,且必须少于5张', max: 4 }]
      },
      uploadImglist: [],
      uploadImgPreImgDialogUrl: '',
      uploadImgPreImgDialogVisible: false,
      uploadImgServiceUrl: process.env.VUE_APP_UPLOADIMGSERVICE_URL
    }
  },
  watch: {
    '$route'(to, from) {
      this.init()
    }
  },
  mounted() {

  },
  created() {
    this.init()
  },
  methods: {
    init() {
      this.loading = true
      var id = getUrlParam('id')
      initManageBaseInfo({ id: id }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.form.id = d.id
          this.form.name = d.name

          this.temp.id = d.id
          this.temp.name = d.name
          this.temp.status = d.status
          this.temp.ctrlSdkVersion = d.ctrlSdkVersion
          this.temp.appVersion = d.appVersion
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
            edit(this.form).then(res => {
              this.$message(res.message)
              if (res.result === 1) {
                this.isEdit = false
                this.init()
              }
            })
          })
        }
      })
    },
    openEdit() {
      this.isEdit = true
    },
    cancleEdit() {
      this.isEdit = false
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
      this.form.logoImgUrl = URL.createObjectURL(file.raw)
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
<style lang="scss" scoped>

#machine_baseinfo{
.el-form .el-form-item{
  max-width: 600px;
}

.el-upload-list >>> .sortable-ghost {
  opacity: .8;
  color: #fff !important;
  background: #42b983 !important;
}

.el-upload-list >>> .el-tag {
  cursor: pointer;
}
}

</style>
