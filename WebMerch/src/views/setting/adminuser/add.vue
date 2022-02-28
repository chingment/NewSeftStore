<template>
  <div id="adminuser_add">
    <page-header />
    <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="80px">
      <el-form-item label="用户名" prop="userName">
        <el-input v-model="form.userName" clearable />
      </el-form-item>
      <el-form-item label="密码" prop="password">
        <el-input v-model="form.password" type="password" clearable />
      </el-form-item>
      <el-form-item label="头像" prop="avatar" class="el-form-item-upload">
        <el-input :value="form.avatar.toString()" style="display:none" />
        <lm-upload
          v-model="form.avatar"
          list-type="picture-card"
          :file-list="form.avatar"
          :action="uploadFileServiceUrl"
          :headers="uploadFileHeaders"
          :data="{folder:'avatar'}"
          ext=".jpg,.png,.jpeg"
          tip="格式为500*500"
          :max-size="1024"
          :sortable="true"
          :limit="1"
        />
      </el-form-item>
      <el-form-item label="昵称" prop="nickName">
        <el-input v-model="form.nickName" clearable />
      </el-form-item>
      <el-form-item label="姓名" prop="fullName">
        <el-input v-model="form.fullName" clearable />
      </el-form-item>
      <el-form-item label="手机号码" prop="phoneNumber">
        <el-input v-model="form.phoneNumber" clearable />
      </el-form-item>
      <el-form-item label="邮箱" prop="email">
        <el-input v-model="form.email" clearable />
      </el-form-item>
      <el-form-item label="音视频">
        <el-switch v-model="form.imIsUse" />
      </el-form-item>
      <el-form-item v-show="checkbox_group_role_options.length>0" label="角色">
        <el-checkbox-group v-model="form.roleIds">
          <el-checkbox v-for="option in checkbox_group_role_options" :key="option.id" style="display:block" :label="option.id">{{ option.label }}</el-checkbox>
        </el-checkbox-group>
      </el-form-item>
      <el-form-item label="工作台">
        <el-radio-group v-model="form.workBench">
          <el-radio :label="1">商城</el-radio>
          <el-radio :label="2">心晓</el-radio>
        </el-radio-group>
        <div>
          <el-image :src="'http://file.17fanju.com/upload/WorkBench'+form.workBench+'.png'" />
        </div>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="onSubmit">保存</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { add, initAdd } from '@/api/adminuser'
import fromReg from '@/utils/formReg'
import { goBack } from '@/utils/commonUtil'
import PageHeader from '@/components/PageHeader/index.vue'
import LmUpload from '@/components/Upload/index.vue'
import { getToken } from '@/utils/auth'
export default {
  name: 'SettingAdminUserAdd',
  components: {
    PageHeader,
    LmUpload
  },
  data() {
    return {
      loading: false,
      form: {
        userName: '',
        password: '',
        fullName: '',
        nickName: '',
        phoneNumber: '',
        email: '',
        avatar: [],
        orgIds: [],
        roleIds: [],
        imIsUse: false,
        workBench: 1
      },
      rules: {
        userName: [{ required: true, message: '必填,且由3到20个数字、英文字母或下划线组成', trigger: 'change', pattern: fromReg.userName }],
        password: [{ required: true, message: '必填,且由6到20个数字、英文字母或下划线组成', trigger: 'change', pattern: fromReg.password }],
        avatar: [{ type: 'array', required: true, message: '必须上传', max: 1 }],
        nickName: [{ required: true, message: '必填', trigger: 'change' }],
        orgIds: [{ required: true, message: '必选' }],
        phoneNumber: [{ required: false, message: '格式错误,eg:13800138000', trigger: 'change', pattern: fromReg.phoneNumber }],
        email: [{ required: false, message: '格式错误,eg:xxxx@xxx.xxx', trigger: 'change', pattern: fromReg.email }]
      },
      cascader_org_props: { multiple: true, checkStrictly: true, emitPath: false },
      cascader_org_options: [],
      checkbox_group_role_options: [],
      uploadFileHeaders: {},
      uploadFileServiceUrl: process.env.VUE_APP_UPLOAD_FILE_SERVICE_URL
    }
  },
  created() {
    this.uploadFileHeaders = { 'X-Token': getToken() }
    this.init()
  },
  methods: {
    init() {
      this.loading = true
      initAdd().then(res => {
        if (res.result === 1) {
          var d = res.data
          this.cascader_org_options = d.orgs
          this.checkbox_group_role_options = d.roles
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
            add(this.form).then(res => {
              if (res.result === 1) {
                this.$message({
                  message: res.message,
                  type: 'success'
                })
                goBack(this)
              } else {
                this.$message({
                  message: res.message,
                  type: 'error'
                })
              }
            })
          }).catch(() => {
          })
        }
      })
    }
  }
}
</script>

<style  lang="scss"  scoped>

#adminuser_add{
   max-width: 600px;
.line {
  text-align: center;
}

.is-leaf{
  display: none !important;
  width: 0px !important;
}
}
</style>

