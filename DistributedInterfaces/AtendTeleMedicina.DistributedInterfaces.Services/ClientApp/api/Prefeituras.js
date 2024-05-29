import axios from 'axios'
import { Notification } from 'element-ui'
import $config from './config'

export default {
  get (filters) {
    filters = {
      ...filters,
      cnpj: (filters.cnpj) ? filters.cnpj.replace(/[.-\s]/g, '') : null,
      logradouroCep: (filters.logradouroCep) ? filters.logradouroCep.replace(/[.-\s]/g, '') : null
    }
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/Prefeitura`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data.items, paginacao: { currentPage: res.data.skip, totalCount: res.data.totalCount }, status: res.status }
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxgetPREF01', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  getById (id) {
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/Prefeitura/${id}`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        console.log('e.response', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxgetPREF02', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  post (form) {
    form = {
      ...form,
      cnpj: (form.cnpj) ? form.cnpj.replace(/[.-\s]/g, '') : null,
      logradouroCep: (form.logradouroCep) ? form.logradouroCep.replace(/[.-\s]/g, '') : null
    }
    return axios({
      method: 'POST',
      data: form,
      url: `${$config.urlApi}/Prefeitura`
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
        Notification({ title: 'Erro: ERRDBxpstPREF01', message: erroMsg, type: 'error' })
        return { status: e.response.status, error: erroMsg }
      })
  },
  put (form) {
    form = {
      ...form,
      cnpj: (form.cnpj) ? form.cnpj.replace(/[.-\s]/g, '') : null,
      logradouroCep: (form.logradouroCep) ? form.logradouroCep.replace(/[.-\s]/g, '') : null
    }
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Prefeitura/${form.id}`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Prefeitura Editada com Sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxputPREF01', message: 'Erro ao Editar Prefeitura', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  delete (id) {
    return axios({
      method: 'DELETE',
      url: `${$config.urlApi}/Prefeitura/${id}`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Prefeitura desativada com sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxDELPREF01', message: 'Erro ao Excluir Indivíduo', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  }
}
