export const mutations = {
	//更新用户ID标识
	SETAID(state, val) {
		state.aId = val;
	},
	//更新用户代理标识
	SETUID(state, val) {
		state.uId = val;
	},
	SETMESSAGEBOX(state, val) {
		localStorage.setItem("MESSAGEBOX",JSON.stringify(val));
		state.messageBox = val;
	},
	SETUSERINFO(state, val) {
		localStorage.setItem("USERINFO",JSON.stringify(val));
		state.userInfo = val;
	}

}