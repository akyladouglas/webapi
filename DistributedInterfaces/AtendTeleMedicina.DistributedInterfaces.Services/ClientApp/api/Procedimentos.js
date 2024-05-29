import axios from 'axios'
import { Notification } from 'element-ui'
import $config from './config'

export default {
  get (filters) {
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/Procedimento`
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
      url: `${$config.urlApi}/Procedimento/${id}`
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
      url: `${$config.urlApi}/Procedimento`
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
  put (form) {
    form = {
      ...form,
      cnpj: (form.cnpj) ? form.cnpj.replace(/[.-\s]/g, '') : null,
      logradouroCep: (form.logradouroCep) ? form.logradouroCep.replace(/[.-\s]/g, '') : null
    }
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Procedimento/${form.id}`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Procedimento Editado com Sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ao Editar Procedimento', message: 'Verifique os Campos do Formulario', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  delete (id) {
    return axios({
      method: 'DELETE',
      url: `${$config.urlApi}/Procedimento/${id}`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Procedimento desativado com sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxDELPROC01', message: 'Erro ao Excluir Indivíduo', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  }
}
