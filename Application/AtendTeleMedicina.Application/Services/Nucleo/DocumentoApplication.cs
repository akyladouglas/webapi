using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class DocumentoApplication : IDocumentoApplication
    {
        private readonly IDocumentoService _documentoService;
        public DocumentoApplication(IDocumentoService agendamentoService)
        {
            _documentoService = agendamentoService;
        }

        public int Add(Documento obj)
        {
            return _documentoService.Add(obj);
        }

        public int Update(string id, Documento obj)
        {
            return _documentoService.Update(id, obj);
        }

        public int Delete(string id)
        {
            return _documentoService.Delete(id);
        }

        public Documento GetById(string id)
        {
            return _documentoService.GetById(id);
        }

        public Task<(IEnumerable<Documento>, int)> GetByParam(Documento filters, string sort, int? skip, int? take)
        {
            return _documentoService.GetByParam(filters, sort, skip, take);
        }
    }
}
