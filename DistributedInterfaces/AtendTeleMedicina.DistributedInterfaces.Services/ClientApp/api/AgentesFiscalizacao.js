import axios from 'axios'
import { Notification } from 'element-ui'
import $config from './config'

export default {
  get (filters) {
    filters = {
      ...filters,
      cpf: (filters.cpf) ? filters.cpf.replace(/[.-\s]/g, '') : null,
      logradouroCep: (filters.logradouroCep) ? filters.logradouroCep.replace(/[.-\s]/g, '') : null
    }
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/AgenteFiscalizacao`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data.items, paginacao: { currentPage: res.data.skip, totalCount: res.data.totalCount }, status: res.status }
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxgetAGF01', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  getById (id) {
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/AgenteFiscalizacao/${id}`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        console.log('e.response', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxgetAGF02', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  post (form) {
    form = {
      ...form,
      cpf: (form.cpf) ? form.cpf.replace(/[.-\s]/g, '') : null,
      logradouroCep: (form.logradouroCep) ? form.logradouroCep.replace(/[.-\s]/g, '') : null
    }
    return axios({
      method: 'POST',
      data: form,
      url: `${$config.urlApi}/AgenteFiscalizacao`
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
        Notification({ title: 'Erro: ERRDBxpstAGF01', message: erroMsg, type: 'error' })
        return { status: e.response.status, error: erroMsg }
      })
  },
  put (form) {
    form = {
      ...form,
      cpf: (form.cpf) ? form.cpf.replace(/[.-\s]/g, '') : null,
      logradouroCep: (form.logradouroCep) ? form.logradouroCep.replace(/[.-\s]/g, '') : null
    }
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/AgenteFiscalizacao/${form.id}`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Agente Fiscalização Editado com Sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxputAGF01', message: 'Erro ao Editar Agente Fiscalização', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  delete (id) {
    return axios({
      method: 'DELETE',
      url: `${$config.urlApi}/AgenteFiscalizacao/${id}`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Agente Fiscalização desativado com sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxDELAGF01', message: 'Erro ao Excluir Indivíduo', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  }
}
