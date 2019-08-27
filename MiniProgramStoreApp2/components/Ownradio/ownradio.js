Component({

  properties: {
    name: {          
      type: String,    
      value: 'Ownradio1'    
    }
  },
  data: {
      isSelected: false
  },
  methods: {
    _radioCheckEvent() {
      console.log("_radioCheckEvent")
    
      this.setData({
        isSelected: !this.data.isSelected
      })
    }
  }
})
