<template>
  <div id="store_baseinfo" v-loading="loading" class="app-container">

    <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="80px" :hide-required-asterisk="!isEdit">

      <el-form-item label="名称" prop="name" :show-message="isEdit">
        <span>{{ temp.name }}</span>
      </el-form-item>

      <el-form-item label="联系地址" prop="contactAddress" :show-message="isEdit">
        <span v-show="!isEdit">{{ temp.contactAddress }}</span>  <el-input v-show="isEdit" v-model="form.contactAddress" clearable />
      </el-form-item>
      <el-form-item>
        <el-button v-show="!isEdit" type="primary" @click="openEdit">编辑</el-button>
        <el-button v-show="isEdit" type="info" @click="cancleEdit">取消</el-button>
        <el-button v-show="isEdit" type="primary" @click="onSubmit">保存</el-button>
      </el-form-item>
    </el-form>

  </div>
</template>
<script>

import { MessageBox } from 'element-ui'
import { edit, initManageBaseInfo } from '@/api/store'
import { getUrlParam, isEmpty } from '@/utils/commonUtil'
import Sortable from 'sortablejs'
import { all } from 'q'

export default {
  name: 'ManagePaneBaseInfo',
  props: {
    storeid: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      isEdit: false,
      loading: false,
      temp: {
        id: '',
        name: '',
        contactAddress: ''
      },
      form: {
        id: '',
        name: '',
        contactAddress: ''
      },
      rules: {
        contactAddress: [{ required: true, min: 1, max: 200, message: '必填,且不能超过200个字符', trigger: 'change' }]
      }
    }
  },
  watch: {
    storeid: function(val, oldval) {
      console.log('storeid 值改变:' + val)

      this.init()
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      if (!isEmpty(this.storeid)) {
        this.loading = true
        initManageBaseInfo({ id: this.storeid }).then(res => {
          if (res.result === 1) {
            var d = res.data
            this.form.id = d.id
            this.form.name = d.name
            this.form.contactAddress = d.contactAddress

            this.temp.name = this.form.name
            this.temp.contactAddress = this.form.contactAddress
          }
          this.loading = false
        })
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
            edit(this.form).then(res => {
              this.$message(res.message)
              if (res.result === 1) {
                this.isEdit = false
                this.init()
              }
            })
          }).catch(() => {
          })
        }
      })
    },
    openEdit() {
      this.isEdit = true
    },
    cancleEdit() {
      this.isEdit = false
    }
  }
}
</script>
<style lang="scss" scoped>

#dsss .el-form-item__label:before{
  content: '';
  margin-left:1000px;
}

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

.bm-view {
  width: 100%;
  height: 200px;
  margin-top: 20px;
}

.autoAddressClass{
  li {
    display: flex;
    i.el-icon-search {margin-top:11px;}
    .mgr10 {margin-right: 10px;}
    .title {
      text-overflow: ellipsis;
      overflow: hidden;
    }

.address-ct{
  flex: 1;
}

    .address {
      line-height: 1;
      font-size: 12px;
      color: #b4b4b4;
      margin-bottom: 5px;
    }

  }
}

}

</style>
