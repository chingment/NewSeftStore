<template>
  <div class="prodcut-list" />
</template>
<style lang="scss" scoped>
.prodcut-list .el-menu {
  border-right: solid 0px #e6e6e6;
}

.prodcut-list .el-col-5 {
  width: 20%;
}

.el-header {
  background-color: #b3c0d1;
  color: #333;
  line-height: 60px;
}

.el-aside {
  color: #333;
}

.box-card-product {
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
        padding: 0px 5px;
      }
    }
    .right {
      width: 100px;
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
