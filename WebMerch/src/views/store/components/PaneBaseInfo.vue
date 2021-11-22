<template>
  <div id="store_baseinfo">
    <el-form ref="form" v-loading="loading" label-width="80px">
      <el-form-item label="名称" prop="name">
        <span>{{ store.name }}</span>
      </el-form-item>
      <el-form-item label="联系人" prop="contactAddress">
        <span>{{ store.contactName }}</span>
      </el-form-item>
      <el-form-item label="联系电话" prop="contactAddress">
        <span>{{ store.contactPhone }}</span>
      </el-form-item>
      <el-form-item label="联系地址" prop="contactAddress">
        <span>{{ store.contactAddress }}</span>
      </el-form-item>
      <el-form-item label="简介" prop="contactAddress">
        <span>{{ store.briefDes }}</span>
      </el-form-item>
    </el-form>
  </div>
</template>
<script>

import { initManageBaseInfo } from '@/api/store'
import { isEmpty } from '@/utils/commonUtil'
export default {
  name: 'StorePaneBaseInfo',
  props: {
    storeId: {
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
        contactName: '',
        contactPhone: '',
        contactAddress: ''
      }
    }
  },
  watch: {
    storeId: function(val, oldval) {
      this.init()
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      if (!isEmpty(this.storeId)) {
        this.loading = true
        initManageBaseInfo({ id: this.storeId }).then(res => {
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

</style>
