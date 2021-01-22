<template>
  <div id="memberright_fee">
    <el-row v-loading="loading" :gutter="20">
      <el-col v-for="item in listData" :key="item.id" :span="6" :xs="24" style="margin-bottom:20px">
        <el-card class="box-card">
          <div slot="header" class="it-header clearfix">
            <div class="left">
              <div class="circle-item"> <span class="name">{{ item.name }}</span> </div>
            </div>
            <div class="right">
              <el-button type="text" @click="handleSetting(item)">设置</el-button>
            </div>
          </div>
          <div class="it-component">
            <div class="img"> <img :src="item.mainImgUrl" alt=""> </div>
            <div class="describe">
              <ul>
                <li>原价：{{ item.feeOriginalValue }} </li>
                <li>实际价：{{ item.feeSaleValue }}</li>
              </ul>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { getFeeSts } from '@/api/memberright'
import { getUrlParam, isEmpty } from '@/utils/commonUtil'
export default {
  name: 'ManagePaneMachine',
  props: {
    levelstId: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      loading: true,
      listData: []
    }
  },
  watch: {
    levelstId: function(val, oldval) {
      this.init()
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      if (!isEmpty(this.levelstId)) {
        var id = this.levelstId
        this._getListData({ id: id })
      }
    },
    _getListData(listQuery) {
      this.loading = true
      getFeeSts(listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.feeSts
        }
        this.loading = false
      })
    }
  }
}
</script>

<style lang="scss" scoped>

#memberright_fee{

  .it-header{
    display: flex;
    justify-content: flex-start;
    align-items: center;
    position: relative;
    height:20px ;
    .left{
      flex: 1;
      justify-content: flex-start;
      align-items: center;
      display: block;
      height: 100%;
    overflow: hidden;
text-overflow:ellipsis;
white-space: nowrap;
    .name{
    padding: 0px 5px;
    }
    }
    .right{
      width: 100px;
      display: flex;
      justify-content: flex-end;
      align-items: center;
    }

  }
  .it-component{
    min-height: 100px;
    display: flex;
    .img{
      width: 120px;
      height: 120px;

      img{
        width: 100%;
        height: 100%;
      }
    }

    .describe{
      flex: 1;
      padding: 5px;
      font-size: 12px;

      ul{
        padding: 0px;
        margin: 0px;
        list-style: none;
         li{
           width: 100%;
             text-align: right;
        height: 26px;
        line-height: 26px;
      }
      }
    }
  }
}
</style>
