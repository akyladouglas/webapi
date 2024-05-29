import $config from './api/config'
import { HubConnectionBuilder, LogLevel } from '@aspnet/signalr'

export default class Hub {
  constructor () {
    this.connection = new HubConnectionBuilder()
      .withUrl(`/Hub`)
      // .withUrl('http://localhost:5001/Hub')
      .configureLogging(LogLevel.Information)
      .build()
  }
}
