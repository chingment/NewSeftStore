<template>
  <div id="client_select">
  
    <div class="its-clients">

        <template v-for="(item,index) in avatars">
    <div class="it" :key="index" style="position: relative">
      <div class="it-avatar">
            <img class="img bd-cycle" :src="item.avatar" style="width: 38px; height: 38px;">
            <i class="el-icon-error" @click="handleDelete(item)" style="position:absolute;right:0;top:0;"></i>
        </div>
        <div class="it-nickname">
            <span class="txt">{{item.nickName}}</span>
     </div>
    </div>
        </template>

   <div class="it" >

        <div class="it-nickname">
              <div @click="handleOpenDialogByClients" > 选择  </div>
     </div>
    </div>


    </div>

 
    
    <el-dialog v-if="dialogIsShowByClients" title="选择" :visible.sync="dialogIsShowByClients" width="800px" append-to-body>
     
    <el-table
      :key="listKey"
      v-loading="loading"
      :data="listData"
      fit
      highlight-current-row
      style="width: 100%;"
      @selection-change="handleSelectionChange"
    >
    <el-table-column
      v-if="multiple"
      type="selection"
      width="55">
    </el-table-column>
     <el-table-column  v-if="!multiple" label="" prop="fullName" align="left"  width="55">
        <template slot-scope="scope">
           <el-radio v-model="selectIdBySingle" :label="scope.row.id"><span/></el-radio>
        </template>
      </el-table-column>
     <el-table-column label="头像" prop="fullName" align="left" min-width="20%">
        <template slot-scope="scope">
         <img class="img bd-cycle" :src="scope.row.avatar" style="width:38px;height:38px;">
        </template>
      </el-table-column>
      <el-table-column label="昵称" prop="nickName" align="left" min-width="20%">
        <template slot-scope="scope">
          <span>{{ scope.row.nickName }}</span>
        </template>
      </el-table-column>
      <el-table-column label="手机号码" prop="phoneNumber" align="left" min-width="10%">
        <template slot-scope="scope">
          <span>{{ scope.row.phoneNumber }}</span>
        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />

      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogIsShowByClients = false">
          取消
        </el-button>
        <el-button type="primary" @click="handleSelect">
          确定
        </el-button>
      </div>

    </el-dialog>

  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { getList,getAvatars } from '@/api/clientuser'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination
export default {
props: {
    multiple: {
      type: Boolean,
      default: false
    },
    selectIds: {
      type: Array,
      default: []
    }
  },
  name: 'ClientSelect',
  components: { Pagination },
  data() {
    return {
      loading: false,
      listKey: 0,
      listData: null,
      listTotal: 0,
      selectIdBySingle:'',
      selectIdsByMultiple:[],
      listQuery: {
        page: 1,
        limit: 10,
        name: undefined
      },
      avatars:null,
      dialogIsShowByClients: false,
      isDesktop: this.$store.getters.isDesktop
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }
    this.getListData()
    this._getAvatars()
  },
  methods: {
    getListData() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: this.listQuery })
      getList(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
          this.listTotal = d.total
        }
        this.loading = false
      })
    },
    handleOpenDialogByClients() {
      this.dialogIsShowByClients = true
    },
    _getAvatars(){
        getAvatars({clientUserIds: this.selectIds}).then(res => {
        if (res.result === 1) {
          this.avatars = res.data
           this.dialogIsShowByClients=false
        }
        this.loading = false
      })
    },
    handleSelectionChange(val) {
      this.selectIdsByMultiple = val
    },
    handleSelect(){
      
      if(this.multiple){
        if(this.selectIdsByMultiple==null||this.selectIdsByMultiple.length==0)
        {
                 this.$message('至少选择一个')
       return
        }

       for(var i=0;i< this.selectIdsByMultiple.length;i++){
      console.log(this.selectIdsByMultiple[i].id)
          this.selectIds.push(this.selectIdsByMultiple[i].id)
          }

      }
      else{
             if(this.selectIdBySingle==''){
       this.$message('请选择')
       return
      }

        this.selectIds.push(this.selectIdBySingle)
      }
     
      this.$emit('GetSelectIds',this.selectIds)

       this._getAvatars()

    },
    handleDelete(item){
   MessageBox.confirm('确定移除'+item.nickName, '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
           
           for(var i=0;i<this.avatars.length;i++){
             if(this.avatars[i].id==item.id){
               this.avatars.splice(i,1)
             }
           }

   for(var i=0;i<this.selectIds.length;i++){
             if(this.selectIds[i]==item.id){
               this.selectIds.splice(i,1)
             }
           }

this.$emit('GetSelectIds',this.selectIds)

          }).catch(() => {
          })
    }
  }
}
</script>
<style lang="scss" scoped>

#client_select{
.its-clients{
   
   .it{
       float: left;
       width: 50px;
       display: flex;
       display: block;

       .it-avatar{
    display: flex;
    justify-content: center
       }

        .it-nickname{
    white-space: nowrap;
    text-overflow: ellipsis;
    overflow: hidden;
       }
   }
}

}

</style>