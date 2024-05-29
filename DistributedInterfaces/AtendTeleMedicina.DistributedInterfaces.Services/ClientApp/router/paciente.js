import Agenda from '../pages/agenda/Lista'
import CadastroPaciente from '../pages/individuo/Lista'
import HistoricoSintomas from '../pages/sintomas/Sintoma'
import Download from '../pages/Individuo/DownloadDocs'
export default {
  children: [

    {
      path: '/agenda',
      component: Agenda,
      name: 'Agenda',
      meta: {
        auth: ['Administrador', 'Médico', 'Recepcionista', 'Triagem', 'MédicoAD'],
        title: 'Agendamento',
        hidden: false
      }
    },

    {
      path: '/cadastroPaciente',
      component: CadastroPaciente,
      name: 'CadastroPaciente',
      meta: {
        auth: ['Administrador', 'Médico', 'Recepcionista', 'Triagem'],
        title: 'Cadastro Paciente',
        hidden: false
      }
    },
    {
      path: '/Download',
      component: Download,
      name: 'Download',
      meta: {
        auth: ['Administrador', 'Recepcionista'],
        title: 'Download',
        hidden: false
      }
    },
    {
      path: '/HistoricoSintomas',
      component: HistoricoSintomas,
      name: 'HistoricoSintomas',
      meta: {
        auth: ['Administrador', 'Médico', 'Recepcionista', 'Triagem'],
        title: 'Histórico de Sintomas',
        hidden: true
      }
    },
    //{
    //  path: '/monitoramento',
    //  component: Monitoramento,
    //  name: 'Monitoramento',
    //  meta: {
    //    auth: ['Administrador', 'Médico', 'Recepcionista', 'MédicoEspecialista', 'Triagem'],
    //    title: 'Monitoramento',
    //    hidden: false
    //  }
    //},

  ]
}
