<template>
  <div id="memberright_level" v-loading="loading">

    <el-alert
      v-if="!isOpenMemberRight"
      title="您好，您的会员权益功能暂未开通，如需要，请联系客服人员!"
      type="warning"
      show-icon
      :closable="false"
      style="margin-bottom:20px"
    />

    <el-row :gutter="20">
      <el-col v-for="item in listData" :key="item.id" :span="6" :xs="24" style="margin-bottom:20px">
        <el-card class="box-card">
          <div slot="header" class="it-header clearfix">
            <div class="left">
              <div class="circle-item">
                <span class="name">{{ item.name }}</span>
              </div>
            </div>
            <div class="right">
              <el-button type="text" @click="handleManageBaseInfo(item)">设置</el-button>
            </div>
          </div>
          <div class="it-component">
            <div class="img">
              <img :src="item.mainImgUrl" alt>
            </div>
            <div class="describe">
              <ul>
                <li>
                  <el-button
                    type="text"
                    style="padding:0px;color:#67c23a;"
                    @click="handleManageBaseInfo(item)"
                  >基本信息</el-button>
                </li>
                <li>
                  <el-button
                    type="text"
                    style="padding:0px;color:#f38b3f;"
                    @click="handleManageFee(item)"
                  >会费设置</el-button>
                </li>
                <li>
                  <el-button
                    type="text"
                    style="padding:0px;color:#139baf;"
                    @click="handleManageCoupon(item)"
                  >赠券设置</el-button>
                </li>
                <li>
                  <el-button
                    type="text"
                    style="padding:0px;color:#409EFF;"
                    @click="handleManageSku(item)"
                  >优惠商品</el-button>
                </li>
              </ul>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script>
import { getLevelSts } from '@/api/memberright'

export default {
  name: 'OperationCenterMemberRightLevel',
  data() {
    return {
      loading: true,
      listData: [],
      isOpenMemberRight: false
    }
  },
  created() {
    if (this.$store.getters.listPageQuery.has(this.$route.path)) {
      this.listQuery = this.$store.getters.listPageQuery.get(this.$route.path)
    }
    this.getListData()
  },
  methods: {
    getListData() {
      this.loading = true
      this.$store.dispatch('app/saveListPageQuery', {
        path: this.$route.path,
        query: this.listQuery
      })
      getLevelSts(this.listQuery).then(res => {
        if (res.result === 1) {
          var d = res.data
          this.listData = d.levelSts
          this.isOpenMemberRight = d.isOpenMemberRight
        }
        this.loading = false
      })
    },
    handleManageBaseInfo(row) {
      this.$router.push({
        name: 'MarketMemberSet',
        path: '/operationcenter/memberright/manage',
        params: {
          id: row.id,
          tab: 'tabBaseInfo'
        }
      })
    },
    handleManageFee(row) {
      this.$router.push({
        name: 'MarketMemberSet',
        path: '/operationcenter/memberright/manage',
        params: {
          id: row.id,
          tab: 'tabFee'
        }
      })
    },
    handleManageCoupon(row) {
      this.$router.push({
        name: 'MarketMemberSet',
        path: '/operationcenter/memberright/manage',
        params: {
          id: row.id,
          tab: 'tabCoupon'
        }
      })
    },
    handleManageSku(row) {
      this.$router.push({
        name: 'MarketMemberSet',
        path: '/operationcenter/memberright/manage',
        params: {
          id: row.id,
          tab: 'tabSku'
        }
      })
    }
  }
}
</script>

<style lang="scss" scoped>
#memberright_level {

  .it-header {
    display: flex;
    justify-content: flex-start;
    align-items: center;
    position: relative;
    height: 20px;
    .left {
      flex: 1;
      justify-content: flex-start;
      align-items: center;
      display: block;
      height: 100%;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
      .name {
        padding: 0 5px;
        display: inline-block;
        flex: 1;
        overflow: hidden;
        white-space: nowrap;
        text-overflow: ellipsis;
      }
    }
    .right {
      max-width: 100px;
      display: flex;
      justify-content: flex-end;
      align-items: center;
    }
  }
  .it-component {
    min-height: 100px;
    display: flex;
    .img {
      width: 120px;
      height: 120px;

      img {
        width: 100%;
        height: 100%;
      }
    }

    .describe {
      flex: 1;
      padding: 0px;
      font-size: 12px;

      ul {
        padding: 0px;
        margin: 0px;
        list-style: none;
        li {
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
