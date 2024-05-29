import Vue from 'vue'
import Router from 'vue-router'

import Full from 'components/containers/Full'
import Login from 'components/security/Login'
import Erro403 from 'components/security/Erro403'
import RedefinirSenha from 'components/security/SenhaIndividuo'
import RedefinirSenhaIndividuo from 'components/security/SenhaIndividuoRedefinir'
import RecuperarSenhaSMS from 'components/security/RecuperarSenhaSMS'
import RecuperarSenhaEmail from 'components/security/RecuperarSenhaEmail'
import SelecionarPerfil from 'components/security/SelecionarPerfil'
import Home from '../pages/Home'
import FAQ from '../pages/FAQ'
import DashBoardGeral from '../pages/dashboard/DashBoardGeral'

import AtendimentoTriagem from '../pages/atendimento/ListaTriagem'
import Atendimento from '../pages/atendimento/Lista'

import MenuHistorico from '../pages/atendimento/MenuHistorico'

import AgendamentosLista from '../pages/agendamento/Lista'
import AgendaCalendario from '../pages/agenda/AgendaCalendario'

import Ausentes from '../pages/ausentes/Lista'

import TeleAtendimento from '../pages/chat/TeleAtendimento'
import VideoBkp from '../pages/chat/VideoBkp'
import ChatRecepcao from '../pages/atendimento/ChatRecepcao'
import AgendarInterconsulta from '../pages/interconsulta/AgendarInterconsulta'
import Interconsulta from '../pages/interconsulta/Interconsulta'

import childrenAdmin from './admin'
import childrenRelatorio from './relatorio'
import childrenPaciente from './paciente'
import childrenPerfil from './perfil'

Vue.use(Router)

export default new Router({
  mode: 'history',
  routes: [
    {
      path: '/',
      redirect: '/home',
      name: 'Inicio',
      component: Full,
      children: [
        {
          path: '/',
          component: Home,
          name: 'Home',
          meta: {
            auth: true,
            titulo: 'Home',
            oculta: false
          }
        },
        {
          path: '/dashboard-geral',
          component: DashBoardGeral,
          name: 'DashBoardGeral',
          meta: {
            auth: true,
            titulo: 'DashBoard',
            oculta: false
          }
        },
        {
          path: '/AgendarInterconsulta',
          component: AgendarInterconsulta,
          name: 'AgendarInterconsulta',
          meta: {
            auth: true,
            titulo: 'AgendarInterconsulta',
            oculta: false
          }
        },
        {
          path: '/Interconsulta',
          component: Interconsulta,
          name: 'Interconsulta',
          meta: {
            auth: true,
            titulo: 'Interconsulta',
            oculta: false
          }
        },
        {
          path: '/chat',
          component: TeleAtendimento,
          name: 'TeleAtendimento',
          meta: {
            auth: true,
            titulo: 'TeleAtendimento',
            oculta: false
          }
        },
        {
          path: '/chat-recepcao',
          component: ChatRecepcao,
          name: 'ChatRecepcao',
          meta: {
            auth: true,
            titulo: 'Chat Recepcao',
            oculta: false
          }
        },
        {
          path: '/VideoBkp',
          component: VideoBkp,
          name: 'VideoBkp',
          meta: {
            auth: true,
            titulo: 'VideoBkp',
            oculta: false
          }
        },
        {
          path: '/ausentes',
          component: Ausentes,
          name: 'Ausentes',
          meta: {
            auth: ['Médico', 'Recepcionista', 'Triagem'],
            titulo: 'Ausentes',
            oculta: true
          }
        },
        {
          path: 'admin',
          redirect: '/administracao/index',
          name: 'Container Administração',
          meta: {
            auth: 'Administrador',
            title: 'Administração',
            hidden: false
          },
          component: {
            render (c) {
              return c('router-view')
            }
          },
          ...childrenAdmin
        },
        {
          path: 'relatorios',
          redirect: '/relatorios/index',
          name: 'Container Relatórios',
          meta: {
            auth: ['GestorDoEstado', 'Administrador'],
            title: 'Relatorios',
            hidden: false
          },
          component: {
            render (c) {
              return c('router-view')
            }
          },
          ...childrenRelatorio
        },
        {
          path: 'paciente',
          redirect: '/paciente/index',
          name: 'Container Pacientes',
          meta: {
            auth: ['Recepcionista', 'Triagem'],
            title: 'Paciente',
            hidden: false
          },
          component: {
            render (c) {
              return c('router-view')
            }
          },
          ...childrenPaciente
        },
        {
          path: 'perfil',
          redirect: '/perfil',
          name: 'Perfil',
          meta: {
            auth: ['Administrador', 'GestorEstado', 'GestorMunicipio', 'AdmMunicipio', 'Atendente', 'Médico'],
            title: 'Perfil',
            hidden: true
          },
          component: {
            render (c) {
              return c('router-view')
            }
          },
          ...childrenPerfil
        },
        {
          path: '/agenda-calendario',
          component: AgendaCalendario,
          name: 'AgendaCalendario',
          meta: {
            auth: ['Administrador', 'Ilha', 'Médico', 'Recepcionista', 'MédicoEspecialista'],
            titulo: 'Agendamentos',
            oculta: false
          }
        },
        {
          path: '/agendamentos',
          component: AgendamentosLista,
          name: 'Agendamentos',
          meta: {
            auth: ['Administrador', 'Médico', 'Recepcionista', 'MédicoEspecialista', 'Triagem', 'MédicoAD'],
            titulo: 'Atender Cidadão',
            oculta: false
          }
        },
        
        {
          path: '/atendimento',
          component: Atendimento,
          name: 'Atendimento',
          meta: {
            auth: ['Administrador', 'Ilha', 'Médico', 'MédicoEspecialista', 'Triagem', 'MédicoAD'],
            titulo: 'Atendimento ao Paciente',
            oculta: false
          }
        },
        {
          path: '/AtendimentoTriagem',
          component: AtendimentoTriagem,
          name: 'AtendimentoTriagem',
          meta: {
            auth: ['Administrador', 'Ilha', 'Recepcionista', 'Triagem'],
            titulo: 'Atendimento ao Paciente',
            oculta: false
          }
        },
        {
          path: '/MenuHistorico',
          component: MenuHistorico,
          name: 'MenuHistorico',
          meta: {
            auth: ['Administrador', 'Médico', 'MédicoEspecialista', 'Recepcionista', 'Triagem', 'MédicoAD'],
            titulo: 'MenuHistorico',
            oculta: false
          }
        },
        {
          path: '/selecionarPerfil',
          name: 'SelecionarPerfil',
          component: SelecionarPerfil,
          meta: {
            auth: ['Administrador', 'Médico', 'MédicoEspecialista', 'Recepcionista', 'Triagem', 'MédicoAD'],
            titulo: 'Selecionar Perfil',
            oculta: false
          }
        },
        {
          path: '/403',
          name: 'Erro403',
          component: Erro403,
          meta: {auth: false}
        }
      ]
    },
    // FORA DO CONTAINER
    {
      path: '/login',
      name: 'Login',
      component: Login,
      meta: {auth: false}
    },
    {
      path: '/redefinir-senha',
      name: 'RedefinirSenha',
      component: RedefinirSenha,
      meta: {auth: false}
    },
    // caso o profissional ou usuario tenha sua senha resetada ira cair nessa tela para o mesmo redefinir uma senha particular dele
    {
      path: '/redefinir-senha-individuo',
      name: 'RedefinirSenhaIndividuo',
      component: RedefinirSenhaIndividuo,
      meta: { auth: false }
    },
    {
      path: '/redefinir-senha-sms',
      name: 'RecuperarSenhaSMS',
      component: RecuperarSenhaSMS,
      meta: { auth: false }
    },
    {
      path: '/redefinir-senha-email',
      name: 'RecuperarSenhaEmail',
      component: RecuperarSenhaEmail,
      meta: { auth: false }
    },

    {
      path: '/faq',
      name: 'FAQ',
      component: FAQ,
      meta: {auth: false}
    }
  ]
})
