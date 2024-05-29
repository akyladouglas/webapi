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
      url: `${$config.urlApi}/Profissional`
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
      url: `${$config.urlApi}/Profissional/${id}`
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
  getRecuperarSenha(cpf, metodo) {
   // console.log('cpf', cpf)
  //  console.log('metodo', metodo)
    cpf: (cpf) ? cpf.replace(/[.-\s]/g, '') : null
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/Profissional/Senha/Recuperar/?cpf=${cpf}&metodo=${metodo}`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Codigo Enviado', type: 'success' })
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro Na Procura Do Usuario', message: 'Usuario Inválido', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  redefinir(form) {
   // console.log('form', form)
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Profissional/Senha/Redefinir/`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Troca Da Senha Realizada Com Sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro no Usuario'
        Notification({ title: erroMsg, message: '', type: 'error' })
        return { data: erroMsg, status: e.response.status }
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
      url: `${$config.urlApi}/Profissional`
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
        Notification({ title: 'Erro: ERRDBxpstEST01', message: erroMsg, type: 'error' })
        return { status: e.response.status, error: erroMsg }
      })
  },
  postUser (form) {
    form = {
      ...form,
      cpf: (form.cpf) ? form.cpf.replace(/[.-\s]/g, '') : null,
      logradouroCep: (form.logradouroCep) ? form.logradouroCep.replace(/[.-\s]/g, '') : null
    }
    return axios({
      method: 'POST',
      data: form,
      url: `${$config.urlApi}/Profissional/PostUser`
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
        Notification({ title: 'Erro: ERRDBxpstEST01', message: erroMsg, type: 'error' })
        return { status: e.response.status, error: erroMsg }
      })
  },

  putAtivar(form) {
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Profissional/`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Profissional editado com sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxDELEST01', message: 'Erro ao Editar Profissional', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  put (form, origem) {
    form = {
      ...form,
      cpf: (form.cpf) ? form.cpf.replace(/[.-\s]/g, '') : null,
      logradouroCep: (form.logradouroCep) ? form.logradouroCep.replace(/[.-\s]/g, '') : null
    }
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Profissional/${form.id}`
    })
      .then(res => {
        if (res.status === 204) {
          if (origem != "selecaoPerfil") Notification({ title: '', message: 'Profissional Editado com Sucesso', type: 'success' })
        }
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        if (erroMsg.includes("for key 'Cpf'")) {
          erroMsg = 'Esse CPF já está cadastrado!';
        }
        Notification({ title: 'Ocorreu um erro!', message: 'Erro ao Editar Profissional - ' + erroMsg, type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  updateFoto(form) {

    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Profissional/UpdateFoto/${form.id}`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Profissional Editado com Sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxputEST01', message: 'Erro ao Editar Profissional', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  delete (id) {
    return axios({
      method: 'DELETE',
      url: `${$config.urlApi}/Profissional/${id}`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Profissional desativado com sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxDELEST01', message: 'Erro ao Excluir Indivíduo', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  postEstabelecimentoProfissionais (form) {
    return axios({
      method: 'POST',
      data: form,
      url: `${$config.urlApi}/EstabelecimentosProfissional/`
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
  getEstabelecimentoProfissionais (filters) {
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/EstabelecimentosProfissional/${filters.profissionalId}/Estabelecimentos`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        // console.log('LOG', res.data);
        return { data: res.data.estebelecimentos, status: res.status }
      })
      .catch(e => {
        console.log('e.response', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxgetEST02', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  getProfissionalHorarios (filters) {
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/Profissionais/escalas`,
      params: filters
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        console.log('e.response', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxgetPROF07', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  getProfissionaisByProcedimento(filters) {
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/Profissionais/procedimentos`,
      params: filters
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        console.log('e.response', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxgetPROF', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  addToken (form) {
    //console.log('ENTROU NA PI ADD TOKEN', form)

   // console.log('FORM', form)

    return axios({
      method: 'PUT',
      params: {
        token: form.token
      },
      url: `${$config.urlApi}/Profissional/${form.id}/AddToken`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Profissional Editado com Sucesso', type: 'success' })
       // console.log('res put', res.data)
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxputEST01', message: 'Erro ao Editar Profissional', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },

  putAceiteTermosDoUso(form) {
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Profissional/AceiteTermosDoUso`
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
  
}
