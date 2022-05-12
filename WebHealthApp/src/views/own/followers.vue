<template>
  <div id="pg_own_followers">
    <div class="step-1">
      <div class="lm-header-big">
        <div class="bg-title">关注我的人</div>
        <div class="sm-title">以下是关注你的人</div>
      </div>
      <div class="lm-body">
        <div v-if="list.items.length>0" class="data-list">
          <mt-cell v-for="(item, index) in list.items" :key="index" :title="item.nickName">
            <img slot="icon" :src="item.avatar" width="24" height="24">
            <span class="btn-remove" @click="onRemove(item)">移除</span>
          </mt-cell>
        </div>

        <div v-else class="data-empty">
          <img class="icon" src="@/assets/images/data_empty.png" alt="">
          <span class="tips">暂无数据</span>
        </div>

      </div>
    </div>

  </div>

</template>
<script>

import { followers } from '@/api/own'

export default {
  name: 'OwnFollowers',
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
      followers({}).then(res => {
        if (res.result === 1) {
          this.list = res.data
        }
        this.loading = false
      })
    },
    onRemove() {
      this.$messagebox.confirm('确定要移除?').then(action => {

      })
    }
  }
}
</script>

<style lang="scss" scope>

#pg_own_followers {
  padding: 20px;

  .btn-remove {
    font-size: 12px;

    padding: 6px 12px;

    color: #fff;
    border-radius: 16px;
    background:rgba(239, 109, 79, 0.92)
  }
}

</style>
