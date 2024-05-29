import axios from 'axios'
import { Notification } from 'element-ui'
import $config from './config'
import moment from 'moment'
moment.locale('pt-br')

export default {
  getExamesF200(filters) {
    // console.log('filters', filters)
    // console.log("AAAAA")
    filters = {
      ...filters
    }
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/ExamesF200/GetExamesF200/`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        // console.log(res)
        return { data: res.data.items, paginacao: { currentPage: res.data.skip, totalCount: res.data.totalCount }, status: res.status }
        // console.log(res)
      })
      .catch(e => {
        console.log('errogetExames F200', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: Não foi possível obter os Exames ECOF200', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  postExamesF200(cpf) {
    //console.log('cpf', cpf)
    let form = {
      cpf: cpf
    }
    return axios({
      method: 'POST',
      data: form,
      url: `${$config.urlApi}/ExamesF200/ReceberExamesF200`
    }).then(res => {
      if (res.status !== 200) return { status: res.status }
      Notification({ title: 'Sucesso ', message: 'Exames salvos com sucesso', type: 'success' })
      return { status: res.status }
    })
      .catch(e => {
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        console.log('Erro receber F200', e.response)
        Notification({ title: 'Erro: ERRDBxpostPROCF200', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  }
}
