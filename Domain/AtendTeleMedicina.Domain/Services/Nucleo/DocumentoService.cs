using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class DocumentoService : IDocumentoService
    {
        private readonly IDocumentoRepository _documentoRepository;

        #region -- Constructor
        public DocumentoService(IDocumentoRepository agendamentoRepository)
        {
            _documentoRepository = agendamentoRepository;
        }
        #endregion

        public int Add(Documento obj)
        {
            obj.Id = Guid.NewGuid().ToString();
            obj.DataCadastro = DateTime.Now;
            return _documentoRepository.Add(obj);
        }
        public int Update(string id, Documento obj)
        {
            return _documentoRepository.Update(id, obj);
        }
        public int Delete(string id)
        {
            return _documentoRepository.Delete(id);
        }

        public Documento GetById(string id)
        {
            return _documentoRepository.GetById(id);
        }
        public Task<(IEnumerable<Documento>, int)> GetByParam(Documento filters, string sort, int? skip, int? take)
        {
            return _documentoRepository.GetByParam(filters, sort, skip, take);
        }
    }
}
