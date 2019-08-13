<template>
  <el-form ref="form" :model="userInfo" :rules="rules" >
    <el-form-item label="账号" style="margin-bottom: 0px">
      <div style="width:100%;display:inline-block">{{ userInfo.userName }}</div>
    </el-form-item>
    <el-form-item v-show="!isOpenEditPassword" label="密码" style="margin-bottom: 0px">
      <div style="width:100%;display:inline-block">
        <span>********</span>
        <span @click="openEditPassword()">修改</span>
      </div>
    </el-form-item>
    <el-form-item v-show="isOpenEditPassword" label="密码" prop="password">
        <div style="display:flex;width:100%">
          <div style="flex:1">
            <el-input v-model="userInfo.password" type="password" />
          </div>
          <div style="width:50px;text-align: center;">
            <span @click="openEditPassword()">取消</span>
          </div>
        </div>
    </el-form-item>
    <el-form-item label="姓名" prop="fullName">
      <el-input v-model.trim="userInfo.fullName" />
    </el-form-item>
    <el-form-item label="邮件" prop="email">
      <el-input v-model.trim="userInfo.email" />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="submit">保存</el-button>
    </el-form-item>
  </el-form>
</template>

<script>
import fromReg from '@/utils/formReg'
import { MessageBox } from 'element-ui'
import { save } from '@/api/userInfo'
export default {
  data() {
    return {
      isOpenEditPassword: false,
      rules: {
        password: [{ required: false, message: '必填,且由6到20个数字、英文字母或下划线组成', trigger: 'change', pattern: fromReg.password }],
        fullName: [{ required: true, message: '必填', trigger: 'change' }],
        email: [{ required: false, message: '格式错误,eg:xxxx@xxx.xxx', trigger: 'change', pattern: fromReg.email }]
      }
    }
  },
  props: {
    userInfo: {
      type: Object,
      default: () => {
        return {
          fullName: '',
          userName: '',
          phoneNumber: '',
          email: '',
          avatar: '',
          introduction: ''
        }
      }
    }
  },
  methods: {
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
    submit() {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            save(this.userInfo).then(res => {
              this.$message(res.message)
            })
          })
        }
      })
    }
  }
}
</script>
