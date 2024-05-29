import axios from 'axios'
import { Notification } from 'element-ui'
import $config from './config'

export default {
  get () {
    return axios({
      method: 'GET',
      url: `${$config.urlApi}/Config`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        let erro = e.response
        if (e.response.status === 500) erro = 'Erro interno no servidor'
        if (e.response.status !== 500) erro = e.response.data
        Notification({ title: 'Erro: ERRDBxpgetIND01', message: erro, type: 'error' })
        return { status: e.response.status, error: erro }
      })
  },
  put(form) {
    if (form.estadosLiberados) {
      form = {
        ...form,
        estadosLiberados: form.estadosLiberados.join()
      }
    }
    
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Config`
    })
      .then(res => {
        Notification({ title: '', message: 'Operação Realizada com Sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        let erro = e.response
        if (e.response.status === 500) erro = 'Erro interno no servidor'
        if (e.response.status !== 500) erro = e.response.data
        Notification({ title: 'Erro: ERRO-DB-PUT-CONFIG', message: erro, type: 'error' })
        return { status: e.response.status, error: erro }
      })
  }
}
