<template>
  <div id="machine_stock">

    <div class="circle-status-bar">
      <span class="circle-status sellQuantity sellQuantity-bg" /> <span class="name">可售</span>
      <span class="circle-status lockQuantity lockQuantity-bg" /> <span class="name">锁定</span>
      <span class="circle-status sumQuantity sumQuantity-bg" /> <span class="name">总量</span>
    </div>

    <div class="filter-container">

      <el-row :gutter="12">
        <el-col :span="4" :xs="24" style="margin-bottom:20px">
          <el-input v-model="listQuery.productSkuName" style="width: 100%" placeholder="商品名称" class="filter-item" />
        </el-col>
        <el-col :span="4" :xs="24" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter">
            查询
          </el-button>
        </el-col>
      </el-row>
    </div>

    <div class="rows">

      <div v-for="(row,rindex) in listData" :key="rindex" :class="'row '+(isDesktop==true?'row-flex':'row-block')">

        <template v-for="(col,cindex) in row.cols">
          <div v-show="col.isShow" :key="cindex" class="col">

            <div class="box-slot">

              <div v-if="col.productSkuId!=null" style="max-width:180px;margin:auto">
                <div class="above">
                  <div class="above-img">
                    <div v-show="col.isOffSell" class="isOffSell-box">
                      <div class="isOffSell-tip">已下架</div>
                    </div>
                    <img :src="col.mainImgUrl" alt=""> </div>
                  <div class="above-des">
                    <div class="des1">
                      <div class="name">{{ col.name }}</div>
                      <div class="price" style="display:none;"> <span class="saleprice">{{ col.salePrice }}</span> </div>
                    </div>
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

                    <el-button type="primary" @click="dialogEditOpen(col)">编辑</el-button>
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

    <el-dialog title="商品库存编辑" :visible.sync="dialogEditIsVisible" :width="isDesktop==true?'500px':'90%'">

      <el-form ref="form" :model="form" :rules="rules" label-width="80px">
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
          <el-input-number v-model="form.sellQuantity" :min="0" :max="20" style="width:160px" @change="sellQuantityChange" />
        </el-form-item>
        <el-form-item label="锁定库存" prop="lockQuantity">
          <el-input-number v-model="form.lockQuantity" :min="0" :max="20" style="width:160px" @change="lockQuantityChange" />
        </el-form-item>
        <el-form-item label="总库存">
          <el-input-number v-model="form.sumQuantity" :disabled="true" :min="0" :max="20" style="width:160px" />
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
        <el-button @click="dialogEditIsVisible = false">
          取消
        </el-button>
        <el-button type="primary" @click="handleEdit">
          确定
        </el-button>
      </div>
    </el-dialog>

  </div>
</template>

<script>
import { MessageBox } from 'element-ui'
import { initManageStock, manageStockGetStocks, manageStockEditStock } from '@/api/machine'
import { getUrlParam } from '@/utils/commonUtil'
import fromReg from '@/utils/formReg'
export default {
  name: 'ManagePaneStock',
  data() {
    return {
      loading: false,
      listTotal: 0,
      listQuery: {
        page: 1,
        limit: 10,
        productSkuName: undefined,
        machineId: undefined,
        cabinetId: undefined
      },
      listData: [],
      dialogEditIsVisible: false,
      rules: {
        id: [{ required: true, min: 1, max: 200, message: '', trigger: 'change' }],
        salePrice: [{ required: true, message: '金额格式,eg:88.88', pattern: fromReg.money }]
      },
      form: {
        productSkuId: '',
        name: '',
        sellQuantity: 0,
        lockQuantity: 0,
        salePrice: 0,
        isOffSell: false,
        mainImgUrl: '',
        machineId: '',
        slotId: '',
        version: 0
      },
      isDesktop: this.$store.getters.isDesktop
    }
  },
  watch: {
    '$route'(to, from) {
      this.init()
    }
  },
  created() {
    this.init()
  },
  methods: {
    init() {
      var id = getUrlParam('id')
      this.loading = true
      this.listQuery.machineId = id
      this.listQuery.cabinetId = '0'
      initManageStock({ id: id }).then(res => {
        if (res.result === 1) {
          var d = res.data
        }
        this.loading = false
      })

      this.getListData(this.listQuery)
    },
    getListData(listQuery) {
      console.log('getListData')
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
    handleFilter() {
      var search = this.listQuery.productSkuName
      var l_listData = this.listData
      for (var i = 0; i < l_listData.length; i++) {
        for (var j = 0; j < l_listData[i].cols.length; j++) {
          if (search !== undefined && search != null && search.length > 0) {
            if (l_listData[i].cols[j].name == null) {
              l_listData[i].cols[j].isShow = false
            } else {
              if (l_listData[i].cols[j].name.search(search) !== -1) {
                console.log(l_listData[i].cols[j].name)
                l_listData[i].cols[j].isShow = true
              } else {
                l_listData[i].cols[j].isShow = false
              }
            }
          } else {
            l_listData[i].cols[j].isShow = true
          }
        }
      }
      this.listData = []
      this.listData = l_listData
    },
    dialogEditOpen(productSku) {
      this.dialogEditIsVisible = true

      this.form.productSkuId = productSku.productSkuId
      this.form.name = productSku.name
      this.form.sellQuantity = productSku.sellQuantity
      this.form.lockQuantity = productSku.lockQuantity
      this.form.sumQuantity = productSku.sumQuantity
      this.form.salePrice = productSku.salePrice
      this.form.isOffSell = productSku.isOffSell
      this.form.mainImgUrl = productSku.mainImgUrl
      this.form.slotId = productSku.slotId
      this.form.version = productSku.version
      this.form.machineId = getUrlParam('id')
    },
    handleEdit() {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          MessageBox.confirm('确定要保存', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            manageStockEditStock(this.form).then(res => {
              this.$message(res.message)
              if (res.result === 1) {
                this.dialogEditIsVisible = false
                this.getListData(this.listQuery)
              }
            })
          })
        }
      })
    },
    sellQuantityChange(currentValue, oldValue) {
      this.form.sumQuantity = currentValue - this.form.lockQuantity
    },
    lockQuantityChange(currentValue, oldValue) {
      this.form.sumQuantity = currentValue + this.form.sellQuantity
    }
  }
}
</script>

<style lang="scss" scoped>

#machine_stock{
  padding: 20px;
  padding-top: 0px;

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
    flex: 1;
  }

  .col:last-child{
    margin-right: 0px;
  }

  .box-slot{
     min-width: 160px;
  }
}

.box-slot {
    box-shadow: 0 2px 12px 0 rgba(0,0,0,.1);
    border: 1px solid #EBEEF5;
    background-color: #FFF;
    color: #303133;
    -webkit-transition: .3s;
    transition: .3s;
    padding: 10px;
    border-radius: 4px;
    overflow: hidden;
    height: 188px;
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
      width: 50px;
      height: 50px;
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
</style>
