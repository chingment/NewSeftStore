<template>
  <div id="profile_userinfo">
    <div class="row-title clearfix">
      <div class="pull-left"> <h5>基本信息</h5>
      </div>
      <div class="pull-right" />
    </div>
    <el-form v-loading="loading" label-width="80px">
      <el-form-item label="用户名" prop="userName">
        <span>{{ userInfo.userName }}</span>
      </el-form-item>
      <el-form-item label="密码">
        <div v-if="!isOpenEditPassword">
          <span>********</span>
          <span class="i-btn-save" @click="onOpenEditPassword()">修改</span>
        </div>
        <div v-else style="display:flex;max-width:300px">
          <div style="flex:1">
            <el-input v-model="form_change_pwd.password" type="password" />
          </div>
          <div style="width:90px;text-align: center;">
            <span class="i-btn-save" @click="onSavePassword()"> 保存</span>
            <span class="i-btn-cancle" @click="onOpenEditPassword()">取消</span>
          </div>
        </div>
      </el-form-item>
      <el-form-item label="姓名" prop="fullName">
        <span>{{ userInfo.fullName }}</span>
      </el-form-item>
      <el-form-item label="手机号码" prop="phoneNumber">
        <span>{{ userInfo.phoneNumber }}</span>
      </el-form-item>
      <el-form-item label="邮箱" prop="email">
        <span>{{ userInfo.email }}</span>
      </el-form-item>
      <el-form-item label="所属角色" prop="email">
        <span>{{ userInfo.roleNames }}</span>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import store from '@/store'
import { MessageBox } from 'element-ui'
import { changePassword } from '@/api/own'
import fromReg from '@/utils/formReg'
export default {
  name: 'ProfileUserInfo',
  data() {
    return {
      loading: false,
      isOpenEditPassword: false,
      userInfo: {
        userName: '',
        fullName: '',
        phoneNumber: '',
        email: '',
        roleNames: ''
      },
      form_change_pwd: {
        password: ''
      }
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      var d = store.getters.userInfo
      this.userInfo.userName = d.userName
      this.userInfo.fullName = d.fullName
      this.userInfo.phoneNumber = d.phoneNumber
      this.userInfo.email = d.email

      var roleNames = '未分配角色'

      if (d.roles != null && d.roles.length > 0) {
        roleNames = ''
        for (var i = 0; i < d.roles.length; i++) {
          roleNames += d.roles[i].name + ','
        }

        roleNames = roleNames.substring(0, roleNames.length - 1)
      }

      this.userInfo.roleNames = roleNames
    },
    onSavePassword() {
      if (this.form_change_pwd.password == null || this.form_change_pwd.password == undefined || this.form_change_pwd.password == '') {
        this.$message('密码不能为空')

        return
      } else if (!this.form_change_pwd.password.match(fromReg.password)) {
        this.$message('密码且由6到20个数字、英文字母或下划线组成')

        return
      }

      MessageBox.confirm('确定要保存密码', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        var data = { password: this.form_change_pwd.password }
        this.loading = true
        changePassword(data).then(res => {
          if (res.result === 1) {
            this.$message({
              message: res.message,
              type: 'success'
            })
            this.isOpenEditPassword = false
          } else {
            this.$message({
              message: res.message,
              type: 'error'
            })
          }

          this.loading = false
        })
      }).catch(() => {
      })
    },
    onOpenEditPassword() {
      if (this.isOpenEditPassword) {
        this.isOpenEditPassword = false
        this.form_change_pwd.password = ''
      } else {
        this.isOpenEditPassword = true
      }
    }
  }
}
</script>

<style lang="scss" scoped>

#profile_userinfo
{
 padding: 10px;
.line {
  text-align: center;
}
.el-tree-node__expand-icon.is-leaf{
  display: none;
}

.i-btn-save{
color: #409EFF;
cursor: pointer;
}
.i-btn-cancle{
color:#97a8be;
cursor: pointer;
}
}
</style>

