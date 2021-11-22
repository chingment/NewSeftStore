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
              <el-button type="text" @click="dialogBySettingOpen(item)">设置</el-button>
            </div>
          </div>
          <div class="it-component">
            <div class="img"> <img :src="item.mainImgUrl" alt=""> </div>
            <div class="describe">
              <ul>
                <li>原价：{{ item.feeOriginalValue }} </li>
                <li>实际价：{{ item.feeSaleValue }}</li>
                <li>状态：{{ item.status.text }}</li>
              </ul>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>
    <el-dialog title="设置" :visible.sync="dialogBySettingIsVisible" :width="isDesktop==true?'600px':'90%'">
      <el-form ref="formBySetting" :model="formBySetting" label-width="80px">
        <el-form-item label="名称">
          <span>{{ formBySetting.name }}</span>
        </el-form-item>
        <el-form-item label="原价" prop="originalValue">
          <el-input v-model="formBySetting.originalValue" clearable style="width:160px">
            <template slot="prepend">￥</template>
          </el-input>
        </el-form-item>
        <el-form-item label="实际价" prop="saleValue">
          <el-input v-model="formBySetting.saleValue" clearable style="width:160px">
            <template slot="prepend">￥</template>
          </el-input>
        </el-form-item>
        <el-form-item label="停用">
          <el-switch v-model="formBySetting.isStop" />
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button size="small" @click="dialogBySettingIsVisible = false">
          取消
        </el-button>
        <el-button size="small" type="primary" @click="handleSetting">
          确定
        </el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { getFeeSts, setFeeSt } from '@/api/memberright'
import { isEmpty } from '@/utils/commonUtil'
export default {
  name: 'OperationCenterMemberRightPaneFee',
  props: {
    levelstId: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      loading: true,
      listQuery: {
        id: ''
      },
      listData: [],
      dialogBySettingIsVisible: false,
      formBySetting: {
        feeStId: '',
        name: '',
        originalValue: 0,
        saleValue: 0,
        isStop: false
      },
      isDesktop: this.$store.getters.isDesktop
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
      this.listQuery = listQuery
      getFeeSts(listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.feeSts
        }
        this.loading = false
      })
    },
    dialogBySettingOpen(item) {
      this.dialogBySettingIsVisible = true
      this.formBySetting.feeStId = item.id
      this.formBySetting.name = item.name
      this.formBySetting.originalValue = item.feeOriginalValue
      this.formBySetting.saleValue = item.feeSaleValue
      this.formBySetting.isStop = item.isStop
    },
    handleSetting() {
      this.$refs['formBySetting'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            setFeeSt(this.formBySetting).then(res => {
              if (res.result === 1) {
                this.$message({
                  message: res.message,
                  type: 'success'
                })
                this.dialogBySettingIsVisible = false
                this._getListData(this.listQuery)
              } else {
                this.$message({
                  message: res.message,
                  type: 'error'
                })
              }
            })
          }).catch(() => {
          })
        }
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
