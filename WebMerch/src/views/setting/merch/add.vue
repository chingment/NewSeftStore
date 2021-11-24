<template>
  <div id="merch_add">
    <page-header />
    <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="80px">
      <el-form-item label="商户名" prop="fullName">
        <el-input v-model="form.fullName" />
      </el-form-item>
      <el-form-item label="用户名" prop="userName">
        <el-input v-model="form.userName" />
      </el-form-item>
      <el-form-item label="密码" prop="password">
        <el-input v-model="form.password" type="password" />
      </el-form-item>
      <el-form-item label="手机号码" prop="phoneNumber">
        <el-input v-model="form.phoneNumber" />
      </el-form-item>
      <el-form-item label="邮箱" prop="email">
        <el-input v-model="form.email" />
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
import { add, initAdd } from '@/api/merch'
import fromReg from '@/utils/formReg'
import { goBack } from '@/utils/commonUtil'
import PageHeader from '@/components/PageHeader/index.vue'
export default {
  name: 'SettingMerchAdd',
  components: {
    PageHeader
  },
  data() {
    return {
      loading: false,
      form: {
        userName: '',
        password: '',
        fullName: '',
        phoneNumber: '',
        email: '',
        workBench: 1
      },
      rules: {
        fullName: [{ required: true, message: '必填', trigger: 'change' }],
        userName: [{ required: true, message: '必填,且由3到20个数字、英文字母或下划线组成', trigger: 'change', pattern: fromReg.userName }],
        password: [{ required: true, message: '必填,且由6到20个数字、英文字母或下划线组成', trigger: 'change', pattern: fromReg.password }],
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
      initAdd().then(res => {
        if (res.result === 1) {

        }
        this.loading = false
      })
    },
    resetForm() {
      this.form = {
        userName: '',
        password: '',
        fullName: '',
        phoneNumber: '',
        email: ''
      }
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
    }
  }
}
</script>

<style  lang="scss"  scoped>

#merch_add{
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

