import axios from 'axios'
import { Notification } from 'element-ui'
import $config from './config'

export default {
  get (filters) {
    filters = {
      ...filters,
      cpf: (filters.cpf) ? filters.cpf.replace(/[.-\s]/g, '') : null
    }
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/Usuario`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data.itens, paginacao: { skip: res.data.skip, totalCount: res.data.totalCount }, status: res.status }
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        erroMsg = ''
        Notification({ title: 'Erro: ERRDBxgetUSU01', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  getSimplified (filters) {
    filters = {
      ...filters,
      cpf: (filters.cpf) ? filters.cpf.replace(/[.-\s]/g, '') : null
    }
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/Usuario/GetSimplified`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data.itens, paginacao: { skip: res.data.skip, totalCount: res.data.totalCount }, status: res.status }
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        erroMsg = ''
        Notification({ title: 'Erro: ERRDBxgetUSU01', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  getById (id) {
    // console.log('chamou o get usuario')
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/Usuario/${id}`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        console.log('e.response', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        erroMsg = ''
        Notification({ title: 'Erro: ERRDBxgetUSU02', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  login (form) {
    return axios({
      method: 'POST',
      data: form,
      url: `${$config.urlApi}/Auth/Login`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        erroMsg = ''
        Notification({ title: 'Verifique os Dados de Login', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  putAceiteTermosDoUso (form) {
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Usuario/AceiteTermosDoUso`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        erroMsg = ''
        Notification({ title: 'Erro ERRDBxputUSU01', message: 'Erro ao Editar Usuário', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  post (form) {
    form = {
      ...form,
      cpf: (form.cpf) ? form.cpf.replace(/[.-\s]/g, '') : null,
      telefone: (form.telefone) ? form.telefone.replace(/[()-\s]/g, '') : null
    }
    return axios({
      method: 'POST',
      data: form,
      url: `${$config.urlApi}/Usuario`
    })
      .then(res => {
        Notification({ title: '', message: 'Usuário cadastrado com sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('e', e)
        let erroMsg = e.response
        if (e.response.status >= 500) erroMsg = 'Erro interno no servidor'
        if (e.response.status < 500) erroMsg = e.response.data
        Notification({ title: '', message: erroMsg, type: 'error' })
        return { status: e.response.status, error: erroMsg }
      })
  },
  put (form, origem) {
    form = {
      ...form,
      cpf: (form.cpf) ? form.cpf.replace(/[.-\s]/g, '') : null,
      telefone: (form.telefone) ? form.telefone.replace(/[()-\s]/g, '') : null
    }
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Usuario/${form.id}`
    })
      .then(res => {
        if (res.status === 200) {
          if (origem != 'selecaoPerfil') Notification({ title: '', message: 'Usuário Editado com Sucesso', type: 'success' })
        }
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        erroMsg = ''
        Notification({ title: 'Erro ERRDBxputUSU01', message: 'Erro ao Editar Usuário', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  delete (id) {
    return axios({
      method: 'DELETE',
      url: `${$config.urlApi}/Usuario/${id}`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Usuário desativado com sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        erroMsg = ''
        Notification({ title: 'Erro ERRDBxDELUSU01', message: 'Erro ao Excluir Indivíduo', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  getRecuperarSenha (cpf, metodo) {
    // console.log('cpf', cpf)
    // console.log('metodo', metodo)
    cpf: (cpf) ? cpf.replace(/[.-\s]/g, '') : null
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/Usuario/Senha/Recuperar/?cpf=${cpf}&metodo=${metodo}`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Codigo Enviado', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        // Notification({ title: 'Erro Na Procura Do Usuario', message: 'Usuario Inválido', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  redefinir (form) {
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Usuario/Senha/Redefinir/`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Troca Da Senha Realizada Com Sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: erroMsg, message: '', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  }
}
