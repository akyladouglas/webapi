import _ from 'lodash'
import moment from 'moment'
import { mapGetters } from 'vuex'

export default {
  data () {
    return {}
  },
  computed: {
    ...mapGetters([
      'roles',
      'claims',
      'demandaEspontanea',
      'selectRole',
    ])
  },
  methods: {
    mxGetConvenioById (array, id) {
      if (!id) return ''
      let convenio = _.find(array, {codigo: id})
      return convenio.descricao
    },
    mxGetConveniosById (array, ids) {
      // a função espera um array, para retornar o titulo de um convenio específico passar o valor em array  ex: [5]
      if (!ids) return 'n/a'
      let convenios = _.filter(array, (convenio) => _.includes(ids, convenio.codigo))
      let desc = []
      convenios.forEach(convenio => {
        desc.push(convenio.descricao)
      })
      return desc.join(', ')
    },
    mxHasExact (recurso) {
      // var usuarioRecursos =  userRoles.split(',') // convertendo para array de string, aqui no caso user().roles já retorna o array de strings
      // A verificação: <li v-if="hasExact('PaisListar')">
      return this.roles.indexOf(recurso) !== -1
    },
    mxHasAny (...recurso) {
      // Para exibição de menu com submenus em caso do usuário possuir pelo menos uma permissão contida no menu
      // Usar em conjunto com hasExact para os submenus
      // <ul v-if="hasAny('Usuario','Perfil')">
      // <li v-if="hasExact('UsuarioListar')">
      var permitido = false
      recurso.forEach(item => {
        var patt = new RegExp(`\\b${item}\\b`, 'g');
        var result = patt.test(this.roles)
        if (result === true) permitido = true
      })
      return permitido
    },

    // o parametro será um array de Strings com a role / userclaim enviada
    mxHasAccess(...arrayString) {
      var permitido = false
      arrayString.forEach(item => {
        /*var patt = new RegExp(item, 'g')*/
        var patt = new RegExp(`\\b${item}\\b`, 'g');

        var result = patt.test(this.selectRole)
        if (result === true) permitido = true
      })
      return permitido
    },

    mxConverterData (string) {
      let dtUltMensagem = this.moment(string).format('DD/MM/YYYY')
      return dtUltMensagem
    },
    mxUltDtFromArray (array) {
      // verifica se ultima data mais recente do array é igual a data de hoje
      let m0 = new Date(Math.max.apply(null, array.map(function (e) {
        return new Date(e.dataCadastro)
      })))
      let dtUltMensagem = this.moment(m0).format('DD/MM/YYYY')
      let hoje = this.moment().format('DD/MM/YYYY')
      return dtUltMensagem === hoje
    },
    isDemandaEspontanea () {
      return this.demandaEspontanea
    },
    inRole (...role) {
      // console.log('roles', ...role)
      if (_.indexOf(role, this.roles) >= 0) {
        return true
      } else {
        return false
      }
    },
    inArray (item, array) {
      if (array.indexOf(item) >= 0) {
        return true
      } else {
        return false
      }
    },
    removeDoArray (array, item) {
      const index = array.indexOf(item)
      array.splice(index, 1)
    },
    mxImgToBytes (url) {
      var byteArray = []
      var req = new XMLHttpRequest()
      req.open('GET', url, false)
      req.overrideMimeType('text\/plain; charset=x-user-defined')
      req.send(null)
      if (req.status !== 200) return byteArray
      for (var i = 0; i < req.responseText.length; ++i) {
        byteArray.push(req.responseText.charCodeAt(i) & 0xff)
      }
      return byteArray
    },
    mxGerarGuid () {
      return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        let r = Math.random() * 16 | 0
        let v = c === 'x' ? r : (r & 0x3 | 0x8)
        return v.toString(16)
      })
    },
    mxProntuario (val) {
      var protocolo = Math.floor(new Date().getTime())
      return `MD${protocolo.toString().slice(-val)}`
    },
    geraProtocolo (tipo) {
      let protocolo
      if (!tipo) tipo = ''
      protocolo = moment().format('YYYYMMDDhhmm').concat(new Date().getMilliseconds() + tipo)
      return protocolo
    },

    // VALIDAÇÕES

    mxValidaNome(value) {
      if (value.match(/^[A-Za-záàâãéèêíïóôõöúçñÁÀÂÃÉÈÊÍÏÓÔÕÖÚÇÑ´^ ]+$/)) {
        return true
      } else {
        return false
      }
    },
    mxValidaNomeUsuarios (value) {
      if ((value.match(/^[A-Za-záàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ]+$/))) {
        return true
      } else {
        return false
      }
    },

    mxValidaComplemento (value) {
      if ((value.match(/^[A-Za-záàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ0-9 ]+$/))) {
        return true
      } else {
        return false
      }
    },
    mxValidaCRM (value) {
      if ((value.match(/^[A-Za-záàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ0-9/ ]+$/))) {
        return true
      } else {
        return false
      }
    },
    mxValidaCnpj (value) {
      if ((value.match(/^[0-9/.-]*$/))) {
        return true
      } else {
        return false
      }
    },
    mxValidaUsernameNumerico (value) {
      if ((value.match(/^[0-9]*$/))) {
        return true
      } else {
        return false
      }
    },

    mxValidaNumero (value) {
      if ((value.match(/^[0-9]*$/))) {
        return true
      } else {
        return false
      }
    },
    mxValidaCnes (value) {
      if ((value.match(/^[0-9.-/]*$/))) {
        return true
      } else {
        return false
      }
    },

    mxValidaCodigo (value) {
      if ((value.match(/^[A-Za-záàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ0-9]*$/))) {
        return true
      } else {
        return false
      }
    },

    mxValidaDescricao (value) {
      if ((value.match(/^[A-Za-záàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ0-9/+()-ªº {2}]*$/))) {
        return true
      } else {
        return false
      }
    },

    mxValidaSenha (value) {
      if ((value.match(/^[A-Za-záàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ0-9 ]+$/))) {
        return true
      } else {
        return false
      }
    },

    mxValidaCPF (cpf) {
      cpf = cpf.replace(/[.-\s]/g, '')
      var numeros, digitos, soma, i, resultado, digitosIguais
      digitosIguais = 1
      if (cpf.length < 11) { return false }
      for (i = 0; i < cpf.length - 1; i++) {
        if (cpf.charAt(i) != cpf.charAt(i + 1)) {
          digitosIguais = 0
          break
        }
      }
      if (!digitosIguais) {
        numeros = cpf.substring(0, 9)
        digitos = cpf.substring(9)
        soma = 0
        for (i = 10; i > 1; i--) { soma += numeros.charAt(10 - i) * i }
        resultado = soma % 11 < 2 ? 0 : 11 - soma % 11
        if (resultado != digitos.charAt(0)) { return false }
        numeros = cpf.substring(0, 10)
        soma = 0
        for (i = 11; i > 1; i--) { soma += numeros.charAt(11 - i) * i }
        resultado = soma % 11 < 2 ? 0 : 11 - soma % 11
        if (resultado != digitos.charAt(1)) { return false }
        return true
      } else { return false }
    },

    // VALIDACOES FORM CADASTRO INDIVIDUO (RECEPCAO)
    mxValidaCpfIndividuo (rule, value, callback) {
      if (value === undefined || value.trim() === '') return callback(new Error('O campo CPF é obrigatório'))

      if (value != undefined) {
        if (value != '' || value != null) {
          // console.log("value", value)
          var cpf = value.replace(/[^\d]+/g, '') // Remove caracteres não numéricos

          if (cpf.length === 0) return callback()
          if (cpf.length < 10) return callback(new Error('O CPF deve conter 11 dígitos'))

          // Verifica se todos os dígitos são iguais (CPF inválido)
          if (/^(\d)\1+$/.test(cpf)) return callback(new Error('CPF não é inválido'))

          let sum = 0
          let remainder

          // Calcula o primeiro dígito verificador
          for (let i = 1; i <= 9; i++) {
            sum += parseInt(cpf.charAt(i - 1)) * (11 - i)
          }

          remainder = (sum * 10) % 11

          if (remainder === 10 || remainder === 11) {
            remainder = 0
          }

          if (remainder !== parseInt(cpf.charAt(9))) return callback(new Error('CPF não é inválido'))

          sum = 0

          // Calcula o segundo dígito verificador
          for (let i = 1; i <= 10; i++) {
            sum += parseInt(cpf.charAt(i - 1)) * (12 - i)
          }

          remainder = (sum * 10) % 11

          if (remainder === 10 || remainder === 11) {
            remainder = 0
          }

          if (remainder !== parseInt(cpf.charAt(10))) return callback(new Error('CPF não é inválido'))

          // CPF válido
          return callback()
        } else {
          return callback()
        }
      } else {
        return callback()
      }
    },

    // VALIDAÇÕES INDIVIDUO

    mxValidaNomeIndividuo (rule, value, callback) {
      if (value === undefined || value.trim() === '') return callback(new Error('O campo nome é obrigatório'))
      if (value.length < 9) return callback(new Error('O campo nome deve conter mais que 9 caracteres'))
      if (value.match(/^[a-zA-Z0-9À-ÿºª\s' -]+$/)) return callback()
      else return callback(new Error('O campo nome é inválido'))
    },

    mxValidaPisPasep (rule, value, callback) {
      if (!value) return callback()
      // if (value === undefined || value.trim() === "") return callback(new Error('O campo PisPasep é obrigatório'))
      if (value.length < 9) return callback(new Error('O campo PisPasep deve conter mais que 9 caracteres'))
      if (value.match(/^[0-9.-]+$/)) return callback()
      else return callback(new Error('O campo PisPasep é inválido'))
    },

    mxValidaNomeDaMaeIndividuo (rule, value, callback) {
      if (value === undefined || value.trim() === '') return callback(new Error('O campo nome da mãe é obrigatório'))
      if (value.length < 9) return callback(new Error('O campo nome da mãe deve conter mais que 9 caracteres'))
      if (value.match(/^[a-zA-Z0-9À-ÿºª\s' -]+$/)) return callback()
      else return callback(new Error('O campo nome da mãe é inválido'))
    },
    mxValidaNomeDoPaiIndividuo (rule, value, callback) {
      if (value === undefined || value.trim() === '') return callback(new Error('O campo nome do pai é obrigatório'))
      if (value.length < 9) return callback(new Error('O campo nome do pai deve conter mais que 9 caracteres'))
      if (value.match(/^[a-zA-Z0-9À-ÿºª\s' -]+$/)) return callback()
      else return callback(new Error('O campo nome do pai é inválido'))
    },

    mxValidaCnsIndividuo (rule, value, callback) {
      // if (value === undefined || value.trim() === "") return callback(new Error('O campo cns é obrigatório'))
      if (value != undefined) {
        if (value != '' || value != null) {
          if (value.length === 0) return callback()
          if (value.length < 14) return callback(new Error('Número CNS inválido. O campo deve conter 15(quinze) dígitos'))
          if (value.match(/^[0-9]*$/)) return callback()
          else return callback(new Error('O campo cns é inválido'))
        } else {
          return callback()
        }
      } else {
        return callback()
      }
    },

    mxValidaSenhaIndividuo (rule, value, callback) {
      if (value === undefined || value.trim() === '') return callback(new Error('O campo senha é obrigatório'))
      if (value.match(/^(?=.*[0-9])(?=.*[a-z]).{8,}/)) return callback()
      else return callback(new Error('A senha deve conter ao menos 1 letra e 1 número e conter no mínimo 8 caracteres.'))
    },

    mxValidaCelularIndividuo (rule, value, callback) {
      if (value === undefined || value.trim() === '') return callback(new Error('O campo celular é obrigatório'))
      value = value.replace(/[^\d]+/g, '')
      if (value.length !== 11) return callback(new Error('O campo celular deve conter exatamente 11 dígitos'))
      if (value.match(/^[1-9]{2}9?[6789]\d{7}$/)) return callback()
      else return callback(new Error('O campo telefone é inválido'))
    },

    mxValidaEmailIndividuo (rule, value, callback) {
      if (value === undefined || value.trim() === '') return callback(new Error('O campo email é obrigatório'))
      if (value.length < 10) return callback(new Error('O campo não pode ter menos que 15 caracteres'))
      if (value.match(/^[a-z*0-9]([\.-]?\w+)*([a-z0-9])@\w+([\.-]?\w+)*(\.\w{2,3})+$/)) return callback()
      else return callback(new Error('O campo email é inválido'))
    },

    mxValidaDataNascimentoIndividuo (rule, value, callback) {
      if (value === undefined || value == null || value == '') return callback(new Error('O campo data do nascimento é obrigatório'))

      const now = moment().format('YYYY/MM/DDThh:mm:ss')
      var splitNow = now.split('/')
      var dataNow = `${splitNow[0]}-${splitNow[1]}-${splitNow[2]}`
      var newValue = moment(value).format('YYYY/MM/DDThh:mm:ss')
      var splitValue = newValue.split('/')
      var newDataValue = `${splitValue[0]}-${splitValue[1]}-${splitValue[2]}`

      if (newDataValue > '1900-01-01T00:00:00' && newDataValue < dataNow) return callback()
      else return callback(new Error('A Data De Nascimento não pode ser maior que o dia de hoje'))
    },


    mxGetGrauImc(row) {
      if (row <= 16.9) return 'Muito abaixo do Peso'
      else if (row >= 17 && row <= 18.4) return 'Abaixo do Peso'
      else if (row >= 18.5 && row <= 24.9) return 'Peso Normal'
      else if (row >= 25 && row <= 29.9) return 'Acima do Peso'
      else if (row >= 30 && row <= 34.9) return 'Obesidade Grau I'
      else if (row >= 35 && row <= 40) return 'Obesidade Grau II'
      else if (row > 40) return 'Obesidade Grau III'
      else return ''
    },





    //VALIDAÇÕES USUARIO

    mxValidaNomeUsuario(rule, value, callback) {
      if (value === undefined || value.trim() === '') return callback(new Error('Campo obrigatório'))
      if (value.length < 9) return callback(new Error('O campo nome deve conter no mínimo 10 caracteres'))
      if (value.length > 50) return callback(new Error('O campo nome deve conter no máximo 50 caracteres'))
      if (value.match(/^[a-zA-Z0-9À-ÿºª\s' -]+$/)) return callback()
      else return callback(new Error('O campo nome é inválido'))
    },

    mxValidaUsernameUsuario(rule, value, callback) {
      if (value === undefined || value.trim() === '' || value === null) return callback(new Error('Campo obrigatório'))
      if (value.length < 9) return callback(new Error('O campo username deve conter no mínimo 10 caracteres'))
      if (value.length > 20) return callback(new Error('O campo username deve conter no máximo 20 caracteres'))
      if (value.match(/^[a-zA-Z0-9À-ÿºª\s' -]+$/)) return callback()
      else return callback(new Error('O campo username é inválido'))
    },

    mxValidaSenhaUsuario(rule, value, callback) {
      if (value === undefined || value.trim() === '') return callback(new Error('Campo obrigatório'))
      if (value.match(/^(?=.*[0-9])(?=.*[a-z]).{8,}/)) return callback()
      else return callback(new Error('A senha deve conter ao menos 1 letra e 1 número e conter no mínimo 8 caracteres.'))
    },

    mxValidaCpfUsuario(rule, value, callback) {
      if (value === undefined || value.trim() === '') return callback(new Error('Campo obrigatório'))

      if (value != undefined) {
        if (value != '' || value != null) {
          // console.log("value", value)
          var cpf = value.replace(/[^\d]+/g, '') // Remove caracteres não numéricos

          if (cpf.length === 0) return callback()
          if (cpf.length < 10) return callback(new Error('O CPF deve conter 11 dígitos'))

          // Verifica se todos os dígitos são iguais (CPF inválido)
          if (/^(\d)\1+$/.test(cpf)) return callback(new Error('CPF não é inválido'))

          let sum = 0
          let remainder

          // Calcula o primeiro dígito verificador
          for (let i = 1; i <= 9; i++) {
            sum += parseInt(cpf.charAt(i - 1)) * (11 - i)
          }

          remainder = (sum * 10) % 11

          if (remainder === 10 || remainder === 11) {
            remainder = 0
          }

          if (remainder !== parseInt(cpf.charAt(9))) return callback(new Error('CPF não é inválido'))

          sum = 0

          // Calcula o segundo dígito verificador
          for (let i = 1; i <= 10; i++) {
            sum += parseInt(cpf.charAt(i - 1)) * (12 - i)
          }

          remainder = (sum * 10) % 11

          if (remainder === 10 || remainder === 11) {
            remainder = 0
          }

          if (remainder !== parseInt(cpf.charAt(10))) return callback(new Error('CPF não é inválido'))

          // CPF válido
          return callback()
        } else {
          return callback()
        }
      } else {
        return callback()
      }
    },

    mxValidaCnsUsuario(rule, value, callback) {
      // if (value === undefined || value.trim() === "") return callback(new Error('O campo cns é obrigatório'))
      if (value != undefined) {
        if (value != '' || value != null) {
          if (value.length === 0) return callback()
          if (value.length < 14) return callback(new Error('Número CNS inválido. O campo deve conter 15(quinze) dígitos'))
          if (value.match(/^[0-9]*$/)) return callback()
          else return callback(new Error('O campo cns é inválido'))
        } else {
          return callback()
        }
      } else {
        return callback()
      }
    },

    mxValidaEmailUsuario(rule, value, callback) {
      if (value === undefined || value.trim() === '') return callback(new Error('Campo obrigatório'))
      if (value.length < 10) return callback(new Error('O campo não pode ter menos que 15 caracteres'))
      if (value.match(/^[a-z*0-9]([\.-]?\w+)*([a-z0-9])@\w+([\.-]?\w+)*(\.\w{2,3})+$/)) return callback()
      else return callback(new Error('O campo email é inválido'))
    },

    mxValidaTelefoneIndividuo(rule, value, callback) {
      if (value === undefined || value.trim() === '') return callback(new Error('Campo obrigatório'))
      value = value.replace(/[^\d]+/g, '')
      if (value.length !== 11) return callback(new Error('O campo telefone deve conter exatamente 11 dígitos'))
      if (value.match(/^[1-9]{2}9?[6789]\d{7}$/)) return callback()
      else return callback(new Error('O campo telefone é inválido'))
    },
  }
}
