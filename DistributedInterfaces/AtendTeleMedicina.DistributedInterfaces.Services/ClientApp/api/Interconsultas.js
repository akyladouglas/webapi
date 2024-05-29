import axios from 'axios'
import { Notification } from 'element-ui'
import $config from './config'

export default {
  get(filters) {
    return axios({
      method: 'GET',
      params: filters,
      url: `${$config.urlApi}/Interconsulta`
    })
      .then(res => {
        if (res.status !== 200) return { status: res.status }
        return { data: res.data.items, paginacao: { skip: res.data.skip, totalCount: res.data.totalCount }, status: res.status }
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro: ERRDBxgetInter01', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },

  post(form) {
    return axios({
      method: 'POST',
      data: form,
      url: `${$config.urlApi}/Interconsulta`
    })
      .then(res => {
        Notification({ title: '', message: 'Agendamento da Interconsulta Realizado com Sucesso', type: 'success' })
        return res
      })
      .catch(e => {
        console.log('e', e)
        let erroMsg = e.response
        if (e.response.status >= 500) erroMsg = 'Erro interno no servidor'
        if (e.response.status < 500) erroMsg = e.response.data
        Notification({ title: 'Erro: ERRDBxpostInter01', message: erroMsg, type: 'error' })
        return { status: e.response.status, error: erroMsg }
      })
  },

  aceiteInterconsulta(form) {
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Interconsulta/AceiteInterconsulta`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Presença alterada da interconsulta', type: 'success' })
        console.log(res)
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxputInter01', message: 'Erro ao confirmar a presença', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },

  putAvaliacaoMedico(form) {
    console.log('entrou na api interconsultas', form)
    return axios({
      method: 'PUT',
      data: form,
      url: `${$config.urlApi}/Interconsulta/AvaliacaoMedico/${form.id}`
    })
      .then(res => {
        if (res.status === 200) Notification({ title: '', message: 'Avaliação realizada com sucesso', type: 'success' })
        console.log(res)
        return res
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ERRDBxputInter01', message: 'Erro ao avaliar', type: 'error' })
        return { data: erroMsg, status: e.response.status }
      })
  },


  notifyWhatsapp(nomeCompleto, telefoneCelular) {
    let url = 'http://api.gupshup.io/sm/api/v1/template/msg';
    let apiKey = 'aashkn3ra03xb6deyzzdjszbeiijmach';
    let source = '558396220468';
    let destination = `55${telefoneCelular}`;

    let template = JSON.stringify({
      id: 'aed409e5-44af-4245-b168-175abc363e6f',
      params: [`${nomeCompleto}`]
    });

    let data = new URLSearchParams();
    data.append('source', source);
    data.append('destination', destination);
    data.append('template', template);


    axios.post(url, data, {
      headers: {
        'apikey': apiKey,
        'Content-Type': 'application/x-www-form-urlencoded'
      }
    })
      .then(response => {
        //console.log('Response:', response.data);
        return response;
      })
      .catch(error => {
        //console.error('Error:', error);
        return error
      });
  },

}
