<template>
  <div class="mb5" >
   <div class="frmgrid" >
    <div class="title">
      <div class="title-left">
        <span class="icon">
          <img src="@/assets/images/home/titlebar_icon.png">
        </span>
        <span class="title">车辆信息</span>
      </div>
      <div class="title-right">
      </div>
    </div>

   <div class="field" >
      <div class="item" >
         <div class="item-left" ><span class="title">车牌号</span></div>
         <div class="item-middle" ><input type="text" value="粤AT810P" /></div>
         <div class="item-right" ></div>
      </div>
      <div class="item" >
         <div class="item-left" ><span class="title">车架号</span></div>
         <div class="item-middle" ><input type="text" /></div>
         <div class="item-right" ></div>
      </div>
      <div class="item" >
         <div class="item-left" ><span class="title">车型</span></div>
         <div class="item-middle" ><input type="text" /></div>
         <div class="item-right" ></div>
      </div>
      <div class="item" >
         <div class="item-left" ><span class="title">发动机号</span></div>
         <div class="item-middle" ><input type="text" /></div>
         <div class="item-right" ></div>
      </div>
      <div class="item" >
         <div class="item-left" ><span class="title">注册日期</span></div>
         <div class="item-middle" ><input type="text" /></div>
         <div class="item-right" ></div>
      </div>
   </div>
   </div>
  <div class="space"></div>

   <div class="frmgrid" >
    <div class="title">
      <div class="title-left">
        <span class="icon">
          <img src="@/assets/images/home/titlebar_icon.png">
        </span>
        <span class="title">投保信息</span>
      </div>
      <div class="title-right">
      </div>
    </div>

   <div class="field" >
      <div class="item" >
         <div class="item-left" ><span class="title">车主</span></div>
         <div class="item-middle" ><input type="text" value="" /></div>
         <div class="item-right" ></div>
      </div>
       <div class="item" >
         <div class="item-left" ><span class="title">证件号码</span></div>
         <div class="item-middle" ><input type="text" value="" /></div>
         <div class="item-right" ></div>
      </div>
   </div>

   </div>



 <button  @click="goAsChooseKind"  class="lumos-button lumos-button-positive lumos-button-bottom" >下一步</button>

  </div>
</template>

<script>
export default {
  data() {
    return {
    };
  },
  methods: {
    goAsChooseKind() {
       console.log("goAsChooseKind");
         this.$router.push({
         path: "/InsCar/As/ChooseKind",
       });
    }
  }
  
};
</script>

