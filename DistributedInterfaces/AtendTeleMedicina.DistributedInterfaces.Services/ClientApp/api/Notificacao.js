import axios from 'axios'
import { Notification } from 'element-ui'
import $config from './config'

export default {

get(filters) {
    //console.log("filters",filters)
  return axios({
    method: 'GET',
    data: filters,
    url: `${$config.urlApi}/Notificacao`
  })
    .then(res => {
      if (res.status !== 200) return { status: res.status }
      //console.log(res)
      return { data: res.data.items, paginacao: { currentPage: res.data.skip, totalCount: res.data.totalCount }, status: res.status }
     // console.log(res)
    })
    .catch(e => {
      let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
      Notification({ title: 'Erro: ERRDBxpostNOTIFIC', message: erroMsg, type: 'error' })
      return { status: 400, error: erroMsg }
    })
},
  post(arr) {
  return axios({
    method: 'POST',
    data: arr,
    url: `${$config.urlApi}/notificacao`,
  })
    .then(res => {
      if (res.status !== 200) return { status: res.status }
      return { data: res.data, status: res.status }
    })
    .catch(e => {
      let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
      console.log('e.response', e.response)
      Notification({ title: 'Erro: ERRDBxpostNOTIFIC', message: erroMsg, type: 'error' })
      return { status: 400, error: erroMsg }
    })
  },
  put(form) {
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/notificacao/${form.id}`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Notificação excluida com sucesso!', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro'
        Notification({ title: erroMsg, message: '', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },
}
