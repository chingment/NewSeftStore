export const actions = {
	//更新用户ID标识
	setAId({ commit }, val) {
		commit('SETAID', val);
	},
	//更新用户代理标识
	setUId({ commit }, val) {
		commit('SETUID', val);
	},
	//更新提示盒子信息
	setMessageBox({ commit }, val) {
		commit('SETMESSAGEBOX', val);
	},
	//更新用户信息
	setUserInfo({ commit }, val) {
		commit('SETUSERINFO', val);
	}

}