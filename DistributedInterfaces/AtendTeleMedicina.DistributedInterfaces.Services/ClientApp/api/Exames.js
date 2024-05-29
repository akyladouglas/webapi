import axios from 'axios'
import { Notification } from 'element-ui'
import $config from './config'
import moment from 'moment'
moment.locale('pt-br')

export default {
  getExames(filters) {
    //console.log('filters', filters)
    filters = {
      ...filters,
    }
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/Exames/GetExames/`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data.items, paginacao: { currentPage: res.data.skip, totalCount: res.data.totalCount }, status: res.status }
        //console.log(res)
      })
      .catch(e => {
        console.log('erroEstabelecimentoImagem', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERROxgetEXAMES', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  getAuscultacao(filters) {
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/Exames/MedicoesEko/`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data.items, paginacao: { currentPage: res.data.skip, totalCount: res.data.totalCount }, status: res.status }
      })
      .catch(e => {
        console.log('errogetAuscultacao', e.response)
        //let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        //Notification({ title: 'Erro: ERROxgetAUSCULTACAO', message: erroMsg, type: 'error' })
        //return { status: 400, error: erroMsg }
      })
  },

  postExames(arr) {
    //console.log('arr', arr)
    return axios({
      method: 'POST',
      data: arr,
      url: `${$config.urlApi}/Exames/ReceberExames/`,
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        console.log('e.response', e.response)
        Notification({ title: 'Erro: ERRDBxgetPROC02', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },

  getTipoExames(filters) {
    //console.log("filters ->", filters)
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/Exames/GetTipoExames/`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        //console.log(res)
        return { data: res.data.items, paginacao: { currentPage: res.data.skip, totalCount: res.data.totalCount }, status: res.status }
       // console.log(res)
      })
      .catch(e => {
        console.log('erroEstabelecimentoImagem', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxgetESTIMG01', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },

  getByIdTipoExames(id) {
    //console.log('id', id)
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/Exames/GetByIdTipoExames/${id}`
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

  putAvaliacao(form) {

    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Exames/AvaliarExames`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Avaliação de Exame Inserido com Sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxputExame01', message: 'Erro ao Avaliar Exame', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },

  //
  //post (form) {
  //  form = {
  //    ...form,
  //    cnpj: (form.cnpj) ? form.cnpj.replace(/[.-\s]/g, '') : null,
  //    logradouroCep: (form.logradouroCep) ? form.logradouroCep.replace(/[.-\s]/g, '') : null
  //  }
  //  return axios({
  //    method: 'POST',
  //    data: form,
  //    url: `${$config.urlApi}/Estabelecimentos`
  //  })
  //    .then(res => {
  //      Notification({ title: '', message: 'Operação Realizada com Sucesso', type: 'success' })
  //      return res
  //    })
  //    .catch(e => {
  //      console.log('e', e)
  //      let erroMsg = e.response
  //      if (e.response.status >= 500) erroMsg = 'Erro interno no servidor'
  //      if (e.response.status < 500) erroMsg = e.response.data
  //      Notification({ title: 'Erro: ERRDBxpstEST01', message: erroMsg, type: 'error' })
  //      return { status: e.response.status, error: erroMsg }
  //    })
  //},
  //put (form) {
  //  form = {
  //    ...form,
  //    cnpj: (form.cnpj) ? form.cnpj.replace(/[.-\s]/g, '') : null,
  //    logradouroCep: (form.logradouroCep) ? form.logradouroCep.replace(/[.-\s]/g, '') : null
  //  }
  //  return axios({
  //    method: 'PUT',
  //    data: form,
  //    url: `${$config.urlApi}/Estabelecimentos/${form.id}`
  //  })
  //    .then(res => {
  //      if (res.status === 200) Notification({ title: '', message: 'Estabelecimento Editado com Sucesso', type: 'success' })
  //      return res
  //    })
  //    .catch(e => {
  //      console.log('erro', e.response)
  //      let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
  //      Notification({ title: 'Erro ERRDBxputEST01', message: 'Erro ao Editar Estabelecimento', type: 'error' })
  //      return { data: erroMsg, status: e.response.status }
  //    })
  //},
  //delete (id) {
  //  return axios({
  //    method: 'DELETE',
  //    url: `${$config.urlApi}/Estabelecimentos/${id}`
  //  })
  //    .then(res => {
  //      if (res.status === 200) Notification({ title: '', message: 'Estabelecimento desativado com sucesso', type: 'success' })
  //      return res
  //    })
  //    .catch(e => {
  //      console.log('erro', e.response)
  //      let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
  //      Notification({ title: 'Erro ERRDBxDELEST01', message: 'Erro ao Excluir Indivíduo', type: 'error' })
  //      return { data: erroMsg, status: e.response.status }
  //    })
  //},
  //getEstabelecimentoProcedimentos (filters) {
  //  return axios({
  //    method: 'GET',
  //    url: `${$config.urlApi}/Estabelecimentos/${filters.estabelecimentoId}/Procedimentos`
  //  })
  //    .then(res => {
  //      if (res.status !== 200) return { status: res.status }
  //      return { data: res.data.procedimentos, status: res.status }
  //    })
  //    .catch(e => {
  //      console.log('e.response', e.response)
  //      let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
  //      Notification({ title: 'Erro: ERRDBxgetEST02', message: erroMsg, type: 'error' })
  //      return { status: 400, error: erroMsg }
  //    })
  //},
  //postEstabelecimentoProcedimentos (form) {
  //  return axios({
  //    method: 'POST',
  //    data: form,
  //    url: `${$config.urlApi}/Estabelecimentos/Procedimentos`
  //  })
  //    .then(res => {
  //      Notification({ title: '', message: 'Operação Realizada com Sucesso', type: 'success' })
  //      return res
  //    })
  //    .catch(e => {
  //      console.log('e', e.response)
  //      let erroMsg = e.response
  //      if (e.response.status >= 500) erroMsg = 'Erro interno no servidor'
  //      if (e.response.status < 500) erroMsg = e.response.data
  //      Notification({ title: 'Erro: ERRDBxpstEST01', message: erroMsg, type: 'error' })
  //      return { status: e.response.status, error: erroMsg }
  //    })
  //},
  //distribuirCota (form) {
  //  return axios({
  //    method: 'PUT',
  //    url: `${$config.urlApi}/Estabelecimentos/${form.estabelecimentoId}/distribuir-cota`,
  //    params: form
  //  })
  //    .then(res => {
  //      if (res.status === 200) Notification({ title: '', message: 'Cota distribuida com Sucesso', type: 'success' })
  //      return res
  //    })
  //    .catch(e => {
  //      console.log('erro', e.response)
  //      let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
  //      Notification({ title: 'Erro ERRDBxputPROC01', message: 'Erro ao adicionar a Cota', type: 'error' })
  //      return { data: erroMsg, status: e.response.status }
  //    })
  //}
}
