<template>
  <div class="app-container">

    <el-tree
      v-loading="loading"
      :data="listData"
      :draggable="tree_setting.draggable"
      node-key="id"
      :expand-on-click-node="false"
      :allow-drop="onDrop"
      class="table-tree"
      :default-expanded-keys="tree_setting.expandedIds"
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
import { getList, sort } from '@/api/prdkind'
import { treeGetNodesByDepth } from '@/utils/commonUtil'
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
    this.getListData()
  },
  methods: {
    getListData() {
      this.loading = true
      getList().then(res => {
        if (res.result === 1) {
          this.listData = res.data
          const nodes = treeGetNodesByDepth(res.data, 1)
          this.tree_setting.expandedIds = nodes.map(item => item.id)
        }
        this.loading = false
      })
    },
    handleCreate(row) {
      this.$router.push({
        path: '/prdkind/add?pId=' + row.id
      })
    },
    handleUpdate(row) {
      this.$router.push({
        path: '/prdkind/edit?id=' + row.id
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

      sort({ ids: ids }).then(res => {
      })
    }

  }
}
</script>
