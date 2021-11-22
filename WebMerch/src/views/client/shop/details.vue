<template>
  <div id="shop_details">
    <el-tabs v-model="activeName" type="card">
      <el-tab-pane label="基本信息" name="tabBaseInfo"> <pane-base-info :client-user-id="id" /></el-tab-pane>
    </el-tabs>
  </div>
</template>
<script>
import { getUrlParam } from '@/utils/commonUtil'
import PaneBaseInfo from './components/PaneBaseInfo'
export default {
  name: 'ClientShopDetails',
  components: { PaneBaseInfo },
  data() {
    return {
      activeName: 'tabBaseInfo',
      curStore: {
        id: '',
        name: ''
      },
      stores: []
    }
  },
  watch: {
    '$route'(to, from) {
      this.id = to.query.id
      this.init()
    }
  },
  created() {
    this.id = getUrlParam('id')
    this.init()
  },
  methods: {
    init() {

    }
  }
}
</script>

