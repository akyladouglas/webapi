import axios from 'axios'
import { Notification } from 'element-ui'
import $config from './config'

export default {
  get (filters) {
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/UnidadeFederativa`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data.items, paginacao: { currentPage: res.data.skip, totalCount: res.data.totalCount }, status: res.status }
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response.data ? e.response.data : 'Erro de comunicação com a API'
        erroMsg = ''
        Notification({ title: 'Erro: ERRDBxgetUF01', message: erroMsg, type: 'error' })
        return { error: erroMsg }
      })
  },
  getById (id) {
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/UnidadeFederativa/${id}`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response.data ? e.response.data : 'Erro de comunicação com a API'
        erroMsg = ''
        Notification({ title: 'Erro: ERRDBxgetUF02', message: erroMsg, type: 'error' })
        return { error: erroMsg }
      })
  }
}
