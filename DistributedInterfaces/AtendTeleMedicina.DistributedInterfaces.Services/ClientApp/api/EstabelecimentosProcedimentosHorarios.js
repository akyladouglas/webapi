import axios from 'axios'
import { Notification } from 'element-ui'
import $config from './config'

export default {
  get (filters) {
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/EstabelecimentosProcedimentosHorarios`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data.items, paginacao: { skip: res.data.skip, totalCount: res.data.totalCount }, status: res.status }
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxgetESTPH01', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  post (form) {
    return axios({
      method: 'POST',
      data: form,
      url: `${$config.urlApi}/EstabelecimentosProcedimentosHorarios`
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
        Notification({ title: 'Erro: ERRDBxpstESTPH01', message: erroMsg, type: 'error' })
        return { status: e.response.status, error: erroMsg }
      })
  }, 
}
