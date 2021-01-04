<template>
  <div id="store_baseinfo" class="app-container">
    <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="80px" :hide-required-asterisk="!isEdit">
      <el-form-item label="名称" prop="name" :show-message="isEdit">
        <span v-show="!isEdit">{{ temp.name }}</span>
      </el-form-item>
    </el-form>

  </div>
</template>
<script>

import { MessageBox } from 'element-ui'
import { edit, initManageBaseInfo } from '@/api/memberright'
import { getUrlParam } from '@/utils/commonUtil'
import Sortable from 'sortablejs'
import { all } from 'q'

export default {
  name: 'ManagePaneBaseInfo',
  props: {
    levelstid: {
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
        tag: ''
      }
    }
  },
  watch: {
    levelstid: function(val, oldval) {
      console.log('levelid 值改变')

      this.init()
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      if (this.levelstid !== '') {
        this.loading = true
        var id = this.levelstid
        initManageBaseInfo({ id: id }).then(res => {
          if (res.result === 1) {
            var d = res.data
            this.temp.name = d.name
            this.temp.tag = d.tag
          }
          this.loading = false
        })
      }
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
