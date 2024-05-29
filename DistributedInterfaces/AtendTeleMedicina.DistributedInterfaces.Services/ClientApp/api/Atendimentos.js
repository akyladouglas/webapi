import axios from 'axios'
import { Notification } from 'element-ui'
import $config from './config'

export default {
  get (filters) {
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/Atendimento`
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
      url: `${$config.urlApi}/Atendimento/${id}`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, paginacao: res.paginacao, status: res.status }
      })
      .catch(e => {
        console.log('e.response', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxgetPROC02', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  getbyAgendamentoId(id) {
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/Atendimento/GetAgendamentoById/${id}`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        console.log('e.response', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        //Notification({ title: 'Erro: ERRDBxgetPROC02', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  
  getMaxAtendimentoByIndividuoId(id) {
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/Atendimento/GetMaxAtendimentoByIndividuoId/${id}`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        console.log('e.response', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        //Notification({ title: 'Erro: ERRDBxgetPROC02', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  post(form) {
    return axios({
      method: 'POST',
      data: form,
      url: `${$config.urlApi}/Atendimento`
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
  postMedico(form) {
    return axios({
      method: 'POST',
      data: form,
      url: `${$config.urlApi}/Atendimento/PostMedico`
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
  putAtendimento(form) {
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Atendimento/${form.id}`
    })
      .then(res => {
        if (res.status === 204) return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxputPROC01', message: 'Erro ao atualizar o atendimento', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  put (form) {
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
        Notification({ title: 'Erro ERRDBxputPROC01', message: 'Erro ao Editar Procedimento', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  putInativar(form) {
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Atendimento/PutInativar/${form.id}`
    })
      .then(res => {
        if (res.status === 204) Notification({ title: '', message: 'Atendimento Inativado com Sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxputPROC01', message: 'Erro ao inativar o atendimento', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  delete (id) {
    return axios({
      method: 'DELETE',
      url: `${$config.urlApi}/Atendimento/${id}`
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
  },
  getContadorTotalTipoAtendimento(filters) {
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/Atendimento/ContadorTotalTipoAtendimento`
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
  }
}
