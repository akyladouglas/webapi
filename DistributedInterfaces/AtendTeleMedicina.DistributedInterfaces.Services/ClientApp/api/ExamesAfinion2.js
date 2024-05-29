import axios from 'axios'
import { Notification } from 'element-ui'
import $config from './config'
import moment from 'moment'
moment.locale('pt-br')

export default {
  getExamesAfinion2(filters) {
    filters = {
      ...filters
    }
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/ExamesAfinion2/GetExamesAfinion2/`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        // console.log(res)
        return { data: res.data.items, paginacao: { currentPage: res.data.skip, totalCount: res.data.totalCount }, status: res.status }
        // console.log(res)
      })
      .catch(e => {
        console.log('errogetExames Afinion2', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: Não foi possível obter os Exames Afinion2', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  //postExamesF200(cpf) {
  //  //console.log('cpf', cpf)
  //  let form = {
  //    cpf: cpf
  //  }
  //  return axios({
  //    method: 'POST',
  //    data: form,
  //    url: `${$config.urlApi}/ExamesF200/ReceberExamesF200`
  //  }).then(res => {
  //    if (res.status !== 200) return { status: res.status }
  //    Notification({ title: 'Sucesso ', message: 'Exames salvos com sucesso', type: 'success' })
  //    return { status: res.status }
  //  })
  //    .catch(e => {
  //      let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
  //      console.log('Erro receber F200', e.response)
  //      Notification({ title: 'Erro: ERRDBxpostPROCF200', message: erroMsg, type: 'error' })
  //      return { status: 400, error: erroMsg }
  //    })
  //}
}
