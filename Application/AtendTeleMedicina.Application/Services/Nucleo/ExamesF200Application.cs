using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class ExamesF200Application : IExamesF200Application
    {
        private readonly IExamesF200Service _examesF200Service;
        public ExamesF200Application(IExamesF200Service examesF200Service)
        {
            _examesF200Service = examesF200Service;
        }

        public int Add(ExamesF200 obj)
        {
            return _examesF200Service.Add(obj);
        }

        public int Update(string id, ExamesF200 obj)
        {
            return _examesF200Service.Update(id, obj);
        }

        //public int Delete(string id)
        //{
        //    return _documentoService.Delete(id);
        //}

        public ExamesF200 GetById(string id)
        {
            return _examesF200Service.GetById(id);
        }

        public Task<(IEnumerable<ExamesF200>, int)> GetByParam(ExamesF200 filters, string sort, int? skip, int? take)
        {
            return _examesF200Service.GetByParam(filters, sort, skip, take);
        }

       public Task<(IEnumerable<ExamesF200>, int)> GetByCpfIndividuo(string cpf)
        {
            return _examesF200Service.GetByCpfIndividuo(cpf);
        }

    }
}