<template>
  <div id="user_add" class="app-container">
    <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="80px">
      <el-form-item label="用户名" prop="userName">
        <el-input v-model="form.userName" />
      </el-form-item>
      <el-form-item label="密码" prop="password">
        <el-input v-model="form.password" type="password" />
      </el-form-item>
      <el-form-item label="姓名" prop="fullName">
        <el-input v-model="form.fullName" />
      </el-form-item>
      <el-form-item label="手机号码" prop="phoneNumber">
        <el-input v-model="form.phoneNumber" />
      </el-form-item>
      <el-form-item label="邮箱" prop="email">
        <el-input v-model="form.email" />
      </el-form-item>
      <el-form-item label="角色">
        <el-checkbox-group v-model="form.roleIds">
          <el-checkbox v-for="option in checkbox_group_role_options" :key="option.id" style="display:block" :label="option.id">{{ option.label }}</el-checkbox>
        </el-checkbox-group>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="onSubmit">保存</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
// https://element.eleme.cn/#/zh-CN/component/cascader
import { MessageBox } from 'element-ui'
import { add, initAdd } from '@/api/user'
import fromReg from '@/utils/formReg'
import { goBack } from '@/utils/commonUtil'
export default {
  data() {
    return {
      loading: false,
      form: {
        userName: '',
        password: '',
        fullName: '',
        phoneNumber: '',
        email: '',
        orgIds: [],
        roleIds: []
      },
      rules: {
        userName: [{ required: true, message: '必填,且由3到20个数字、英文字母或下划线组成', trigger: 'change', pattern: fromReg.userName }],
        password: [{ required: true, message: '必填,且由6到20个数字、英文字母或下划线组成', trigger: 'change', pattern: fromReg.password }],
        fullName: [{ required: true, message: '必填', trigger: 'change' }],
        orgIds: [{ required: true, message: '必选' }],
        phoneNumber: [{ required: false, message: '格式错误,eg:13800138000', trigger: 'change', pattern: fromReg.phoneNumber }],
        email: [{ required: false, message: '格式错误,eg:xxxx@xxx.xxx', trigger: 'change', pattern: fromReg.email }]
      },
      cascader_org_props: { multiple: true, checkStrictly: true, emitPath: false },
      cascader_org_options: [],
      checkbox_group_role_options: []
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
          var d = res.data
          this.cascader_org_options = d.orgs
          this.checkbox_group_role_options = d.roles
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

#user_add{
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

