export default {
  data() {
    return {
      screenWidth: window.innerWidth,
    };
  },
  computed: {
    dynamicWidth() {
      return this.screenWidth;
    },
  },
  mounted() {
    window.addEventListener('resize', this.updateScreenWidth);
    this.updateScreenWidth();
  },
  beforeDestroy() {
    window.removeEventListener('resize', this.updateScreenWidth);
  },
  methods: {
    updateScreenWidth() {
      this.screenWidth = window.innerWidth;
    },

    mxDynamicWidth() {
      return this.dynamicWidth
    }
  },
};
