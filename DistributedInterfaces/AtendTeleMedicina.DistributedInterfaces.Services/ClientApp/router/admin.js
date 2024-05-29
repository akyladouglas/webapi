import AdminUsuario from '../pages/admin/Usuario/Usuario'
import childrenAdminEstabelecimento from './adminEstabelecimento'
import childrenAdminProfissional from './adminProfissional'
import childrenAdminProcedimento from './adminProcedimento'
// import childrenAdminPaciente from './adminPaciente'
import childrenAdminNotificacao from './adminNotificacao'
import childrenAdminInformacao from './adminInformacao'
import childrenAdminHistorico from './adminHistoricoGeral'
/* import childrenAdminEscala from './adminEscala' */
import childrenAdminAuditoria from './adminAuditoria'
import childrenAdminIntegracao from './adminIntegracao'
import childrenAdminConfiguracao from './adminConfiguracao'
// import AdminPecMeedsPaciente from '../pages/admin/integracao/PecMeedsPaciente'

export default {
  children: [
    {
      path: '/admin/usuario',
      name: 'AdminUsuario',
      component: AdminUsuario,
      meta: {
        auth: ['Administrador', 'GestorEstado', 'AdmMunicipio'],
        title: 'Usuários',
        hidden: false
      }
    },
    {
      path: 'estabelecimento',
      redirect: { name: 'Estabelecimentos' },
      name: 'AdminEstabelecimento',
      meta: {
        auth: ['GestorDoEstado', 'Administrador'],
        title: 'Estabelecimento',
        hidden: false
      },
      component: {
        render (c) {
          return c('router-view')
        }
      }
    },
    {
      path: 'profissional',
      redirect: { name: 'Profissionais' },
      name: 'AdminProfissional',
      meta: {
        auth: ['GestorDoEstado', 'Administrador'],
        title: 'Profissional',
        hidden: false
      },
      component: {
        render (c) {
          return c('router-view')
        }
      }
    },
    {
      path: 'procedimento',
      redirect: { name: 'Procedimentos' },
      name: 'AdminProcedimento',
      meta: {
        auth: ['GestorDoEstado', 'Administrador'],
        title: 'Procedimento',
        hidden: false
      },
      component: {
        render (c) {
          return c('router-view')
        }
      }
    },
    {
      path: 'notificacao',
      redirect: { name: 'notificacao' },
      name: 'Notificacao',
      meta: {
        auth: ['Administrador', 'GestorEstado', 'GestorMunicipio'],
        title: 'Notificação',
        hidden: false
      },
      component: {
        render (c) {
          return c('router-view')
        }
      }
    },
    {
      path: 'informacao',
      redirect: { name: 'informacao' },
      name: 'Informacao',
      meta: {
        auth: ['Administrador', 'GestorEstado', 'GestorMunicipio'],
        title: 'Informações Úteis',
        hidden: false
      },
      component: {
        render (c) {
          return c('router-view')
        }
      }
    },
    {
      path: 'historico',
      redirect: { name: 'historico' },
      name: 'Historico',
      meta: {
        auth: ['GestorDoEstado', 'Administrador'],
        title: 'Histórico Geral',
        hidden: false
      },
      component: {
        render (c) {
          return c('router-view')
        }
      }
    },
    {
      path: 'auditoria',
      redirect: { name: 'Auditoria' },
      name: 'AdminAuditoria',
      meta: {
        auth: ['Administrador'],
        title: 'Auditoria',
        hidden: false
      },
      component: {
        render (c) {
          return c('router-view')
        }
      }
    },
    {
      path: 'integracao',
      redirect: { name: 'Integracao' },
      name: 'AdminIntegracao',
      meta: {
        auth: ['Administrador'],
        title: 'Integração',
        hidden: false
      },
      component: {
        render (c) {
          return c('router-view')
        }
      }
    },
    {
      path: 'configuracao',
      redirect: { name: 'Configuracao' },
      name: 'AdminConfiguracao',
      meta: {
        auth: ['Administrador'],
        title: 'Configuração',
        hidden: false
      },
      component: {
        render (c) {
          return c('router-view')
        }
      }
    },
    // {
    //   path: '/admin/importar-paciente',
    //   name: 'AdminPecMeedsPaciente',
    //   component: AdminPecMeedsPaciente,
    //   meta: {
    //     auth: ['Administrador'],
    //     title: 'Importar Paciente',
    //     hidden: false
    //   }
    // },

    ...childrenAdminEstabelecimento.children,
    ...childrenAdminProfissional.children,
    ...childrenAdminProcedimento.children,
    // ...childrenAdminPaciente.children,
    ...childrenAdminNotificacao.children,
    ...childrenAdminInformacao.children,
    ...childrenAdminHistorico.children,
    // ...childrenAdminEscala.children,
    ...childrenAdminAuditoria.children,
    ...childrenAdminIntegracao.children,
    ...childrenAdminConfiguracao.children
  ]
}
