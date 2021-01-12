<template>
  <div id="store_baseinfo" v-loading="loading" class="app-container">

    <el-form ref="form" v-loading="loading" label-width="80px">
      <el-form-item label="名称" prop="name">
        <span>{{ store.name }}</span>
      </el-form-item>
      <el-form-item label="联系地址" prop="contactAddress">
        <span>{{ store.contactAddress }}</span>
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
      store: {
        id: '',
        name: '',
        contactAddress: ''
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
            this.store.id = d.id
            this.store.name = d.name
            this.store.contactAddress = d.contactAddress
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
