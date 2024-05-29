import axios from 'axios'
import moment from 'moment'
import { Notification } from 'element-ui'
import $config from '../../config'

export default {
  filters: {
    codigo: Number,
    nome: String,
    profissional: Number,
    currentPage: Number,
    pageSize: Number
  },
  getByParams (filters) {
   // console.log('filtros', filters)
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/Acessos`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, paginacao: JSON.parse(res.headers.pagination), status: res.status }
      })
      .catch((e) => {
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxgetPROFNAL01', message: erroMsg, type: 'error' })
        return { error: erroMsg }
      })
  },
  post (form) {
    return axios({
      method: 'POST',
      data: form,
      url: `${$config.urlApi}/Acessos`
    })
      .then(res => {
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxpstPROFNAL01', message: erroMsg, type: 'error' })
        return { error: erroMsg }
      })
  },
  put (form) {
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Acessos/${form.codigo}`
    })
      .then(res => {
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxputPROFNAL01', message: erroMsg, type: 'error' })
        return { error: erroMsg }
      })
  }
}
