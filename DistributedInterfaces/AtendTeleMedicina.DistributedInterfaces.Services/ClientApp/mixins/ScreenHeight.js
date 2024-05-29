export default {
  data() {
    return {
      screenHeight: window.innerHeight,
    };
  },
  computed: {
    dynamicHeight() {
      return this.screenHeight;
    },
  },
  mounted() {
    window.addEventListener('resize', this.updateScreenHeight);
    this.updateScreenHeight();
  },
  beforeDestroy() {
    window.removeEventListener('resize', this.updateScreenHeight);
  },
  methods: {
    updateScreenHeight() {
      this.screenHeight = window.innerHeight;
    },
    mxDynamicHeight() {
      return this.dynamicHeight
    }
  },
};
