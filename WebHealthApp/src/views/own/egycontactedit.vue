<template>
  <div id="pg_egycontactedit">
    <div class="step-1">
      <div class="lm-header-big">
        <div class="bg-title">紧急联系人信息</div>
      </div>

      <div class="list-field">
        <mt-field v-model="form.fullName" label="姓名" placeholder="请输入姓名" />
        <mt-field v-model="form.phoneNumber" label="手机号码" placeholder="请输入手机号码" />
      </div>

      <mt-button style="width: 100%;" type="primary" @click="onSave">保存</mt-button>

    </div>

  </div>

</template>
<script>
import { getDetails, save } from '@/api/egycontact'
import { isEmpty } from '@/utils/commonUtil'

export default {
  name: 'EgyContacts',
  components: {
  },
  data() {
    return {
      loading: false,
      appInfo: {},
      form: {
        id: '',
        fullName: '',
        phoneNumber: ''
      }
    }
  },
  created() {
    this.onInit()
  },
  methods: {
    onInit() {
      this.loading = true
      var id = this.$route.query.id
      this.form.id = id
      getDetails({ id: id }).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.form = d
        }
        this.loading = false
      })
    },
    onSave() {
      if (isEmpty(this.form.fullName)) {
        this.$toast('姓名不能为空')
        return
      }
      if (isEmpty(this.form.phoneNumber)) {
        this.$toast('手机不能为空')
        return
      }
      this.$messagebox.confirm('确定要保存?').then(action => {
        this.loading = true
        save(this.form).then(res => {
          this.$toast(res.message)
          if (res.result === 1) {
            this.$router.go(-1)
          }
          this.loading = false
        })
      })
    }
  }
}
</script>

<style lang="scss" scope>

#pg_egycontactedit {
  padding: 20px;

  .list-field {
    padding: 50px 0;
  }

}

</style>
