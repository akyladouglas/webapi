<template>
   <div class="col-md-3 rooms">
       <div class="room" v-for="room in rooms" v-bind:key="room.id" @click="showRoom(room.name)">
           {{room.name}}
       </div>
   </div>
</template>

<script>
import { EventBus } from '../../Event'
import AddRoom from './AddRoom'

export default {
  name: 'Rooms', // Name of the component
  data () {
    return {
      rooms: [
        {id: 1, name: 'dc8338d9-67a2-4ed9-b86b-54e5529cc995'}
      ],
      roomCount: 1, // used to keep track of the number of rooms present
      loading: false // indicate when tracks in a room is being loaded
    }
  },
  components: {
    AddRoom // imported AddRoom component
  },
  created () {
    EventBus.$on('new_room', (data) => {
      this.roomCount++
      this.rooms.push({id: this.roomCount, name: data})
    })
  },
  methods: {
    showRoom (room) {
      EventBus.$emit('show_room', room)
    }
  }

}
</script>

<style scoped> /* scoped attribute is used here so the styling applies to this component alone */
   .rooms > .room {
       border: 1px solid rgb(124, 129, 124);
       padding: 13px;
       margin: 3px 0px;
       color: #000;
   }

   .rooms {
     border: 1px solid rgb(64, 68, 64);
     cursor: pointer;
   }
</style>
