import axios from 'axios'
import { Notification } from 'element-ui'
import $config from './config'

export default {
  get (filters) {
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/Acompanhamento`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data.items, paginacao: { skip: res.data.skip, totalCount: res.data.totalCount }, status: res.status }
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxgetPROC01', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  getById (id) {
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/Acompanhamento/${id}`
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
  post (form) {
    return axios({
      method: 'POST',
      data: form,
      url: `${$config.urlApi}/Acompanhamento`
    })
      .then(res => {
        //Notification({ title: '', message: 'Operação Realizada com Sucesso', type: 'success' })
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
  atualizarSintomas(individuoId, form) {
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Individuo/SintomasTriagem/${individuoId}`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        console.log("ERROR REQ", e);
        let erro = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxpgetIND02', message: erro.data, type: 'error' })
        return { status: erro.status, error: erro.data }
      })
  },

  atualizarAcompanhamento(atendimentoId, acompanhamento) {
    console.log('ATENDIMENTO#', atendimentoId);
    console.log('ACOMPANHAMENTO#', acompanhamento);
    return axios({
      method: 'PUT',
      data: acompanhamento,
      url: `${$config.urlApi}/Acompanhamento/AtualizarAcompanhamento/${atendimentoId}`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        console.log("ERROR REQ", e);
        let erro = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxpgetIND02', message: erro.data, type: 'error' })
        return { status: erro.status, error: erro.data }
      })
  },
 
  
}
