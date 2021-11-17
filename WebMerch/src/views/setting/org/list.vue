<template>
  <div class="app-container">

    <el-tree
      v-loading="loading"
      :data="listData"
      :draggable="tree_setting.draggable"
      node-key="id"
      :expand-on-click-node="false"
      :allow-drop="onDrop"
      :default-expand-all="true"
      class="table-tree"
      @node-drop="sort"
    >
      <span slot-scope="{ node, data }" class="custom-tree-node">
        <span>{{ node.label }}</span>
        <span>
          <el-button
            v-if="data.extAttr.canAdd"
            type="text"
            size="mini"
            @click="() => handleCreate(data)"
          >
            添加子节点
          </el-button>
          <el-button
            v-if="data.extAttr.canEdit"
            type="text"
            size="mini"
            @click="() => handleUpdate(data)"
          >
            编辑
          </el-button>
        </span>
      </span>
    </el-tree>
  </div>
</template>
<script>
import { getList, sort } from '@/api/org'

export default {
  data() {
    return {
      tree_setting: {
        draggable: true,
        expandedIds: []
      },
      loading: false,
      listData: [],
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    this.onGetList()
  },
  methods: {
    onGetList() {
      this.loading = true
      getList().then(res => {
        if (res.result === 1) {
          this.listData = res.data
        }
        this.loading = false
      })
    },
    handleCreate(row) {
      this.$router.push({
        path: '/admin/org/add?pId=' + row.id
      })
    },
    handleUpdate(row) {
      this.$router.push({
        path: '/admin/org/edit?id=' + row.id
      })
    },
    onDrop(moveNode, inNode, type) {
      if (moveNode.level === inNode.level && moveNode.parent.id === inNode.parent.id) {
        return type === 'prev' || type === 'next'
      }
    },
    sort(draggingNode, dropNode, type, event) {
      const ids = []

      for (const item of dropNode.parent.childNodes) {
        ids.push(item.data.id)
      }

      sortOrg({ ids: ids }).then(res => {
      })
    }

  }
}
</script>
