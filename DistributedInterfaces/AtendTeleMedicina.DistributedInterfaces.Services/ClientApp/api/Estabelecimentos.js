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
      url: `${$config.urlApi}/Estabelecimentos`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data.items, paginacao: { skip: res.data.skip, totalCount: res.data.totalCount }, status: res.status }
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxgetEST01', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  getById (id) {
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/Estabelecimentos/${id}`
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
  post (form) {
    form = {
      ...form,
      cnpj: (form.cnpj) ? form.cnpj.replace(/[.-\s]/g, '') : null,
      responsavelCpf: (form.responsavelCpf) ? form.responsavelCpf.replace(/[.-\s]/g, '') : null,
      logradouroCep: (form.logradouroCep) ? form.logradouroCep.replace(/[.-\s]/g, '') : null
    }
    return axios({
      method: 'POST',
      data: form,
      url: `${$config.urlApi}/Estabelecimentos`
    })
      .then(res => {
        Notification({ title: '', message: 'Operação realizada com sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('e', e)
        let erroMsg = e.response
        if (e.response.status >= 500) erroMsg = 'Erro interno no servidor'
        if (e.response.status < 500) erroMsg = e.response.data
        Notification({ title: 'Erro ao Criar Estabelecimento', message: 'Cpf Ja Cadastrado Como Responsavel de Um Estabelecimento', type: 'error' })
        return { status: e.response.status, error: erroMsg }
      })
  },
  put (form) {
    form = {
      ...form,
      cnpj: (form.cnpj) ? form.cnpj.replace(/[.-\s]/g, '') : null,
      responsavelCpf: (form.responsavelCpf) ? form.responsavelCpf.replace(/[.-\s]/g, '') : null,
      cep: (form.cep) ? form.cep.replace(/[.-\s]/g, '') : null
    }
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Estabelecimentos/${form.id}`
    })
      .then(res => {
        if (res.status === 204) Notification({ title: '', message: 'Estabelecimento editado com sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxputEST01', message: 'Erro ao Editar Estabelecimento', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  delete (form) {
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Estabelecimentos/`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Estabelecimento editado com sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxDELEST01', message: 'Erro ao Editar Estabelecimento', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  getEstabelecimentoProcedimentos (filters) {
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/Estabelecimentos/${filters.estabelecimentoId}/Procedimentos`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data.procedimentos, status: res.status }
      })
      .catch(e => {
        console.log('e.response', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxgetEST02', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  postEstabelecimentoProcedimentos (form) {
    return axios({
      method: 'POST',
      data: form,
      url: `${$config.urlApi}/Estabelecimentos/Procedimentos`
    })
      .then(res => {
        Notification({ title: '', message: 'Operação Realizada com Sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('e', e.response)
        let erroMsg = e.response
        if (e.response.status >= 500) erroMsg = 'Erro interno no servidor'
        if (e.response.status < 500) erroMsg = e.response.data
        Notification({ title: 'Erro: ERRDBxpstEST01', message: erroMsg, type: 'error' })
        return { status: e.response.status, error: erroMsg }
      })
  },
  deleteEstabelecimentoProcedimentos(form) {
    //form = {
    //  ...form,
    //}
    return axios({
      method: 'DELETE',
      data: form,
      url: `${$config.urlApi}/Estabelecimentos/DeleteProcedimentos`
    })
      .then(res => {
        //Notification({ title: '', message: 'Operação Realizada com Sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        erroMsg = ''
        //Notification({ title: 'Erro: ERRDBxpstIND01', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  distribuirCota (form) {
    return axios({
      method: 'PUT',
      url: `${$config.urlApi}/Estabelecimentos/${form.estabelecimentoId}/distribuir-cota`,
      params: form
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Cota distribuida com Sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxputPROC01', message: 'Erro ao adicionar a Cota', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  }
}
