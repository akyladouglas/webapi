using AtendTeleMedicina.Application.Contracts.Integracao;
using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Application.Contracts.Security;
using AtendTeleMedicina.Application.Services.Integracao;
using AtendTeleMedicina.Application.Services.Nucleo;
using AtendTeleMedicina.Application.Services.Security;
using AtendTeleMedicina.Domain.Contracts.Repositories.Integracao;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Repositories.Security;
using AtendTeleMedicina.Domain.Contracts.Services.Integracao;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Security;
using AtendTeleMedicina.Domain.Services.Integracao;
using AtendTeleMedicina.Domain.Services.Nucleo;
using AtendTeleMedicina.Domain.Services.Security;
using AtendTeleMedicina.Infra.Data.Context;
using AtendTeleMedicina.Infra.Data.Repositories.Integracao;
using AtendTeleMedicina.Infra.Data.Repositories.Nucleo;
using AtendTeleMedicina.Infra.Data.Repositories.Security;
using Microsoft.Extensions.DependencyInjection;

namespace AtendTeleMedicina.Infra.CrossCutting.Ioc
{
    public static class BootStrapper
    {
        public static void Register(IServiceCollection services)
        {
            services.AddTransient<IMySqlContext, MySqlContext>();

            services.AddTransient<IUserApplication, UserApplication>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddTransient<IConfiguracaoApplication, ConfiguracaoApplication>();
            services.AddTransient<IConfiguracaoService, ConfiguracaoService>();
            services.AddTransient<IConfiguracaoRepository, ConfiguracaoRepository>();

            //Nucleo

            services.AddTransient<IBairroApplication, BairroApplication>();
            services.AddTransient<IBairroService, BairroService>();
            services.AddTransient<IBairroRepository, BairroRepository>();

            services.AddTransient<ICidadeApplication, CidadeApplication>();
            services.AddTransient<ICidadeService, CidadeService>();
            services.AddTransient<ICidadeRepository, CidadeRepository>();

            services.AddTransient<IUnidadeFederativaApplication, UnidadeFederativaApplication>();
            services.AddTransient<IUnidadeFederativaService, UnidadeFederativaService>();
            services.AddTransient<IUnidadeFederativaRepository, UnidadeFederativaRepository>();

            services.AddTransient<IIndividuoApplication, IndividuoApplication>();
            services.AddTransient<IIndividuoService, IndividuoService>();
            services.AddTransient<IIndividuoRepository, IndividuoRepository>();
            services.AddTransient<IPrimeiroAcessoApplication, PrimeiroAcessoApplication>();

            services.AddTransient<IIndividuoGlicemiaApplication, IndividuoGlicemiaApplication>();
            services.AddTransient<IIndividuoGlicemiaService, IndividuoGlicemiaService>();
            services.AddTransient<IIndividuoGlicemiaRepository, IndividuoGlicemiaRepository>();

            services.AddTransient<IProcedimentoApplication, ProcedimentoApplication>();
            services.AddTransient<IProcedimentoService, ProcedimentoService>();
            services.AddTransient<IProcedimentoRepository, ProcedimentoRepository>();

            services.AddTransient<IEstabelecimentoApplication, EstabelecimentoApplication>();
            services.AddTransient<IEstabelecimentoService, EstabelecimentoService>();
            services.AddTransient<IEstabelecimentoRepository, EstabelecimentoRepository>();

            services.AddTransient<IEstabelecimentoProcedimentoApplication, EstabelecimentoProcedimentoApplication>();
            services.AddTransient<IEstabelecimentoProcedimentoService, EstabelecimentoProcedimentoService>();
            services.AddTransient<IEstabelecimentoProcedimentoRepository, EstabelecimentoProcedimentoRepository>();

            services.AddTransient<IEstabelecimentoProcedimentoHorarioApplication, EstabelecimentoProcedimentoHorarioApplication>();
            services.AddTransient<IEstabelecimentoProcedimentoHorarioService, EstabelecimentoProcedimentoHorarioService>();
            services.AddTransient<IEstabelecimentoProcedimentoHorarioRepository, EstabelecimentoProcedimentoHorarioRepository>();

            services.AddTransient<INotificacaoApplication, NotificacaoApplication>();
            services.AddTransient<INotificacaoService, NotificacaoService>();
            services.AddTransient<INotificacaoRepository, NotificacaoRepository>();

            services.AddTransient<ICustomizacaoApplication, CustomizacaoApplication>();
            services.AddTransient<ICustomizacaoService, CustomizacaoService>();
            services.AddTransient<ICustomizacaoRepository, CustomizacaoRepository>();

            services.AddTransient<IAcompanhamentoApplication, AcompanhamentoApplication>();
            services.AddTransient<IAcompanhamentoService, AcompanhamentoService>();
            services.AddTransient<IAcompanhamentoRepository, AcompanhamentoRepository>();

            services.AddTransient<IInformacoesUteisApplication, InformacoesUteisApplication>();
            services.AddTransient<IInformacoesUteisService, InformacoesUteisService>();
            services.AddTransient<IInformacoesUteisRepository, InformacoesUteisRepository>();

            services.AddTransient<IProfissionalApplication, ProfissionalApplication>();
            services.AddTransient<IProfissionalService, ProfissionalService>();
            services.AddTransient<IProfissionalRepository, ProfissionalRepository>();

            services.AddTransient<ISatisfacaoApplication, SatisfacaoApplication>();
            services.AddTransient<ISatisfacaoService, SatisfacaoService>();
            services.AddTransient<ISatisfacaoRepository, SatisfacaoRepository>();

            services.AddTransient<IAgendamentoApplication, AgendamentoApplication>();
            services.AddTransient<IAgendamentoService, AgendamentoService>();
            services.AddTransient<IAgendamentoRepository, AgendamentoRepository>();

            services.AddTransient<IEstabelecimentoProfissionalApplication, EstabelecimentoProfissionalApplication>();
            services.AddTransient<IEstabelecimentoProfissionalService, EstabelecimentoProfissionalService>();
            services.AddTransient<IEstabelecimentoProfissionalRepository, EstabelecimentoProfissionalRepository>();

            services.AddTransient<IAgendamentoParticipanteApplication, AgendamentoParticipanteApplication>();
            services.AddTransient<IAgendamentoParticipanteService, AgendamentoParticipanteService>();
            services.AddTransient<IAgendamentoParticipanteRepository, AgendamentoParticipanteRepository>();

            services.AddTransient<IDocumentoApplication, DocumentoApplication>();
            services.AddTransient<IDocumentoService, DocumentoService>();
            services.AddTransient<IDocumentoRepository, DocumentoRepository>();

            services.AddTransient<IDashboardApplication, DashboardApplication>();
            services.AddTransient<IDashboardService, DashboardService>();
            services.AddTransient<IDashboardRepository, DashboardRepository>();

            services.AddTransient<IIndividuoProcedimentoAutorizacaoApplication, IndividuoProcedimentoAutorizacaoApplication>();
            services.AddTransient<IIndividuoProcedimentoAutorizacaoService, IndividuoProcedimentoAutorizacaoService>();
            services.AddTransient<IIndividuoProcedimentoAutorizacaoRepository, IndividuoProcedimentoAutorizacaoRepository>();

            services.AddTransient<ICidApplication, CidApplication>();
            services.AddTransient<ICidService, CidService>();
            services.AddTransient<ICidRepository, CidRepository>();

            services.AddTransient<ICiapApplication, CiapApplication>();
            services.AddTransient<ICiapService, CiapService>();
            services.AddTransient<ICiapRepository, CiapRepository>();

            services.AddTransient<IAtendimentoApplication, AtendimentoApplication>();
            services.AddTransient<IAtendimentoService, AtendimentoService>();
            services.AddTransient<IAtendimentoRepository, AtendimentoRepository>();

            services.AddTransient<IIndividuoCiapApplication, IndividuoCiapApplication>();
            services.AddTransient<IIndividuoCiapService, IndividuoCiapService>();
            services.AddTransient<IIndividuoCiapRepository, IndividuoCiapRepository>();

            services.AddTransient<IIndividuoCid10Application, IndividuoCid10Application>();
            services.AddTransient<IIndividuoCid10Service, IndividuoCid10Service>();
            services.AddTransient<IIndividuoCid10Repository, IndividuoCid10Repository>();
            
            services.AddTransient<IIndividuoProcedimentoApplication, IndividuoProcedimentoApplication>();
            services.AddTransient<IIndividuoProcedimentoService, IndividuoProcedimentoService>();
            services.AddTransient<IIndividuoProcedimentoRepository, IndividuoProcedimentoRepository>();

            services.AddTransient<IAuditoriaApplication, AuditoriaApplication>();
            services.AddTransient<IAuditoriaService, AuditoriaService>();
            services.AddTransient<IAuditoriaRepository, AuditoriaRepository>();

            services.AddTransient<IExamesApplication, ExamesApplication>();
            services.AddTransient<IExamesService, ExamesService>();
            services.AddTransient<IExamesRepository, ExamesRepository>();

            services.AddTransient<ITipoExameApplication, TipoExameApplication>();
            services.AddTransient<ITipoExameService, TipoExameService>();
            services.AddTransient<ITipoExameRepository, TipoExameRepository>();

            services.AddTransient<IExamesF200Application, ExamesF200Application>();
            services.AddTransient<IExamesF200Service, ExamesF200Service>();
            services.AddTransient<IExamesF200Repository, ExamesF200Repository>();

            services.AddTransient<IExamesAfinion2Application, ExamesAfinion2Application>();
            services.AddTransient<IExamesAfinion2Service, ExamesAfinion2Service>();
            services.AddTransient<IExamesAfinion2Repository, ExamesAfinion2Repository>();

            services.AddTransient<IIndividuoNotificacaoApplication, IndividuoNotificacaoApplication>();
            services.AddTransient<IIndividuoNotificacaoService, IndividuoNotificacaoService>();
            services.AddTransient<IIndividuoNotificacaoRepository, IndividuoNotificacaoRepository>();

            services.AddTransient<IPaisApplication, PaisApplication>();
            services.AddTransient<IPaisService, PaisService>();
            services.AddTransient<IPaisRepository, PaisRepository>();

            services.AddTransient<ILoteApplication, LoteApplication>();
            services.AddTransient<ILoteService, LoteService>();
            services.AddTransient<ILoteRepository, LoteRepository>();

            services.AddTransient<IOcupacaoApplication, OcupacaoApplication>();
            services.AddTransient<IOcupacaoService, OcupacaoService>();
            services.AddTransient<IOcupacaoRepository, OcupacaoRepository>();

            services.AddTransient<IInterconsultaApplication, InterconsultaApplication>();
            services.AddTransient<IInterconsultaService, InterconsultaService>();
            services.AddTransient<IInterconsultaRepository, InterconsultaRepository>();

            services.AddTransient<IInterconsultaCidApplication, InterconsultaCidApplication>();
            services.AddTransient<IInterconsultaCidService, InterconsultaCidService>();
            services.AddTransient<IInterconsultaCidRepository, InterconsultaCidRepository>();
            
            services.AddTransient<IInterconsultaCiapApplication, InterconsultaCiapApplication>();
            services.AddTransient<IInterconsultaCiapService, InterconsultaCiapService>();
            services.AddTransient<IInterconsultaCiapRepository, InterconsultaCiapRepository>();

            services.AddTransient<IAvaliacaoApplication, AvaliacaoApplication>();
            services.AddTransient<IAvaliacaoService, AvaliacaoService>();
            services.AddTransient<IAvaliacaoRepository, AvaliacaoRepository>();
        }
    }
}
