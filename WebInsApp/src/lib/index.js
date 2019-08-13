import tabbar from "./tabbar/src/tabbar";
import header from "./header/src/header";
import switchc from "./switch/src/switch";
import plateNumber from "./plateNumber/src/plateNumber";
import cityselect from "./cityselect/src/cityselect";
import swiper from "./swiper/src/swiper";
import test from "./test/src/test";
import Loading from './loading'
import Confirm from './confirm'
import Toast from './toast'

// import loading from "./loading/src/loading";
// const components={
//     install(Vue){
//         Vue.component("lumos-tabbar",tabbar)
//         Vue.component("lumos-header",header)
//     }
// };

// if(typeof window !=='undefined'&&window.Vue){
//     window.Vue.use(components)
// }

// export default components;


var components={
    tabbar,
    header,
    cityselect,
    plateNumber,
    switchc,
    swiper,
    test
}

var uses={
    Loading,
    Confirm,
    Toast
}

export default {
    components,
    uses,
    Toast
}