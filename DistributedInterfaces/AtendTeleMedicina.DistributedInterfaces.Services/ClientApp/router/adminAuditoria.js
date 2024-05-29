import AdminAuditoria from '../pages/admin/Auditoria/Auditoria'

export default {
  children: [
    {
      path: '/auditoria',
      name: 'Auditoria',
      component: AdminAuditoria,
      meta: {
        auth: ['Administrador', 'Auditor'],
        title: 'Auditoria',
        hidden: true
      }
    }
  ]
}
