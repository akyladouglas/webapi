import axios from 'axios'
import { Notification } from 'element-ui'
import $config from './config'

export default {
  twCreateRoom (params) {
    return axios({
      method: 'POST',
      params: params,
      url: `${$config.urlApi}/TeleConsulta`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Aviso', message: 'A Sala de Atendimento já estava aberta', type: 'warning' })
        return { data: e.response.data, status: 400, error: erroMsg }
      })
  },
  twGetToken (id) {
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/TeleConsulta/${id}`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        //Notification({ title: 'Erro: ERRDBxgetTELE01', message: erroMsg, type: 'error' })
        Notification({ title: 'Erro ao abrir ou entrar a sala', type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  twGetChatToken (id) {
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/TeleConsulta/GetUserChatToken/${id}`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxgetTELE01', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  }

}
