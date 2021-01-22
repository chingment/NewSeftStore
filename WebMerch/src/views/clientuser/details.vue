<template>
  <div id="clientuser_details">
    <el-tabs v-model="activeName" type="card">
      <el-tab-pane label="基本信息" name="tabBaseInfo"> <details-pane-base-info :clientuserid="id" /></el-tab-pane>
      <el-tab-pane label="订单信息" name="tabOrder"><details-pane-order :clientuserid="id" /></el-tab-pane>
    </el-tabs>
  </div>
</template>
<script>
import { getUrlParam } from '@/utils/commonUtil'
import detailsPaneBaseInfo from './components/DetailsPaneBaseInfo'
import detailsPaneOrder from '@/views/order/list'
export default {
  components: { detailsPaneBaseInfo, detailsPaneOrder },
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

