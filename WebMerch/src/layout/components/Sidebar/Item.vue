<script>
export default {
  name: 'MenuItem',
  functional: true,
  props: {
    icon: {
      type: String,
      default: ''
    },
    title: {
      type: String,
      default: ''
    }
  },
  render(h, context) {
    const { icon, title } = context.props
    const vnodes = []

    if (icon) {
      vnodes.push(<svg-icon icon-class={icon}/>)
    }
    // <el-badge value='0' max='0' class='item'/>
    if (title) {
      vnodes.push(<span slot='title'>{(title)}</span>)
    }

    return vnodes
  }
}
</script>
