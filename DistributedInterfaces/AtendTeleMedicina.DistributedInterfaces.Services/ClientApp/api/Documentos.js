import axios from 'axios'
import { Notification } from 'element-ui'
import $config from './config'

export default {
  get(filters) {
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/Documento`
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
  getById(id) {
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/Documentos/${id}`
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
}
