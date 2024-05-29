using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class PrimeiroAcessoApplication : IPrimeiroAcessoApplication
    {
        private readonly IIndividuoService _individuoService;

        public PrimeiroAcessoApplication(IIndividuoService individuoService)
        {
           _individuoService = individuoService;
        }

        public async Task<(IEnumerable<Individuo>, int)> FindUserExternalService(Individuo individuo)
        {

            ValidaCpf(individuo.Cpf);
            ValidaCpf(individuo.Cpf);
            ValidaCpf(individuo.Cpf);
            var savedIndividuo = _individuoService.Add(individuo);
            var individuoSearch = new Individuo();
            individuoSearch.Cpf = individuo.Cpf;
            var saved = await _individuoService.GetByParam(individuoSearch, "NomeCompleto", 1, 1);
            return saved;


        }
        public async Task<(IEnumerable<Individuo>, int)> ConfirmData(Individuo individuo)
        {
            ValidaCpf(individuo.Cpf);
            var individuoSearch = new Individuo();
            individuoSearch.Cpf = individuo.Cpf;
            var saved = await _individuoService.GetByParam(individuoSearch, "NomeCompleto", 1, 1);
            return saved;
        }


        public Task<(IEnumerable<Individuo>, int)> Add(Individuo individuo)
        {
            throw new NotImplementedException();
        }

        protected void ValidaCpf(string cpf)
        {
            var cpfRgerx = new Regex(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$", RegexOptions.IgnoreCase);
            var isValid = cpfRgerx.IsMatch(cpf);
            if (!isValid)
            {
                throw new Exception("Cpf invalido");
            }
        }
    }
}
