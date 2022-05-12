<template>
  <div id="pg_device_manage">
    <div class="lm-header-big">
      <div class="bg-title">我的紧急联系人</div>
      <div class="sm-title">以下是您的紧急联系人信息</div>
    </div>

    <div
      style="ackground-color: rgb(248, 248, 248);
    padding: 20px;
    /* margin: 1.2em 0px; */
    overflow: auto;
    border-radius: 8px;
    line-height: 22px;
    font-size: 12px;"
    >
      当您的健康报告出现异常时，心晓设备会同步发送短信通知您和紧急联系人，提醒您的健康异常情况。同时建议您邀请紧急联系人关注您的心晓精准健康账号，关注后紧急联系人可同步收到您的每日健康报告
    </div>

    <div class="lm-body">
      <div v-if="list.items.length>0" class="data-list">
        <mt-cell v-for="(item, index) in list.items" :key="index" :title="item.fullName" is-link @click.native="onEditContact(item)" />
      </div>
      <div v-else class="data-empty">
        <img class="icon" src="@/assets/images/data_empty.png" alt="">
        <span class="tips">暂无数据</span>
      </div>
    </div>

    <mt-button class="btn-unbind" type="primary" @click="onNewContact">新建联系人</mt-button>

  </div>

</template>
<script>
import { egyContacts } from '@/api/own'

export default {
  name: 'OwnEgyContacts',
  components: {
  },
  data() {
    return {
      loading: false,
      list: {
        total: 0,
        items: []
      }
    }
  },
  created() {
    this.onInit()
  },
  methods: {
    onInit() {
      this.loading = true
      egyContacts({}).then(res => {
        if (res.result === 1) {
          this.list = res.data
        }
        this.loading = false
      })
    },
    onNewContact() {
      this.$router.push('/own/egycontact/edit')
    },
    onEditContact(item) {
      this.$router.push('/own/egycontact/edit?id=' + item.id)
    }
  }
}
</script>

<style lang="scss" scope>

#pg_device_manage {
  padding: 20px;

  .data-list {
    padding: 20px 0;
  }

  .btn-unbind {
    width: 100%;
  }
}

</style>
