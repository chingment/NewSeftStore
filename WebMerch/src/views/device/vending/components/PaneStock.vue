<template>
  <div id="device_stock">

    <div class="circle-status-bar">
      <div class="circle-item"> <span class="icon-status sellQuantity sellQuantity-bg" /> <span class="name">可售</span></div>
      <div class="circle-item"> <span class="icon-status lockQuantity lockQuantity-bg" /> <span class="name">锁定</span></div>
      <div class="circle-item"> <span class="icon-status sumQuantity sumQuantity-bg" /> <span class="name">总量</span></div>
    </div>

    <div class="filter-container" style="margin-bottom:20px">

      <el-radio-group v-model="listQuery.cabinetId" @change="onChangeCabinet">
        <el-radio-button v-for="item in options_cabinets" :key="item.value" :label="item.value">{{ item.label }}库存</el-radio-button>
      </el-radio-group>

    </div>

    <div v-loading="loading" class="rows">

      <div v-for="(row,rindex) in listData" :key="rindex" :class="'row '+(isDesktop==true?'row-flex':'row-block')">
        <template v-for="(col,cindex) in row.cols">
          <div v-show="col.isShow" :key="cindex" class="col">

            <div class="box-slot">

              <div v-if="col.skuId!=null">
                <div class="above">
                  <div class="above-img">
                    <div v-show="col.isOffSell" class="isOffSell-box">
                      <div class="isOffSell-tip">已下架</div>
                    </div>
                    <img :src="col.mainImgUrl" alt=""> </div>
                  <div class="above-des">
                    <!-- <div class="des1">
                      <div class="name">{{ col.name }}</div>
                      <div class="price" style="display:none;"> <span class="saleprice">{{ col.salePrice }}</span> </div>
                    </div> -->
                    <div class="des2">
                      <span class="sellQuantity">{{ col.sellQuantity }}</span> /
                      <span class="lockQuantity">{{ col.lockQuantity }}</span> /
                      <span class="sumQuantity">{{ col.sumQuantity }}</span>
                    </div>
                  </div>
                </div>
                <div class="below">
                  <div class="below-left">
                    <!-- <el-button type="success">置满</el-button>
              <el-button type="warning">沽清</el-button> -->
                  </div>
                  <div class="below-right">

                    <el-button type="text" @click="onDialogEditOpen(col)">编辑</el-button>
                  </div>
                </div>
              </div>
              <div v-else>
                未设置
              </div>

            </div>

          </div>
        </template>
      </div>
    </div>
    <div v-show="listData.length<=0" class="list-empty">
      <span>暂无数据</span>
    </div>

    <el-dialog title="商品库存编辑" :visible.sync="dialogEditIsVisible" :width="isDesktop==true?'600px':'90%'">

      <el-form ref="form" v-loading="dialogEditIsLoading" :model="form" :rules="rules" label-width="80px">
        <el-form-item label="货槽编号">
          <span>{{ form.slotId }}</span>
        </el-form-item>
        <el-form-item label="名称">
          <span>{{ form.name }}</span>
        </el-form-item>
        <el-form-item label="图片">

          <img
            :src="form.mainImgUrl"
            alt=""
            style="width:100px;height:100px"
          >
        </el-form-item>
        <el-form-item
          label="
            可售库存"
          prop="sellQuantity"
        >
          <el-input-number v-model="form.sellQuantity" size="small" :disabled="true" :min="0" :max="20" style="width:160px" />
        </el-form-item>
        <el-form-item label="锁定库存" prop="lockQuantity">
          <el-input-number v-model="form.lockQuantity" size="small" :disabled="true" :min="0" :max="20" style="width:160px" />
        </el-form-item>
        <el-form-item label="总库存">
          <el-input-number v-model="form.sumQuantity" size="small" :min="0" :max="20" style="width:160px" @change="sumQuantityChange" />
        </el-form-item>
        <el-form-item label="预留库存">
          <el-input-number v-model="form.holdQuantity" size="small" :min="0" :max="20" style="width:160px" />
        </el-form-item>
        <el-form-item label="最大库存">
          <el-input-number v-model="form.maxQuantity" size="small" :min="0" :max="20" style="width:160px" />
        </el-form-item>
        <el-form-item label="报警库存">
          <el-input-number v-model="form.warnQuantity" size="small" :min="0" :max="20" style="width:160px" />
        </el-form-item>
        <el-form-item v-show="false" label="销售价" prop="salePrice">
          <el-input v-model="form.salePrice" style="width:160px" class="ip-prepend">
            <template slot="prepend">￥</template>
          </el-input>
        </el-form-item>
        <el-form-item v-show="false" label="下架" prop="isOffSell">
          <el-checkbox v-model="form.isOffSell" />
        </el-form-item>
      </el-form>

      <div slot="footer" class="dialog-footer">
        <el-button size="small" @click="dialogEditIsVisible = false">
          取消
        </el-button>
        <el-button type="primary" size="small" @click="onSave">
          保存
        </el-button>
      </div>
    </el-dialog>

  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { initManageStock, manageStockGetStocks, manageStockEditStock } from '@/api/devvending'
import { isEmpty } from '@/utils/commonUtil'
import fromReg from '@/utils/formReg'
export default {
  name: 'DeviceVendingPaneStock',
  props: {
    deviceId: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      loading: false,
      listTotal: 0,
      listQuery: {
        page: 1,
        limit: 10,
        skuName: undefined,
        deviceId: undefined,
        cabinetId: undefined
      },
      listData: [],
      dialogEditIsLoading: false,
      dialogEditIsVisible: false,
      rules: {
        id: [{ required: true, min: 1, max: 200, message: '', trigger: 'change' }],
        salePrice: [{ required: true, message: '金额格式,eg:88.88', pattern: fromReg.money }]
      },
      form: {
        skuId: '',
        name: '',
        sellQuantity: 0,
        lockQuantity: 0,
        warnQuantity: 0,
        maxQuantity: 0,
        holdQuantity: 0,
        salePrice: 0,
        isOffSell: false,
        mainImgUrl: '',
        deviceId: '',
        cabinetId: '',
        slotId: '',
        version: 0
      },
      options_cabinets: [{
        value: 'dsx01n01',
        label: '主柜'
      }],
      isDesktop: this.$store.getters.isDesktop
    }
  },
  watch: {
    deviceId: function(val, oldval) {
      this.init()
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      this.loading = true
      if (!isEmpty(this.deviceId)) {
        this.listQuery.deviceId = this.deviceId
        initManageStock({ id: this.deviceId }).then(res => {
          if (res.result === 1) {
            var d = res.data
            this.options_cabinets = d.optionsCabinets
            this.listQuery.cabinetId = this.options_cabinets[0].value
            this.onChangeCabinet()
          }
          this.loading = false
        })

        this.onGetList(this.listQuery)
      }
    },
    onGetList(listQuery) {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', { path: this.$route.path, query: listQuery })
      manageStockGetStocks(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d

          for (var i = 0; i < this.listData.length; i++) {
            for (var j = 0; j < this.listData[i].cols.length; j++) {
              this.listData[i].cols[j].isShow = true
            }
          }
        } else {
          this.listData = []
        }
        this.loading = false
      })
    },
    onChangeCabinet() {
      this.onGetList(this.listQuery)
    },
    onDialogEditOpen(sku) {
      this.dialogEditIsVisible = true
      this.form.skuId = sku.skuId
      this.form.name = sku.name
      this.form.sellQuantity = sku.sellQuantity
      this.form.lockQuantity = sku.lockQuantity
      this.form.sumQuantity = sku.sumQuantity
      this.form.maxQuantity = sku.maxQuantity
      this.form.warnQuantity = sku.warnQuantity
      this.form.holdQuantity = sku.holdQuantity
      this.form.salePrice = sku.salePrice
      this.form.isOffSell = sku.isOffSell
      this.form.mainImgUrl = sku.mainImgUrl
      this.form.cabinetId = this.listQuery.cabinetId
      this.form.slotId = sku.slotId
      this.form.version = sku.version
      this.form.deviceId = this.listQuery.deviceId
    },
    onSave() {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            this.dialogEditIsLoading = true
            manageStockEditStock(this.form).then(res => {
              if (res.result === 1) {
                this.$message({
                  message: res.message,
                  type: 'success'
                })
                this.dialogEditIsLoading = false
                this.dialogEditIsVisible = false
                this.onGetList(this.listQuery)
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
    },
    sumQuantityChange(currentValue, oldValue) {
      this.form.sellQuantity = currentValue - this.form.lockQuantity
    }
  }
}
</script>

<style lang="scss" scoped>

#device_stock{
.el-form-item {
    margin-bottom: 5px;
}

.rows{
.row{

  .col{
    margin-bottom:10px;
  }
}

.row-block{
  display: block;

  .col{
    display: block;

  }

    .box-slot{
     width: 100%;
  }
}

.row-flex{
  display: flex;

  .col{
      margin-right: 10px;
  }

  .col:last-child{
    margin-right: 0px;
  }

  .box-slot{
     min-width: 85px;
  }
}

.box-slot {
    box-shadow: 0 2px 12px 0 rgba(0,0,0,.1);
    border: 1px solid #EBEEF5;
    background-color: #FFF;
    color: #303133;
    -webkit-transition: .3s;
    transition: .3s;
    padding: 4px;
    border-radius: 4px;
    overflow: hidden;
    height: 100px;
}
  .above{

    position: relative;
    .above-des{
     flex: 1;
      .des1{
        height: 65px;

        .name{
          line-height: 21px;
          max-height: 42px;
          font-size: 14px;
          color: #909399;
          overflow: hidden;
          text-overflow: ellipsis;
        }

        .saleprice{
          color: #f56c6c;
          line-height: 18px;
        }
      }
      .des2{
        height: 24px;
        line-height: 24px;
      }
     }
    .above-img{
      text-align: center;
          position: relative;
     img{
      width: 30px;
      height: 30px;
    }
     }
  }

  .below{
       display: flex;

       .below-left{
          flex: 2;
          text-align: left;
       }

       .below-right{
  flex: 1;
  text-align: right;
       }

          .el-button{
     font-size: 12px ;
     padding: 6px 12px;
   }
  }

.isOffSell-box{
height:100%;
width:100%;
background: rgba(208,208,208, 0.5);
position:absolute;
display:flex;
overflow:hidden;
justify-content: center;
align-items: center;
}
/*水平居中必备样式*/

.isOffSell-tip {
  display: inline-block;
  padding: 5px;
  color: #fff;
  background: rgba(0, 0, 0, 0.5);
  border-radius: .5rem;
  text-align: center;

}
}

          .sellQuantity{
          color: #67c23a;
        }
.sellQuantity-bg{
  background-color: #67c23a
}

        .lockQuantity{
color: #f56c6c;
        }
        .lockQuantity-bg{
          background-color: #f56c6c
        }
        .sumQuantity{
color: #e6a23c;
        }

        .sumQuantity-bg{
          background-color: #e6a23c
        }

}

 .demo4 {
    width: 1500px !important;
    margin: auto;
    .ul-item {
      width: 3000px;
    }
  }
  .ul-item {
    list-style: none;
    margin: 0;
    padding: 0;
    width: 670px;
    &.random {
      display: flex;
      width: auto;
    }
    &.decimal {
      width: 670.4px;
    }
    &.ul-item2 {
      width: 0;
    }
    .li-item {
      float: left;
      width: 124px;
      height: 124px;
      margin: 10px 0 10px 10px;
      line-height: 124px;
      background-color: lightgray;
      font-family: "Amaranth", sans-serif;
      font-size: 82px;
      text-align: center;
    }
  }
  .warp2 {
    width: 400px;
    height: 150px;
    overflow: hidden;
  }
  .test {
    height: 126px;
    width: 600px;
    overflow: hidden;
    font-size: 14px;
  }
  .test .item {
    display: flex;
    margin-top: 10px;
    div {
      flex: 1;
    }
    a {
      display: block;
    }
  }
</style>
