import axios from 'axios'
import { Notification } from 'element-ui'
import $config from './config'

export default {
  getContadorAtendimentos(filters) {
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/Dashboard/ContadorAtendimentos`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response.data ? e.response.data : 'Erro de comunicação com a API'
        erroMsg = ''
        Notification({ title: 'Erro: ERRDBxgetDASH01', message: erroMsg, type: 'error' })
        return { error: erroMsg }
      })
  },
  getContadorAtendimentosPacientes(filters) {
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/Dashboard/ContadorAtendimentosPacientes`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response.data ? e.response.data : 'Erro de comunicação com a API'
        erroMsg = ''
        Notification({ title: 'Erro: ERRDBxgetDASH02', message: erroMsg, type: 'error' })
        return { error: erroMsg }
      })
  },

}
