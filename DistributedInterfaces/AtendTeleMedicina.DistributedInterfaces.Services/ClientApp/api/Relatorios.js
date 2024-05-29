import axios from 'axios'
import { Notification } from 'element-ui'
import $config from './config'

export default {
  listaRelatorioIndividuo (filters) {
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/Relatorio/ListaRelatorioIndividuos`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data.fichas, paginacao: { currentPage: res.data.page, totalCount: res.data.totalCount }, status: res.status }
      })
      .catch(e => {
        console.log('e', e)
        let erro = e.response ? e.response : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxpgetIND01', message: erro.data, type: 'error' })
        return { status: erro.status, error: erro.data }
      })
  },

  excelRelatorioIndividuo(filters) {
    axios({
      method: 'GET',
      url: `/api/Relatorio/ExcelRelatorioIndividuo`,
      responseType: 'blob'
    })
      .then((res) => {
        if (res.status === 200) {
          let blob = window.URL.createObjectURL(new Blob([res.data], { type: 'application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet' }))
          let link = document.createElement('a')
          link.href = blob
          link.setAttribute(`download`, `RelatorioIndividuos.xlsx`)
          document.body.appendChild(link)
          link.click()
        } else {
          console.log('sem registros para gerar relatório')
        }
      })
      .catch((e) => {
        console.log(e.response)
        this.erroMsg = 'Erro, tenta efetuar a operação novamente ou entre em contato com o suporte'
      })
  },

  //listaRelatorioAtendimentoDia (filters) {
  //  return axios({
  //    method: 'GET',
  //    params: filters,
  //    url: `${$config.urlApi}/Relatorio/ListaRelatorioAtendimentoDia`
  //  })
  //    .then(res => {
  //      if (res.status !== 200) return { status: res.status }
  //      return { data: res.data.fichas, paginacao: { currentPage: res.data.page, totalCount: res.data.totalCount }, status: res.status }
  //    })
  //    .catch(e => {
  //      console.log('e', e)
  //      let erro = e.response ? e.response : 'Erro de comunicação com a API'
  //      Notification({ title: 'Erro: ERRDBxpgetIND01', message: erro.data, type: 'error' })
  //      return { status: erro.status, error: erro.data }
  //    })
  //},

  //RELATORIO ATENDIMENTO MEDICO
  excelRelatorioAtendimento(filters) {
    axios({
      method: 'GET',
      url: `${$config.urlApi}/Relatorio/ExcelRelatorioAtendimento`,
      params: filters ,
      responseType: 'blob',
    })
      .then((res) => {
        console.log(res.status)
        console.log(res.data)
        if (res.status === 200) {
          let blob = window.URL.createObjectURL(new Blob([res.data], { type: 'application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet' }))
          let link = document.createElement('a')
          link.href = blob
          link.setAttribute(`download`, `RelatorioAtendimentosMedicosMeeds.xlsx`)
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

  //RELATORIO AUDITORIA
  excelRelatorioAuditoria(filters) {
    axios({
      method: 'GET',
      url: `${$config.urlApi}/Relatorio/ExcelRelatorioAuditoria`,
      params: filters,
      responseType: 'blob',
    })
      .then((res) => {
        console.log(res.status)
        console.log(res.data)
        if (res.status === 200) {
          let blob = window.URL.createObjectURL(new Blob([res.data], { type: 'application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet' }))
          let link = document.createElement('a')
          link.href = blob
          link.setAttribute(`download`, `RelatorioAuditoriaMeeds.xlsx`)
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

  

  //listaRelatorioAtendimentoPorMedico (filters) {
  //  return axios({
  //    method: 'GET',
  //    params: filters,
  //    url: `${$config.urlApi}/Relatorio/ListaRelatorioAtendimentoPorMedico`
  //  })
  //    .then(res => {
  //      if (res.status !== 200) return { status: res.status }
  //      return { data: res.data.fichas, paginacao: { currentPage: res.data.page, totalCount: res.data.totalCount }, status: res.status }
  //    })
  //    .catch(e => {
  //      console.log('e', e)
  //      let erro = e.response ? e.response : 'Erro de comunicação com a API'
  //      Notification({ title: 'Erro: ERRDBxpgetIND01', message: erro.data, type: 'error' })
  //      return { status: erro.status, error: erro.data }
  //    })
  //},

  //RELATORIO ATENDIMENTO PACIENTE
  excelRelatorioAtendimentoPaciente (filters) {
    axios({
      method: 'GET',
      url: `${$config.urlApi}/Relatorio/ExcelRelatorioAtendimentoPaciente`,
      params: filters,
      responseType: 'blob',
    })
      .then((res) => {
        if (res.status === 200) {
          let blob = window.URL.createObjectURL(new Blob([res.data], { type: 'application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet' }))
          let link = document.createElement('a')
          link.href = blob
          link.setAttribute(`download`, `RelatorioAtendimentoPacienteMeeds.xlsx`)
          document.body.appendChild(link)
          link.click()
        } else {
          console.log('sem registros para gerar relatório')
        }
      })
      .catch((e) => {
        console.log(e.response)
        this.erroMsg = 'Erro, tenta efetuar a operação novamente ou entre em contato com o suporte'
      })
  },


  listaRelatorioGrauDeRiscoUf (filters) {
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/Relatorio/ListaRelatorioGrauDeRiscoUf`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data.fichas, paginacao: { currentPage: res.data.page, totalCount: res.data.totalCount }, status: res.status }
      })
      .catch(e => {
        console.log('e', e)
        let erro = e.response ? e.response : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxpgetIND01', message: erro.data, type: 'error' })
        return { status: erro.status, error: erro.data }
      })
  },

  excelRelatorioGrauDeRiscoUf (filters) {
    axios({
      method: 'GET',
      url: `/api/Relatorio/ExcelRelatorioGrauDeRiscoUf`,
      params: filters,
      responseType: 'blob'
    })
      .then((res) => {
        if (res.status === 200) {
          let blob = window.URL.createObjectURL(new Blob([res.data], { type: 'application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet' }))
          let link = document.createElement('a')
          link.href = blob
          link.setAttribute(`download`, `RelatorioGrauDeRiscoUf.xlsx`)
          document.body.appendChild(link)
          link.click()
        } else {
          console.log('sem registros para gerar relatório')
        }
      })
      .catch((e) => {
        console.log(e.response)
        this.erroMsg = 'Erro, tenta efetuar a operação novamente ou entre em contato com o suporte'
      })
  },

  listaRelatorioCadastrosUf (filters) {
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/Relatorio/ListaRelatorioCadastrosUf`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data.fichas, paginacao: { currentPage: res.data.page, totalCount: res.data.totalCount }, status: res.status }
      })
      .catch(e => {
        console.log('e', e)
        let erro = e.response ? e.response : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxpgetIND01', message: erro.data, type: 'error' })
        return { status: erro.status, error: erro.data }
      })
  },

  excelRelatorioCadastrosUf (filters) {
    axios({
      method: 'GET',
      url: `/api/Relatorio/ExcelRelatorioCadastrosUf`,
      params: filters,
      responseType: 'blob'
    })
      .then((res) => {
        if (res.status === 200) {
          let blob = window.URL.createObjectURL(new Blob([res.data], { type: 'application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet' }))
          let link = document.createElement('a')
          link.href = blob
          link.setAttribute(`download`, `RelatorioCadastrosUf.xlsx`)
          document.body.appendChild(link)
          link.click()
        } else {
          console.log('sem registros para gerar relatório')
        }
      })
      .catch((e) => {
        console.log(e.response)
        this.erroMsg = 'Erro, tenta efetuar a operação novamente ou entre em contato com o suporte'
      })
  },
  listaRelatorioTotalCadastros (filters) {
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/Relatorio/ListaRelatorioTotalCadastros`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data.fichas, paginacao: { currentPage: res.data.page, totalCount: res.data.totalCount }, status: res.status }
      })
      .catch(e => {
        console.log('e', e)
        let erro = e.response ? e.response : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxpgetIND01', message: erro.data, type: 'error' })
        return { status: erro.status, error: erro.data }
      })
  },


  // RELATÓRIO DE GLICEMIAS
  excelRelatorioGlicemia(filters) {
    axios({
      method: 'GET',
      url: `${$config.urlApi}/Relatorio/ExcelRelatorioGlicemia`,
      params: filters,
      responseType: 'blob',
    })
      .then((res) => {
        if (res.status === 200) {
          let blob = window.URL.createObjectURL(new Blob([res.data], { type: 'application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet' }))
          let link = document.createElement('a')
          link.href = blob
          link.setAttribute(`download`, `RelatorioGlicemiaMeeds.xlsx`)
          document.body.appendChild(link)
          link.click()
          return { status: res.status }
        } else {
          console.log('sem registros para gerar relatório')
        }

      })
      .catch((e) => {
        console.log(e)
        this.erroMsg = 'Erro, tente efetuar a operação novamente ou entre em contato com o suporte'
      })
  }
}
