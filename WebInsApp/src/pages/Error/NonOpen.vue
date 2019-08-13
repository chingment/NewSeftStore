<template>
  <div id="app_wrapper" >
    <div class="messagebox">
    <div class="icon"> <img src="@/assets/images/error_an.jpg"  alt=""> </div>
    <P class="title">温馨提示</P>
    <P class="content">该功能未开通，敬请期待！</P>
    </div>
  </div>
</template>
<style  lang="less" >
body {
  background-color: #fff;
}

.messagebox {
  margin-top: 5rem;
  .icon {
    width: 50%;
    margin: auto;
  }

  .title {
    line-height: 2.4rem;
    height: 2.4rem;
    font-size: 1.1rem;
  }

  .content {
    line-height: 2.4rem;
    height: 2.4rem;
    font-size: 0.9rem;
  }
}
</style>
