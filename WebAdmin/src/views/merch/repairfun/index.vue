<template>
  <div class="app-container">
    <div class="filter-container">
      <el-button class="filter-item" style="margin-left: 10px;" type="primary" @click="_reLoadSpuCache">
        重新加载商户商品数据缓存
      </el-button>
    </div>
  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { reLoadSpuCache } from '@/api/merchrepairfun'

export default {
  name: 'Repairfun',
  data() {
    return {
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {

  },
  methods: {
    _reLoadSpuCache() {
      MessageBox.confirm('确定要重新加载商户商品数据缓存？', '提示（慎重操作）', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        reLoadSpuCache(null).then(res => {
          this.$message(res.message)
        })
      })
    }
  }
}
</script>
