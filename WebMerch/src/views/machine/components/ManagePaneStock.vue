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
          <el-input v-model="listQuery.productSkuName" style="width: 100%" placeholder="商品名称" va class="filter-item" @keyup.enter.native="handleFilter" />
        </el-col>
        <el-col :span="4" :xs="24" style="margin-bottom:20px">
          <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter">
            查询
          </el-button>
        </el-col>
      </el-row>
    </div>
    <el-row v-loading="loading" :gutter="20">

      <el-col v-for="(productSku,index) in listData" :key="index" :span="6" :xs="24" style="margin-bottom:20px">
        <el-card class="box-card">
          <div class="above">
            <div v-show="productSku.isOffSell" class="isOffSell-box">
              <div class="isOffSell-tip">已下架</div>
            </div>
            <div class="above-des">
              <div class="des1">
                <div class="name">      <span>({{ productSku.slotId }})</span> {{ productSku.name }}</div>
                <div class="price"> <span class="saleprice">{{ productSku.salePrice }}</span> </div>
              </div>
              <div class="des2">
                <span class="sellQuantity">{{ productSku.sellQuantity }}</span> /
                <span class="lockQuantity">{{ productSku.lockQuantity }}</span> /
                <span class="sumQuantity">{{ productSku.sumQuantity }}</span>
              </div>
            </div>
            <div class="above-img"> <img :src="productSku.mainImgUrl" alt=""> </div>
          </div>
          <div class="below">
            <div class="below-left">
              <!-- <el-button type="success">置满</el-button>
              <el-button type="warning">沽清</el-button> -->
            </div>
            <div class="below-right">

              <el-button type="primary" @click="dialogEditOpen(productSku)">编辑</el-button>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <pagination v-show="listTotal>0" :total="listTotal" :page.sync="listQuery.page" :limit.sync="listQuery.limit" @pagination="getListData" />

    <div v-show="listTotal<=0" class="list-empty">
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
          <el-input-number v-model="form.sellQuantity" :min="0" :max="20" label="描述文字" style="width:160px" />
        </el-form-item>
        <el-form-item label="锁定库存" prop="lockQuantity">
          <el-input-number v-model="form.lockQuantity" :min="0" :max="20" label="描述文字" style="width:160px" />
        </el-form-item>
        <el-form-item label="销售价" prop="salePrice">
          <el-input v-model="form.salePrice" style="width:160px" class="ip-prepend">
            <template slot="prepend">￥</template>
          </el-input>
        </el-form-item>
        <el-form-item label="下架" prop="isOffSell">
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
import { initManageStock, manageStockGetStockList, manageStockEditStock } from '@/api/machine'
import Pagination from '@/components/Pagination' // secondary package based on el-pagination
import { getUrlParam } from '@/utils/commonUtil'
import fromReg from '@/utils/formReg'
export default {
  name: 'ManagePaneStock',
  components: { Pagination },
  data() {
    return {
      loading: false,
      listTotal: 0,
      listQuery: {
        page: 1,
        limit: 10,
        productSkuName: undefined,
        machineId: undefined
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
        slotId: ''
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
      manageStockGetStockList(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.items
          this.listTotal = d.total
        }
        this.loading = false
      })
    },
    handleFilter() {
      this.listQuery.page = 1
      this.getListData()
    },
    dialogEditOpen(productSku) {
      this.dialogEditIsVisible = true

      this.form.productSkuId = productSku.id
      this.form.name = productSku.name
      this.form.sellQuantity = productSku.sellQuantity
      this.form.lockQuantity = productSku.lockQuantity
      this.form.salePrice = productSku.salePrice
      this.form.isOffSell = productSku.isOffSell
      this.form.mainImgUrl = productSku.mainImgUrl
      this.form.slotId = productSku.slotId
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
    }
  }
}
</script>

<style lang="scss" scoped>

#machine_stock{
  padding: 20px;
  padding-top: 0px;
  .above{
    height: 110px;
    display: flex;
    position: relative;
    .above-des{
     flex: 1;
      .des1{
        height: 80px;

        .name{
          line-height: 21px;
          max-height: 42px;
          font-size: 16px;
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
        height: 30px;
      }
     }
    .above-img{
     img{
      width: 80px;
      height: 80px;
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
  padding: 1rem 2rem;
  font-size: .1.2rem;
  color: #fff;
  background: rgba(0, 0, 0, 0.5);
  border-radius: .5rem;
  text-align: center;
  line-height:1.2rem;
}

}
</style>
