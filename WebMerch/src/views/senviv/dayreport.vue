<template>
  <div id="senviv_user_list">
    <div class="filter-container">
      <el-form ref="form" label-width="120px">
        <el-form-item label="目前困扰">

          <el-checkbox-group v-model="checkboxGroup1">
            <el-checkbox-button v-for="item in perplexs" :key="item.value" :label="item.value">{{ item.label }}</el-checkbox-button>
          </el-checkbox-group>

        </el-form-item>
        <el-form-item label="慢性病">
          <el-radio-group v-model="listQuery.chronic" @change="handleFilter">
            <el-radio-button v-for="item in chronics" :key="item.value" :label="item.value">{{ item.label }}</el-radio-button>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="呼吸综合症">
          <el-radio-group v-model="listQuery.sas" @change="handleFilter">
            <el-radio-button v-for="item in sass" :key="item.value" :label="item.value">{{ item.label }}</el-radio-button>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="搜索">
          <el-input v-model="listQuery.name" clearable style="max-width: 300px;" placeholder="昵称/姓名" @keyup.enter.native="handleFilter" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="el-icon-search" @click="handleFilter">查询</el-button>
        </el-form-item>
      </el-form>
    </div>

    <pane-day-report />

  </div>
</template>

<script>

import PaneDayReport from './components/PaneDayReport.vue'
export default {
  name: 'SenvivDayReport',
  components: { PaneDayReport },
  data() {
    return {
      loading: false,
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        page: 1,
        limit: 20,
        name: undefined,
        sas: '0',
        chronic: '0',
        perplexs: null
      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {

  },
  methods: {

  }
}
</script>

