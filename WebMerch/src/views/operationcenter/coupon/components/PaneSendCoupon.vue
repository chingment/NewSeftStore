<template>
  <div id="pane_send_coupon">
    <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="80px">
      <el-form-item label="优惠券" />
      <el-form-item label="数量">
        <el-input-number v-model="form.quantity" :min="0" :max="20" style="width:160px" />
      </el-form-item>
      <el-form-item label="客户">

        <pane-client-select multiple :select-ids="selectIds" @GetSelectIds="getClientUserIds" />

      </el-form-item>
    </el-form>

  </div>
</template>

<script>
import PaneClientSelect from '@/views/client/components/select.vue'
export default {
  name: 'OperationCenterCouponPaneSendCoupon',
  components: {
    PaneClientSelect
  },
  data() {
    return {
      loading: false,
      form: {
        quantity: 0
      },
      selectIds: [],
      rules: {

      }
    }
  },
  created() {

  },
  methods: {
    onSubmit() {
      console.log(JSON.stringify(this.selectIds))
    },
    getClientUserIds(ids) {
      console.log(JSON.stringify(ids))
      this.selectIds = ids
    }
  }
}
</script>

<style  lang="scss"  scoped>

</style>

