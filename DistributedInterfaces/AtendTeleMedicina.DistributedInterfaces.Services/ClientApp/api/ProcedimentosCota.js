import axios from 'axios'
import { Notification } from 'element-ui'
import $config from './config'

export default {
  put (procedimentoId, quantidade) {
    return axios({
      method: 'PUT',
      url: `${$config.urlApi}/Procedimento/${procedimentoId}/Cota`,
      params: {
        quantidade
      }
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Cota adicionada com Sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxputPROC01', message: 'Erro ao adicionar a Cota', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  }
}
