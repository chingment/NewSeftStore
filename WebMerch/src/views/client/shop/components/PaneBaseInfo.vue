<template>
  <div id="shop_baseinfo">
    <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="80px">
      <el-form-item label="编码">
        {{ temp.id }}
      </el-form-item>
      <el-form-item label="用户名">
        {{ temp.userName }}
      </el-form-item>
      <el-form-item label="姓名">
        {{ temp.fullName }}
      </el-form-item>
      <el-form-item label="手机号码">
        {{ temp.phoneNumber }}
      </el-form-item>
      <el-form-item label="昵称">
        {{ temp.nickName }}
      </el-form-item>
      <el-form-item label="标识">
        <span v-show="!isEdit&&form.isHasProm">推销者</span>
        <span v-show="!isEdit&&form.isStaff">公司职员</span>
        <el-checkbox v-show="isEdit" v-model="form.isHasProm ">推销者</el-checkbox>
        <el-checkbox v-show="isEdit" v-model="form.isStaff">公司职员</el-checkbox>
      </el-form-item>
      <el-form-item>
        <el-button v-show="!isEdit" type="primary" @click="onOpenEdit">编辑</el-button>
        <el-button v-show="isEdit" type="info" @click="onCancleEdit">取消</el-button>
        <el-button v-show="isEdit" type="primary" @click="onSubmit">保存</el-button>
      </el-form-item>
    </el-form>

  </div>
</template>
<script>
import { MessageBox } from 'element-ui'
import { initDetailsBaseInfo, edit } from '@/api/clientuser'
import { getUrlParam } from '@/utils/commonUtil'

export default {
  name: 'ClientShopPaneBaseInfo',
  data() {
    return {
      isEdit: false,
      loading: false,
      temp: {
        id: '',
        userName: '',
        fullName: '',
        phoneNumber: '',
        nickName: ''
      },
      form: {
        id: '',
        isHasProm: false,
        isStaff: false
      },
      rules: {

      }
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
      initDetailsBaseInfo({ id: id }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.temp.id = d.id
          this.temp.userName = d.userName
          this.temp.fullName = d.fullName
          this.temp.phoneNumber = d.phoneNumber
          this.temp.nickName = d.nickName
          this.form.id = d.id
          this.form.isHasProm = d.isHasProm
          this.form.isStaff = d.isStaff
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
            this.loading = true
            edit(this.form).then(res => {
              if (res.result === 1) {
                this.$message({
                  message: res.message,
                  type: 'success'
                })
                this.isEdit = false
                this.init()
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
        }
      })
    },
    onOpenEdit() {
      this.isEdit = true
    },
    onCancleEdit() {
      this.isEdit = false
    }
  }
}
</script>
<style lang="scss" scoped>

#shop_baseinfo{
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
