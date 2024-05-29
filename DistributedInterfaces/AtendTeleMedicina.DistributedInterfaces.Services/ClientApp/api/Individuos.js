import axios from 'axios'
import { Notification } from 'element-ui'
import $config from './config'

export default {
  get (filters) {
    filters = {
      ...filters,
      cpf: (filters.cpf) ? filters.cpf.replace(/[.-\s]/g, '') : null,
      corStatusIn: filters.corStatusIn ? filters.corStatusIn.join() : null
    }
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/AtendTeleMedicina/Lista`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data.fichas, paginacao: { skip: res.data.page, totalCount: res.data.totalCount }, status: res.status }
      })
      .catch(e => {
        console.log('e', e)
        let erro = e.response
        if (e.response.status >= 500) erro = 'Erro interno no servidor'
        if (e.response.status < 500) erro = e.response.data
        Notification({ title: 'Erro: ERRDBxpgetIND01', message: erro, type: 'error' })
        return { status: e.response.status, error: erro }
      })
  },
  getRecuperarSenha(cpf, metodo) {
    cpf: (cpf) ? cpf.replace(/[.-\s]/g, '') : null
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/Individuo/Senha/Recuperar/?cpf=${cpf}&metodo=${metodo}`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Codigo Enviado', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxDELIND01', message: 'Erro', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  redefinir(form) {
   // console.log('form', form)
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Individuo/Senha/Redefinir/`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Troca Da Senha Realizada Com Sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxDELIND01', message: 'Erro', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  getAll(filters) {
    filters = {
      ...filters,
      cpf: (filters.cpf) ? filters.cpf.replace(/[.-\s]/g, '') : null,
      corStatusIn: filters.corStatusIn ? filters.corStatusIn.join() : null
    }
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/Individuo`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
     //   console.log("RES LOG", res.data);
        return { data: res.data.items, paginacao: { skip: res.data.page, totalCount: res.data.totalCount }, status: res.status }
      })
      .catch(e => {
        console.log('e', e)
        let erro = e.response
        if (e.response.status >= 500) erro = 'Erro interno no servidor'
        if (e.response.status < 500) erro = e.response.data
        Notification({ title: 'Erro: ERRDBxpgetIND01', message: erro, type: 'error' })
        return { status: e.response.status, error: erro }
      })
  },

  getById (id) {
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/Individuo/${id}`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        console.log("ERROR REQ", e);
        let erro = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxpgetIND021', message: erro.data, type: 'error' })
        return { status: erro.status, error: erro.data }
      })
  },

  getMapeamento (filters) {
    filters = {
      ...filters,
      corStatusIn: filters.corStatusIn ? filters.corStatusIn.join() : null
    }
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/AtendTeleMedicina/MapaAcompanhamento`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        console.log('e', e.response)
        let erro = e.response
        if (e.response.status >= 500) erro = 'Erro interno no servidor'
        if (e.response.status < 500) erro = e.response.data
        Notification({ title: 'Erro: ERRDBxgetMAP01', message: erro, type: 'error' })
        return { status: e.response.status, error: erro }
      })
  },
  getListaCSV (data) {
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/AtendTeleMedicina/ListaCSV?data=${data}`
    })
      .then(res => {
     //   console.log('res', res)
        if (res.status !== 200) return { status: res.status }
        const url = window.URL.createObjectURL(new Blob([res.data]))
        const link = document.createElement('a')
        link.href = url
        link.setAttribute('download', 'MonitoraCovid.zip')
        document.body.appendChild(link)
        link.click()
        // let blob = window.URL.createObjectURL(new Blob([res.data], { type: 'application/zip' }))
        // let link = document.createElement('a')
        // link.href = blob
        // link.setAttribute(`download`, `Individuos.zip`)
        // document.body.appendChild(link)
        // link.click()
        // setTimeout(function () {
        //   document.body.removeChild(link)
        //   window.URL.revokeObjectURL(blob)
        // }, 100)
        return { status: res.status }
      })
      .catch(e => {
        console.log('e', e.response)
        let erro = e.response
        if (e.response.status >= 500) erro = 'Erro interno no servidor'
        if (e.response.status < 500) erro = e.response.data
        Notification({ title: 'Erro: ERRDBxgetMAP01', message: erro, type: 'error' })
        return { status: e.response.status, error: erro }
      })
  },
  post(form) {
    form = {
      ...form,
      cpf: (form.cpf) ? form.cpf.replace(/[.-\s]/g, '') : null,
      telefoneCelular: (form.telefoneCelular) ? form.telefoneCelular.replace(/[()-\s]/g, '') : null,
      telefoneResidencia: (form.telefoneResidencia) ? form.telefoneResidencia.replace(/[()-\s]/g, '') : null,
      telefoneContatoProximo: (form.telefoneContatoProximo) ? form.telefoneContatoProximo.replace(/[()-\s]/g, '') : null,
      logradouroCep: (form.logradouroCep) ? form.logradouroCep.replace(/[.-\s]/g, '') : null,
    }
    console.log("form individuo api", form)
    return axios({
      method: 'POST',
      data: form,
      url: `${$config.urlApi}/Individuo`
    })
      .then(res => {
        //Notification({ title: '', message: 'Operação Realizada com Sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        //console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro ao cadastrar paciente.'
        console.log('ERRO RETORNO API: ', e.response.data)
        if (e.response.data == "Telefone já cadastrado.") {
          erroMsg = 'Esse Telefone já está cadastrado!';
        }
        console.log('ERRO POST: ', e.response.data.message)
        Notification({ title: '', message: 'Ocorreu um erro:  ' + erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  postPacientes(form) {
    //form = {
    //  ...form,
    //}
    return axios({
      method: 'POST',
      data: form,
      url: `${$config.urlApi}/IndividuoProcedimentoAutorizacao/`
    })
      .then(res => {
        Notification({ title: '', message: 'Operação Realizada com Sucesso', type: 'success' })
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
  deleteProcedimentos(form) {
    //form = {
    //  ...form,
    //}
    return axios({
      method: 'DELETE',
      data: form,
      url: `${$config.urlApi}/IndividuoProcedimentoAutorizacao`
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
  put(form) {
    console.log(form)
    form = {
      ...form,
      cpf: (form.cpf) ? form.cpf.replace(/[.-\s]/g, '') : null,
      telefoneCelular: (form.telefoneCelular) ? form.telefoneCelular.replace(/[()-\s]/g, '') : null,
      telefoneResidencia: (form.telefoneResidencia) ? form.telefoneResidencia.replace(/[()-\s]/g, '') : null,
      telefoneContatoProximo: (form.telefoneContatoProximo) ? form.telefoneContatoProximo.replace(/[()-\s]/g, '') : null,
      logradouroCep: (form.logradouroCep) ? form.logradouroCep.replace(/[.-\s]/g, '') : null,
    }
    
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Individuo/${form.id}`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Cadastro do Paciente editado com sucesso!', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        if (erroMsg.includes("for key 'IX_UNIQUE_TELEFONE'")) {
          erroMsg = 'Esse Telefone já está cadastrado!';
        }
        Notification({ title: 'Oops!', message: 'Ocorreu um erro ao editar o paciente -' + erroMsg, type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  atualizarAlergiasAntecedentes(id, form) {
    
    console.log('PUT INDIVIDUO#')
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Individuo/${id}/PutAlergiasAntecedentes`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Informações adicionais editadas com sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        erroMsg = ''
        Notification({ title: 'Erro ERRDBxputIND01', message: 'Erro ao Editar Indivíduo', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  putSenha (form) {
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Senha/Redefinir`
    })
      .then(res => {
      //  console.log('res',res)
        if (res.status === 200) return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response.status < 500 ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxputIND02', message: erroMsg, type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  alterarSituacaoAtendimento (form) {
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/AtendTeleMedicina/UpdateEmAtendimento`
    })
      .then(res => {
        if (res.status === 200) {
          if (form.emAtendimentoUsuario_Id) {
            Notification({ title: '', message: 'Atendimento iniciado com com Sucesso', type: 'success' })
          } else {
            Notification({ title: '', message: 'Atendimento finalizado com com Sucesso', type: 'success' })
          }
        }
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response.status < 500 ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxputIND02', message: erroMsg, type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  delete (id) {
    return axios({
      method: 'DELETE',
      url: `${$config.urlApi}/AtendTeleMedicina/Excluir/${id}`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Indivíduo excluido com sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxDELIND01', message: 'Erro ao Excluir Indivíduo', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  atualizarComorbidade(individuoId, form) {
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/IndividuoComorbidade/${individuoId}/Comorbidades`
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
