import axios from 'axios'
import { Notification } from 'element-ui'
import $config from './config'

export default {
  get () {
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/Memed/`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        console.log('e.response', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxgetEST02', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },

  post (data) {
    return axios({
      method: 'POST',
      data: data,
      url: `${$config.urlApi}/Memed`
    })
      .then(res => {
        Notification({ title: '', message: 'Operação Realizada com Sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('e', e)
        let erroMsg = e.response
        if (e.response.status >= 500) erroMsg = 'Erro interno no servidor'
        if (e.response.status < 500) erroMsg = e.response.data
        Notification({ title: 'Erro: ERRDBxpstPROC01', message: erroMsg, type: 'error' })
        return { status: e.response.status, error: erroMsg }
      })
  },
  getById (id) {
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/Ciap/${id}`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        console.log('e.response', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxgetPROC02', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  getPrescriptionsById (id, token, idPaciente, idProfissional, idAgendamento) {
    // console.log('dando console no id dentro da api memed getPrescriptionsById', id)

    return axios({
      method: 'post',
      url: `${$config.urlApi}/Memed/RetornoDocumentos/${id}/${token}`,
      data: {
        individuoId: idPaciente,
        profissionalId: idProfissional,
        agendamentoId: idAgendamento
      }
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        // console.log('ta retornando api memed getPrescriptionsbyid e retornando para controller memed RetornoDocumentos: ', data)
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        //console.log('e.response', e.response)
        Notification({ title: 'Erro: ERRDBxgetPROC02', message: erroMsg, type: 'error' })
       // console.log('ta retornando erro na api memed getPrescriptionsById')
        return { status: 400, error: erroMsg }
      })
  },

  recoverTokenMedico () {
    // console.log('entrando na apiiiiiiii')
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/Memed/GetToken/`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data.data, status: res.status }
      })
      .catch(e => {
        // console.log('e.response', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxgetPROC02', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
        // console.log('ta retornando erro')
      })
  },
  configImpressao () {
    return axios({
      method: 'POST',
      url: `${$config.urlApi}/Memed/ConfigImpressao`
    })
    .then(res => {
      Notification({ title: '', message: 'Operação Realizada com Sucesso', type: 'success' })
      return res
    })
    .catch(e => {
      console.log('e', e)
      let erroMsg = e.response
      if (e.response.status >= 500) erroMsg = 'Erro interno no servidor'
      if (e.response.status < 500) erroMsg = e.response.data
      Notification({ title: 'Erro: ERRDBxpstPROC01', message: erroMsg, type: 'error' })
      return { status: e.response.status, error: erroMsg }
    })
  }

}
