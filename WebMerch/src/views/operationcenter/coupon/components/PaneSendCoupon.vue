<template>
  <div id="adminuser_add">
    <el-form ref="form" v-loading="loading" :model="form" :rules="rules" label-width="80px">
      <el-form-item label="优惠券">

      </el-form-item>
      <el-form-item label="数量">
          <el-input-number v-model="form.quantity" :min="0" :max="20" style="width:160px" />
        </el-form-item>
      <el-form-item label="客户">
      
             <client-select  multiple :select-ids="selectIds" />

      </el-form-item>

      <el-form-item>
        <el-button type="primary" @click="onSubmit">保存</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { add, initAdd } from '@/api/adminuser'
import fromReg from '@/utils/formReg'
import { goBack } from '@/utils/commonUtil'
import PageHeader from '@/components/PageHeader/index.vue'
import ClientSelect from '@/views/client/components/select.vue'
export default {
  name: 'SettingAdminUserAdd',
  components: {
    PageHeader,
    ClientSelect
  },
  data() {
    return {
      loading: false,
      form: {
        quantity: 0
      },
      selectIds:['253395d25b344825a7c8bdbe191e8f45','55ca2c52278a4d9e8103dabde2956896'],
      rules: {

      }
    }
  },
  created() {
   
  },
  methods: {

  }
}
</script>

<style  lang="scss"  scoped>

</style>

