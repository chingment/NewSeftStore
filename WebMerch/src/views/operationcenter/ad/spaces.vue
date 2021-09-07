<template>
  <div id="adspace_list">
    <el-table
      :key="listKey"
      v-loading="loading"
      :data="listData"
      fit
      highlight-current-row
      style="width: 100%;"
    >
      <el-table-column label="序号" align="left" width="80">
        <template slot-scope="scope">
          <span>{{ scope.$index+1 }} </span>
        </template>
      </el-table-column>
      <el-table-column label="版位名称" align="left" min-width="30%">
        <template slot-scope="scope">
          <span>{{ scope.row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column label="描述" align="left" min-width="70%">
        <template slot-scope="scope">
          <span>{{ scope.row.description }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" width="180" class-name="small-padding fixed-width">
        <template slot-scope="{row}">
          <el-button type="text" size="mini" @click="onRelease(row)">
            发布
          </el-button>
          <el-button type="text" size="mini" @click="onSawContents(row)">
            发布记录
          </el-button>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<script>
import { getSpaces } from '@/api/ad'

export default {
  name: 'OperationCenterAdSpaces',
  data() {
    return {
      loading: false,
      listKey: 0,
      listData: null,
      listTotal: 0,
      listQuery: {
        page: 1,
        limit: 10
      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    // if (this.$store.getters.listPageQuery.has(this.$route.path)) {
    //   this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    // }
    this.onGetSpaces()
  },
  methods: {
    onGetSpaces() {
      this.loading = true
      // this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      getSpaces(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
          this.listTotal = d.total
        }
        this.loading = false
      })
    },
    onRelease(item) {
      this.$router.push({
        path: '/operationcenter/ad/release?id=' + item.id
      })
    },
    onSawContents(item) {
      this.$router.push({
        path: '/operationcenter/ad/contents?id=' + item.id
      })
    }
  }
}
</script>
<style lang="scss" scoped>

</style>
