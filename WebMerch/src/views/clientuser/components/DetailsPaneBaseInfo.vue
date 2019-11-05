<template>
  <div id="store_baseinfo" v-loading="loading" class="app-container">

    <el-form v-show="!isEdit" class="noeditform" label-width="80px">
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

    </el-form>

  </div>
</template>
<script>
import { initDetailsBaseInfo } from '@/api/clientuser'
import { getUrlParam } from '@/utils/commonUtil'

export default {
  name: 'ManagePaneBaseInfo',
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
        }
        this.loading = false
      })
    }
  }
}
</script>
<style lang="scss" scoped>

#store_baseinfo{
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
