import axios from 'axios'
import { Notification } from 'element-ui'
import $config from './config'

export default {
  get (filters) {
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/Pais`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data.items, status: res.status }
      })
      .catch(e => {
        console.log('e', e)
        let erro = e.response ? e.response : 'Erro de comunicação com a API'
        Notification({ title: 'Erro no retorno dos paises', message: erro.data, type: 'error' })
        return { status: erro.status, error: erro.data }
      })
  }

}
