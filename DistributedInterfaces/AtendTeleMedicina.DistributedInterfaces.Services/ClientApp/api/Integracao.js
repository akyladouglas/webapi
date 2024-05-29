import axios from 'axios'
import { Notification } from 'element-ui'
import $config from './config'

export default {
  listarLotes (filters) {
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/Integracao/ListarLote`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data.items, paginacao: { currentPage: res.data.skip, totalCount: res.data.totalCount }, status: res.status }
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxgetLOT01', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  gerarLote (filters) {
    return axios({
      method: 'POST',
      params: filters,
      url: `${$config.urlApi}/Integracao/GerarLote`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data, status: res.status }
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxPSTLOT01', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },
  getRelatorioErros (filters) {
    axios({
      method: 'GET',
      url: `${$config.urlApi}/Integracao/RelatorioErros`,
      params: filters,
      responseType: 'blob'
    })
      .then((res) => {
        let fileName = res.headers['content-disposition'].split('; ')[1].replace('filename=', '')
        if (res.status === 200) {
          let blob = window.URL.createObjectURL(new Blob([res.data], { type: 'application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet' }))
          let link = document.createElement('a')
          link.href = blob
          link.setAttribute(`download`, `${fileName}`)
          document.body.appendChild(link)
          link.click()
          return { status: res.status }
        } else {
          console.log('sem registros para gerar relatório')
        }
      })
      .catch((e) => {
        console.log(e)
        this.erroMsg = 'Erro, tenta efetuar a operação novamente ou entre em contato com o suporte'
      })
  },
  getRelatorioFichasGeradas (filters) {
    axios({
      method: 'GET',
      url: `${$config.urlApi}/Integracao/RelatorioFichasGeradas`,
      params: filters,
      responseType: 'blob'
    })
      .then((res) => {
        let fileName = res.headers['content-disposition'].split('; ')[1].replace('filename=', '')
        if (res.status === 200) {
          let blob = window.URL.createObjectURL(new Blob([res.data], { type: 'application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet' }))
          let link = document.createElement('a')
          link.href = blob
          link.setAttribute(`download`, `${fileName}`)
          document.body.appendChild(link)
          link.click()
          return { status: res.status }
        } else {
          console.log('sem registros para gerar relatório')
        }
      })
      .catch((e) => {
        console.log(e)
        this.erroMsg = 'Erro, tenta efetuar a operação novamente ou entre em contato com o suporte'
      })
  }
}
