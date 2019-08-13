<template>
  <div class="app-container">
    <el-table
      ref="dragTable"
      v-loading="listLoading"
      :data="listData"
      style="width: 100%;margin-bottom: 20px;"
      row-key="id"
      border
      :default-expand-all="true"
      :tree-props="{children: 'children', hasChildren: 'hasChildren'}"
    >
      <el-table-column
        prop="label"
        label="机构名称"
        min-width="50"
      />
      <el-table-column
        v-if="isDesktop"
        prop="description"
        label="描述"
        min-width="50"
      />
      <el-table-column label="操作" align="center" width="280">
        <template slot-scope="{row}">
          <button type="button" class="el-button el-button--success el-button--small" @click="handleCreate(row)">
            添加子菜单
          </button>
          <button type="button" class="el-button el-button--default el-button--small" @click="handleUpdate(row)">
            编辑
          </button>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>
<script>
import { fetchList } from '@/api/adminmenu'
export default {
  data() {
    return {
      listLoading: true,
      listData: [],
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    this.getListData()
  },
  methods: {
    getListData() {
      fetchList().then(res => {
        this.listLoading = false
        this.listData = res.data
        this.expandAll()
      })
    },
    expandAll() {
      this.$nextTick(() => {
        var els = document.getElementsByClassName('el-table__row') // 获取点击的箭头元素
        for (let i = 0; i < els.length; i++) {
          els[i].style.display = 'table-row'
        }
      })
    },
    handleCreate(row) {
      this.$router.push({
        path: '/admin/menu/add?pMenuId=' + row.id
      })
    },
    handleUpdate(row) {
      this.$router.push({
        path: '/admin/menu/edit?menuId=' + row.id
      })
    }
  }
}
</script>
