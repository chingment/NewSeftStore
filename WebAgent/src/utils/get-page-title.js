// import defaultSettings from '@/settings'

const title = process.env.VUE_APP_WEBSITE_NAME

export default function getPageTitle(pageTitle) {
  if (pageTitle) {
    return `${pageTitle} - ${title}`
  }
  return `${title}`
}
