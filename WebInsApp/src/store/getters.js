export const getters = {
    getAId: (state) => state.aId,
    getUId: (state) => state.uId,
    getMessagesBox(state) {
        state.messageBox = JSON.parse(localStorage.getItem("MESSAGEBOX"));
        return state.messageBox;
    },
    getUserInfo(state) {
        var userInfo = localStorage.getItem("USERINFO");
        if (userInfo != null) {
            state.userInfo = JSON.parse(userInfo);
        }
        return state.userInfo;
    }
}
