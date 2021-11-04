<template>
  <div class="app-container my-container1">
    <el-container>
      <el-aside class="my-aside">
        <el-menu router :default-active="navActive">
          <el-menu-item-group>
            <template slot="title">支付中心</template>
            <el-menu-item index="/paytrans/pay/query">支付查询</el-menu-item>
          </el-menu-item-group>
          <el-menu-item-group>
            <template slot="title">退款中心</template>
            <el-menu-item index="/paytrans/refund/query">退款查询</el-menu-item>
            <el-menu-item index="/paytrans/refund/apply">申请退款</el-menu-item>
            <el-badge :value="count.refundHandle" class="item" :hidden="count.refundHandle==0">
              <el-menu-item index="/paytrans/refund/handle">退款处理</el-menu-item>
            </el-badge>
          </el-menu-item-group>
        </el-menu>
      </el-aside>
      <el-container>
        <el-main>
          <router-view @onGetSummary="onGetSummary" />
        </el-main>
      </el-container>
    </el-container>
  </div>
</template>

<script>
import { getSummary } from '@/api/paytrans'
export default {
  name: 'PayTransIndex',
  data() {
    return {
      navActive: '/order/list',
      listQuery: null,
      loading: false,
      isDesktop: this.$store.getters.isDesktop,
      count: {
        refundHandle: 0
      }
    }
  },
  created() {
    this.navActive = this.$route.path
    this.onGetSummary()
  },
  methods: {
    onGetSummary() {
      this.loading = true
      getSummary(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.count = d.count
        }
        this.loading = false
      })
    }
  }
}
</script>
