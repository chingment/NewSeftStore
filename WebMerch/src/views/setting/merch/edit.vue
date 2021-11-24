<template>
  <div id="merch_edit">
    <page-header />
    <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="80px">
      <el-form-item label="商户名" prop="fullName">
        <el-input v-model="form.fullName" />
      </el-form-item>
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
            <el-input v-model="form.password" type="password" />
          </div>
          <div style="width:50px;text-align: center;">
            <span @click="openEditPassword()">取消</span>
          </div>
        </div>
      </el-form-item>
      <el-form-item label="手机号码" prop="phoneNumber">
        <el-input v-model="form.phoneNumber" />
      </el-form-item>
      <el-form-item label="邮箱" prop="email">
        <el-input v-model="form.email" />
      </el-form-item>
      <el-form-item label="禁用">
        <el-switch v-model="form.isDisable" />
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
import { edit, initEdit } from '@/api/merch'
import fromReg from '@/utils/formReg'
import { getUrlParam, goBack } from '@/utils/commonUtil'
import PageHeader from '@/components/PageHeader/index.vue'
export default {
  name: 'SettingMerchEdit',
  components: {
    PageHeader
  },
  data() {
    return {
      loading: false,
      isOpenEditPassword: false,
      form: {
        merchId: '',
        userName: '',
        password: '',
        fullName: '',
        phoneNumber: '',
        email: '',
        workBench: 1
      },
      rules: {
        password: [{ required: false, message: '必填,且由6到20个数字、英文字母或下划线组成', trigger: 'change', pattern: fromReg.password }],
        fullName: [{ required: true, message: '必填', trigger: 'change' }],
        phoneNumber: [{ required: false, message: '格式错误,eg:13800138000', trigger: 'change', pattern: fromReg.phoneNumber }],
        email: [{ required: false, message: '格式错误,eg:xxxx@xxx.xxx', trigger: 'change', pattern: fromReg.email }]
      }
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
          this.form.merchId = d.merchId
          this.form.userName = d.userName
          this.form.fullName = d.fullName
          this.form.phoneNumber = d.phoneNumber
          this.form.email = d.email
          this.form.workBench = d.workBench
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
                goBack(this)
              }
            })
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
    }
  }
}
</script>

<style  lang="scss"  scoped>

#merch_edit{
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
