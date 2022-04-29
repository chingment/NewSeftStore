<template>
  <el-card class="box-card box-card-senviv-user" :body-style="{ padding: '0px' }">
    <div class="it-header clearfix">
      <div class="left">
        <div class="l1">
          <el-avatar :src="rd.avatar" size="medium" />
        </div>
        <div class="l2">
          <span class="name">{{ rd.signName }}</span>
        </div>
      </div>
      <div class="right">
        <el-button type="text" @click="_onSawDetail(rd)">查看</el-button>
      </div>
    </div>
    <div class="it-component">
      <div class="t1"><span class="sex">{{ rd.sex.text }}</span> <span class="age">{{ rd.age }}岁</span> <span class="height">身高：{{ rd.height }}</span><span class="weight">体重：{{ rd.weight }}</span></div>
      <div class="t2"> <div class="zs">心脏总能量：<dv-item :value="rd.hrvXzznl" sign /> </div><div class="zs">基准心率：<dv-item :value="rd.xlDcjzxl" sign /></div>   </div>
      <div>
        <el-tag
          v-for="tag in rd.signTags"
          :key="tag.name"
          style="margin-right: 10px;margin-bottom: 10px"
          type="warning"
        >
          {{ tag.name }}
        </el-tag>
      </div>
      <div>
        <el-tag
          v-for="tag in rd.smTags"
          :key="tag"
          style="margin-right: 10px;margin-bottom: 10px"
        >
          {{ tag }}
        </el-tag>
      </div>
    </div>
  </el-card>

</template>

<script>
import DvItem from './DvItem.vue'
export default {
  name: 'CardUser',
  components: { DvItem },
  props: {
    rd: {
      type: Object,
      default: null
    },
    onSawDetail: { type: Function, default: function(item) { } }
  },
  data() {
    return {
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {

  },
  methods: {
    _onSawDetail(item) {
      console.log(item)
      this.onSawDetail(item)
    }
  }
}
</script>

