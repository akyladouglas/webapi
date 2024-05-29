import axios from 'axios'
import { Notification } from 'element-ui'

export default {
  getCep (cep) {
    cep = cep.replace(/[.-\s]/g, '')
    //console.log('cep', cep)
    return fetch(`https://viacep.com.br/ws/${cep}/json/`, {
      method: 'get',
      headers: new Headers({ 'Content-Type': 'application/json; charset=utf-8' })
    }).then((response) => response.json()).then(res => {
      if (res.erro) {
        Notification({ message: 'CEP não encontrado', type: 'error' })
        return { status: 400 }
      }
      return { data: res, status: 200 }
    }).catch(e => {
      let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
      Notification({ title: 'Erro', message: 'CEP não encontrado', type: 'error' })
      return { data: erroMsg, status: e.response.status }
    })
  },

  // Chamada da API MEMED
  getMemed (form) {
      return axios({
        method: 'GET',
        url: `https://integrations.api.memed.com.br/v1/prescricoes/`,
      params: { 'token': 'eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.WzQwNDM0LCIzZTM4MmVmYWNkNzk5YTdjN2I4ZmJiMTk5ZWJjNDgzYyIsIjIwMjItMDQtMDEiLCJzaW5hcHNlLnByZXNjcmljYW8iLCJwYXJ0bmVyLjMuMzU2ODkiXQ.5wZ7KyGxJZnU08PZ6FRAGvTVOEdx8o2OLuyhpXR7hfo' }

      }).then(res => {
             if (res.status !== 200) return { status: res.status }
             return { data: res.data, status: res.status }
          })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxGETCEP01', message: 'API MEMED NÃO ENCONTRADA', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
  
  // fim Chamada da API MEMED

  // Cadastro de Medicos Memed
  
  async getLatLng (endereco) {
    return fetch(`https://maps.googleapis.com/maps/api/geocode/json?address=${endereco}&key=AIzaSyCNZxZkkyu0HSu6v5TZLCxuDtZuPJEqCCQ`, {
      method: 'GET'
    }).then((response) => response.json()).then(res => {
      return { data: res.results[0].geometry.location, status: 200 }   
    }).catch(e => {
      console.log('erro', e)
      let erroMsg = 'Não foi possivel recuperar as coordenadas'
      Notification({ title: 'Erro ERRDBxGETLATLNG01', message: erroMsg, type: 'error' })
      return { data: erroMsg, status: 400 }
    })
  },
  async getPacienteAtend (params) {
    return axios({
      method: 'GET',
      url: `${params.urlAtend}/api/CadastroIndividual/GetByCnsCpf?cpf=${params.cpf}&cns=${params.cns}`
    }).then(res => {
      return { data: res.data, status: res.status }
    })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxGETATEND01', message: 'Falha de comunicação com o Atend Saude', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  }
}
