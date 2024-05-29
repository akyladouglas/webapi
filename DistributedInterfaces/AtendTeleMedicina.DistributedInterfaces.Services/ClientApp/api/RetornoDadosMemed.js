import axios from 'axios'
import { Notification } from 'element-ui'

export default {

  // Chamada dos Dados do Prontuario da API MEMED
 
  RetornoDadosMemed(form) {
    return fetch(`https://integrations.api.memed.com.br/v1/prescricoes/?token=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.WzQwNDM0LCIzZTM4MmVmYWNkNzk5YTdjN2I4ZmJiMTk5ZWJjNDgzYyIsIjIwMjItMDQtMDEiLCJzaW5hcHNlLnByZXNjcmljYW8iLCJwYXJ0bmVyLjMuMzU2ODkiXQ.5wZ7KyGxJZnU08PZ6FRAGvTVOEdx8o2OLuyhpXR7hfo'`, {
      method: 'get',
      params: { 'token': "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.WzQwNDM0LCIzZTM4MmVmYWNkNzk5YTdjN2I4ZmJiMTk5ZWJjNDgzYyIsIjIwMjItMDQtMDEiLCJzaW5hcHNlLnByZXNjcmljYW8iLCJwYXJ0bmVyLjMuMzU2ODkiXQ.5wZ7KyGxJZnU08PZ6FRAGvTVOEdx8o2OLuyhpXR7hfo" },
      headers: new Headers({ 'Content-Type': 'application/json; charset=utf-8' })
    }).then((response) => response.json()).then(res => {
      if (res.erro) {
        Notification({ message: 'DADOS NÂO RETORNADOS', type: 'error' })
        return { status: 400 }
      }
      return { data: res, status: 200 }
    }).catch(e => {
      let erroMsg = e.response ? e.res.data : 'ERRO NO RETORNO DE DADOS DA API'
      Notification({ title: 'Erro ERRDBxGETDADOS01', message: 'DADOS NÃO RETORNADOS', type: 'error' })
      return { data: erro.Ms, status: e.response.status }
    })
   },

}
