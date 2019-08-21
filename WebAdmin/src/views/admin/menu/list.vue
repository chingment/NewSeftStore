<template>
  <div class="app-container">

    <el-tree
      v-loading="loading"
      :data="listData"
      :draggable="tree_setting.draggable"
      node-key="id"
      default-expand-all
      :expand-on-click-node="false"
      :allow-drop="onDrop"
      class="table-tree"
      @node-drop="sort"
    >
      <span slot-scope="{ node, data }" class="custom-tree-node">
        <span>{{ node.label }}</span>
        <span>
          <el-button
            type="text"
            size="mini"
            @click="() => handleCreate(data)"
          >
            添加子节点
          </el-button>
          <el-button
            type="text"
            size="mini"
            @click="() => handleUpdate(node, data)"
          >
            编辑
          </el-button>
        </span>
      </span>
    </el-tree>
  </div>
</template>
<script>
import { fetchList, sortMenu } from '@/api/adminmenu'
export default {
  data() {
    return {
      tree_setting: {
        draggable: true
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
      fetchList().then(res => {
        if (res.result === 1) {
          this.listData = res.data
        }
        this.loading = false
      })
    },
    handleCreate(row) {
      this.$router.push({
        path: '/admin/menu/add?pId=' + row.id
      })
    },
    handleUpdate(row) {
      this.$router.push({
        path: '/admin/menu/edit?id=' + row.id
      })
    },
    onDrop(moveNode, inNode, type) {
      if (moveNode.level === inNode.level && moveNode.parent.id === inNode.parent.id) {
        return type === 'prev' || type === 'next'
      }
    },
    sort(draggingNode, dropNode, type, event) {
      console.log(draggingNode)
      console.log(dropNode)
      if (draggingNode.data.aboveId === dropNode.data.aboveId) {
        const obj = {
          aboveId: '',
          arr: []
        }
        obj.aboveId = dropNode.data.aboveId
        for (const item of dropNode.parent.childNodes) {
          obj.arr.push(item.data.id)
        }

        sortMenu({ ids: obj.arr }).then(res => {
        })

        // console.log(JSON.stringify(obj))
        // this.updateOrderMe(obj)
      }
    }
  }
}
</script>
