<template>
  <div id="adminuser_edit">
    <page-header />
    <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="80px">
      <el-form-item label="用户名" prop="userName">
        {{ form.userName }}
      </el-form-item>
      <el-form-item v-show="!isOpenEditPassword" label="密码">
        <span>********</span>
        <span @click="openEditPassword()">修改</span>
      </el-form-item>
      <el-form-item v-show="isOpenEditPassword" label="密码" prop="password">
        <div style="display:flex">
          <div style="flex:1">
            <el-input v-model="form.password" type="password" clearable />
          </div>
          <div style="width:50px;text-align: center;">
            <span @click="openEditPassword()">取消</span>
          </div>
        </div>
      </el-form-item>
      <el-form-item label="头像" prop="avatar">
        <el-input :value="form.avatar" style="display:none" />
        <el-upload
          ref="uploadImg"
          :action="uploadImgServiceUrl"
          list-type="picture-card"
          :on-success="handleSuccessByAvatar"
          :on-remove="handleRemoveByAvatar"
          :on-error="handleErrorByAvatar"
          :on-preview="handlePreviewByAvatar"
          :file-list="uploadImglistByAvatar"
          :limit="1"
        >
          <i class="el-icon-plus" />
        </el-upload>
        <el-dialog :visible.sync="uploadImgPreImgDialogVisibleByAvatar">
          <img width="100%" :src="uploadImgPreImgDialogUrlByAvatar" alt="">
        </el-dialog>
        <div class="remark-tip"><span class="sign">*注</span>：格式为500*500</div>
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
      <el-form-item label="禁用">
        <el-switch v-model="form.isDisable" />
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
import { edit, initEdit } from '@/api/adminuser'
import fromReg from '@/utils/formReg'
import { getUrlParam, goBack } from '@/utils/commonUtil'
import PageHeader from '@/components/PageHeader/index.vue'
export default {
  name: 'SettingAdminUserEdit',
  components: {
    PageHeader
  },
  data() {
    return {
      loading: false,
      isOpenEditPassword: false,
      form: {
        id: '',
        userName: '',
        password: '',
        fullName: '',
        nickName: '',
        phoneNumber: '',
        email: '',
        avatar: '',
        orgIds: [],
        roleIds: [],
        imIsUse: false,
        isDisable: false,
        workBench: 1
      },
      rules: {
        password: [{ required: false, message: '必填,且由6到20个数字、英文字母或下划线组成', trigger: 'change', pattern: fromReg.password }],
        avatar: [{ required: true, message: '必须上传' }],
        nickName: [{ required: true, message: '必填', trigger: 'change' }],
        orgIds: [{ required: true, message: '必选' }],
        phoneNumber: [{ required: false, message: '格式错误,eg:13800138000', trigger: 'change', pattern: fromReg.phoneNumber }],
        email: [{ required: false, message: '格式错误,eg:xxxx@xxx.xxx', trigger: 'change', pattern: fromReg.email }]
      },
      cascader_org_props: { multiple: true, checkStrictly: true, emitPath: false },
      cascader_org_options: [],
      checkbox_group_role_options: [],
      uploadImglistByAvatar: [],
      uploadImgPreImgDialogUrlByAvatar: '',
      uploadImgPreImgDialogVisibleByAvatar: false,
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
      initEdit({ id: id }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.form.id = d.id
          this.form.userName = d.userName
          this.form.fullName = d.fullName
          this.form.nickName = d.nickName
          this.form.phoneNumber = d.phoneNumber
          this.form.email = d.email
          this.form.orgIds = d.orgIds
          this.form.roleIds = d.roleIds
          this.form.imIsUse = d.imIsUse
          this.form.avatar = d.avatar
          this.form.isDisable = d.isDisable
          this.form.workBench = d.workBench
          this.cascader_org_options = d.orgs
          this.checkbox_group_role_options = d.roles

          if (d.avatar != null && d.avatar.length > 0) {
            this.uploadImglistByAvatar.push({ name: 'xx', url: d.avatar })
            var var1 = document.querySelector('.el-upload')
            var1.style.display = 'none'
          }
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
    },
    openEditPassword() {
      if (this.isOpenEditPassword) {
        this.isOpenEditPassword = false
        this.form.password = ''
        this.rules.password[0].required = false
      } else {
        this.isOpenEditPassword = true
        this.rules.password[0].required = true
      }
    },
    cascader_org_change() {

    },
    handleRemoveByAvatar(file, fileList) {
      this.uploadImglistByAvatar = fileList
      this.form.avatar = ''
      var var1 = document.querySelector('.el-upload')
      var1.style.display = 'block'
    },
    handleSuccessByAvatar(response, file, fileList) {
      this.uploadImglistByAvatar = fileList
      this.form.avatar = file.response.data.url
      var var1 = document.querySelector('.el-upload')
      var1.style.display = 'none'
    },
    handleErrorByAvatar(errs, file, fileList) {
      this.uploadImglistByAvatar = fileList
    },
    handlePreviewByAvatar(file) {
      this.uploadImgPreImgDialogUrlByAvatar = file.url
      this.uploadImgPreImgDialogVisibleByAvatar = true
    }
  }
}
</script>

<style lang="scss" scoped>

#adminuser_edit
{
   max-width: 600px;

.line {
  text-align: center;
}
.el-tree-node__expand-icon.is-leaf{
  display: none;
}
}
</style>

