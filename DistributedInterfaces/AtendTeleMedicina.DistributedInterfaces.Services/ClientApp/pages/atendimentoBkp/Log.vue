<template>
    <!-- <el-row>
        <el-col :span="24" class="box">
            <ul class="list-unstyled">
                <li class="log"  v-for="log in logs" v-bind:key="log.id">
                    {{log.message}}
                </li>
            </ul>
        </el-col>
    </el-row> -->
</template>

<script>

import { EventBus } from '../../Event'

export default {
  name: 'Logs',
  data () {
    return {
      logs: [],
      logCount: 0
    }
  },
  created () {
    EventBus.$on('new_log', (message) => {
      this.logs.push({id: this.logCount, message: message})

      this.logCount += 1
    })
  }
}
</script>

<style scoped>
    .log {
        border-bottom: 1px solid rgb(124, 129, 124);
        padding: 13px;
        margin: 3px 0px;
        font-size: 10px;
    }

    .box {
        margin-top: 20px;
        border: 1px solid #e7e7e7;
        padding: 5px;
    }
</style>